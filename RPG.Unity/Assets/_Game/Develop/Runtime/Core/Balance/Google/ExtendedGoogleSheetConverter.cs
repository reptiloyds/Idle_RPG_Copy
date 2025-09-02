#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cathei.BakingSheet.Internal;
using Cathei.BakingSheet.Raw;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Tool;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.Balance.Google
{
    public class ExtendedGoogleSheetConverter : RawSheetImporter
    {
        private string _gsheetAddress;
        private readonly ICredential _credential;
        private Spreadsheet _spreadsheet;
        private readonly Dictionary<string, List<Page>> _pages = new ();
        private readonly ILogger _logger;
        private readonly Dictionary<string, List<string>> _subSheets;

        public ExtendedGoogleSheetConverter(string gsheetAddress, string credential, ILogger logger, Dictionary<string, List<string>> subSheets,
            TimeZoneInfo timeZoneInfo = null, IFormatProvider formatProvider = null)
            : base(timeZoneInfo, formatProvider)
        {
            _gsheetAddress = gsheetAddress;
            _credential = GoogleCredential.
                FromJson(credential).
                CreateScoped(new[] { DriveService.Scope.DriveReadonly });
            _logger = logger;
            _subSheets = subSheets;
        }

        public async Task<DateTime> FetchModifiedTime()
        {
            using (var service = new DriveService(new BaseClientService.Initializer() {
                HttpClientInitializer = _credential
            }))
            {
                var fileReq = service.Files.Get(_gsheetAddress);
                fileReq.SupportsTeamDrives = true;
                fileReq.Fields = "modifiedTime";

                var file = await fileReq.ExecuteAsync();
                return file.ModifiedTime ?? default;
            }
        }

        public override void Reset()
        {
            base.Reset();
            _pages.Clear();
        }

        protected override async Task<bool> LoadData()
        {
            using (var service = new SheetsService(new BaseClientService.Initializer() {
                HttpClientInitializer = _credential
            }))
            {
                var sheetReq = service.Spreadsheets.Get(_gsheetAddress);
                sheetReq.Fields = "sheets(properties.title,data.rowData.values.formattedValue)";
                _spreadsheet = await sheetReq.ExecuteAsync();
            }

            _pages.Clear();

            foreach (var sheetListData in _spreadsheet.Sheets)
            {
                if (sheetListData.Properties.Title.StartsWith(Config.Comment))
                    continue;

                var (sheetName, subName) = Config.ParseSheetName(sheetListData.Properties.Title);

                if (_subSheets.TryGetValue(sheetName, out var subSheets))
                {
                    foreach (var subSheet in subSheets)
                    {
                        if (!_pages.TryGetValue(subSheet, out var sheetList))
                        {
                            sheetList = new List<Page>();
                            _pages.Add(subSheet, sheetList);
                        }
                        
                        var subGrid = GetSubGridData(sheetListData.Data.First(), sheetName, subSheet);
                        sheetList.Add(new Page(subGrid, subSheet));
                    }
                }
                else
                {
                    if (!_pages.TryGetValue(sheetName, out var sheetList))
                    {
                        sheetList = new List<Page>();
                        _pages.Add(sheetName, sheetList);
                    }
                    
                    sheetList.Add(new Page(sheetListData.Data.First(), subName));   
                }
            }
            return true;
        }
        
        private class Page : IRawSheetImporterPage
        {
            private readonly GridData _grid;

            public string SubName { get; }

            public Page(GridData gridData, string subName)
            {
                _grid = gridData;
                SubName = subName;
            }

            public string GetCell(int col, int row)
            {
                if (row >= _grid.RowData.Count ||
                    col >= _grid.RowData[row].Values?.Count)
                    return null;

                var value = _grid.RowData[row].Values?[col];
                return value?.FormattedValue;
            }
        }

        protected override IEnumerable<IRawSheetImporterPage> GetPages(string sheetName)
        {
            if (_pages.TryGetValue(sheetName, out var pages))
                return pages;
            return Enumerable.Empty<IRawSheetImporterPage>();
        }
        
        private GridData GetSubGridData(GridData gridData, string sheetName, string subTableTag)
        {
            IList<IList<object>> rowsValue = new List<IList<object>>();
            foreach (var rowData in gridData.RowData)
            {
                if(rowData.Values == null) continue;
                
                var row = new List<object>();
                foreach (var cellData in rowData.Values) 
                    row.Add(cellData.FormattedValue);
                
                rowsValue.Add(row);
            }
            
            IList<IList<object>> rowsOrigin = new List<IList<object>>();
            foreach (var rowData in gridData.RowData)
            {
                if(rowData.Values == null) continue;
                
                var row = new List<object>();
                foreach (var cellData in rowData.Values) 
                    row.Add(cellData);
                
                rowsOrigin.Add(row);
            }

            SubTableRect rect = ConfigSubTableHelper.GetSubTableRect(rowsValue, subTableTag);

            if (!rect.IsValid()) {
                _logger?.LogError($"Get config data error: sub table '{subTableTag}' not found in sheet {sheetName}");
                return null;
            }

            var newGrid = new GridData { RowData = new List<RowData>() };

            foreach (var rowNum in rect.IterateRow()) 
            {
                var rowData = new RowData();
                rowData.Values = new List<CellData>();
                newGrid.RowData.Add(rowData);
                
                foreach (var colNum in rect.IterateCol())
                {
                    var row = rowsOrigin[rowNum];
                    if (colNum >= row.Count)
                        rowData.Values.Add(new CellData());
                    else
                    {
                        var cell = row[colNum];
                        rowData.Values.Add(cell as CellData);   
                    }
                }
            }

            return newGrid;
        }
    }
}

#endif
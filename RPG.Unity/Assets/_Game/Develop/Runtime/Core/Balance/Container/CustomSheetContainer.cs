using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cathei.BakingSheet;
using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.Balance.Container
{
    public abstract class CustomSheetContainer : SheetContainerBase
    {
        [Inject] protected BalanceContainer Balance;
        private readonly ILogger _logger;
        private bool _isValidationNeeded;
        
        public CustomSheetContainer(ILogger logger) : base(logger)
        {
            _logger = logger;
        }
        
        public async Task<bool> BakeCustom(params ISheetImporter[] importers)
        {
            var context = new SheetConvertingContext
            {
                Container = this,
                Logger = _logger,
            };

            var properties = GetSheetProperties().Values;
            int propAmount = 0;
            foreach (var prop in properties)
            {
                // clear currently assigned sheets
                prop.SetValue(this, null);
                propAmount++;
            }
            Debug.LogWarning($"Prop cleared: {propAmount}");
            
            foreach (var importer in importers)
            {
                var success = await importer.Import(context);

                if (!success)
                    return false;
            }
            
            PostLoad();

            return true;
        }

        public override void PostLoad()
        {
            var context = new SheetConvertingContext
            {
                Container = this,
                Logger = _logger,
            };

            var properties = GetSheetProperties();

            var rowTypeToSheet = new Dictionary<Type, ISheet>(properties.Count);

            foreach (var pair in properties)
            {
                var sheet = pair.Value.GetValue(this) as ISheet;

                if (sheet == null)
                {
                    context.Logger.LogError("Failed to find sheet: {SheetName}", pair.Key);
                    continue;
                }

                sheet.Name = pair.Key;

                if (rowTypeToSheet.ContainsKey(sheet.RowType))
                {
                    // row type must be unique in a sheet container
                    context.Logger.LogWarning("Duplicated Row type is used for {SheetName}", pair.Key);
                    
                    continue;
                }

                rowTypeToSheet.Add(sheet.RowType, sheet);
            }

            // making sure all references are mapped before calling PostLoad
            foreach (var sheet in rowTypeToSheet.Values)
                sheet.MapReferences(context, rowTypeToSheet);

            foreach (var sheet in rowTypeToSheet.Values)
            {
                sheet.PostLoad(context);   
            }

            if (Application.isPlaying) 
                PostLoadRuntime();
            else
                PostLoadEditor();
        }

        protected virtual void PostLoadEditor()
        {
            
        }

        protected virtual void PostLoadRuntime()
        {
            
        }
        
        public Dictionary<string, List<string>> GetSubSheets()
        {
            var subSheets = new Dictionary<string, List<string>>();
            
            var propertiesInfo = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var propertyInfo in propertiesInfo)
            {
                var attribute = (SheetListAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(SheetListAttribute));
                if (attribute == null) continue;

                var sheetName = OverrideSubSheetName(attribute.SheetName);

                if (subSheets.TryGetValue(sheetName, out var list))
                    list.Add(propertyInfo.Name);
                else
                {
                    var newList = new List<string>();
                    subSheets.Add(sheetName, newList);
                    newList.Add(propertyInfo.Name);
                }
            } 

            return subSheets;
        }

        protected virtual string OverrideSubSheetName(string from) => from;
    }
}
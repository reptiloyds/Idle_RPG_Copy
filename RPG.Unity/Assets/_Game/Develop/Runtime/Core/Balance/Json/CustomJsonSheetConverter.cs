using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cathei.BakingSheet;
using Cathei.BakingSheet.Internal;
using Cathei.BakingSheet.Unity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Save.DataTransformers;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PleasantlyGames.RPG.Runtime.Core.Balance.Json
{
    public class CustomJsonSheetConverter : ISheetConverter
    {
        private readonly IDataTransformer _dataTransformer = new GzipDataCompressor(new MockDataTransformer());
        public virtual string Extension => "json";

        private readonly UnityLogger _logger;
        private readonly string _fileName;
        private readonly string _loadPath;
        private Dictionary<string, string> _dictionary = new();

        public CustomJsonSheetConverter(string path, string fileName)
        {
            _logger = new UnityLogger();
            _loadPath = path;
            _fileName = fileName;
        }

        public static void ErrorHandler(ILogger logError, ErrorEventArgs err)
        {
            if (err.ErrorContext.Member?.ToString() == nameof(ISheetRow.Id) &&
                err.ErrorContext.OriginalObject is ISheetRow &&
                !(err.CurrentObject is ISheet))
            {
                // if Id has error, the error must be handled on the sheet level
                return;
            }

            using (logError.BeginScope(err.ErrorContext.Path))
                logError.LogError(err.ErrorContext.Error, err.ErrorContext.Error.Message);

            err.ErrorContext.Handled = true;
        }

        public virtual JsonSerializerSettings GetSettings(ILogger logError, Formatting formatting = Formatting.None)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = formatting
            };
            
            settings.Error = (_, err) => ErrorHandler(logError, err);
            settings.ContractResolver = JsonSheetContractResolver.Instance;

            return settings;
        }

        protected virtual string Serialize(object obj, Type type, ILogger logger) => 
            JsonConvert.SerializeObject(obj, type, GetSettings(logger));

        protected virtual object Deserialize(string json, Type type, ILogger logger) => 
            JsonConvert.DeserializeObject(json, type, GetSettings(logger));

        public Task<bool> Import(SheetConvertingContext context)
        {
            var sheetProps = context.Container.GetSheetProperties();
            foreach (var pair in sheetProps)
            {
                using (context.Logger.BeginScope(pair.Key))
                {
                    var concreteData = _dictionary[pair.Key];
                    var sheet = Deserialize(concreteData, pair.Value.PropertyType, context.Logger) as ISheet;
                    pair.Value.SetValue(context.Container, sheet);
                }
            }

            return Task.FromResult(true);
        }

        public Task<bool> Export(SheetConvertingContext context)
        {
            var sheetProps = context.Container.GetSheetProperties();
            
            foreach (var pair in sheetProps)
            {
                using (context.Logger.BeginScope(pair.Key))
                {
                    var sheet = pair.Value.GetValue(context.Container);
                    _dictionary.Add(pair.Key, JsonConvert.SerializeObject(sheet, GetSettings(context.Logger)));
                }
            }

            return Task.FromResult(true);
        }

        public async Task<bool> ReadFromStreamingAssets()
        {
            var fileSystem = new StreamingAssetsFileSystem();
            string data;
            var path = Path.Combine(_loadPath, $"{_fileName}.{Extension}");
            if (!fileSystem.Exists(path))
                return false;
            using (var stream = fileSystem.OpenRead(path))
            using (var reader = new StreamReader(stream))
                data = await reader.ReadToEndAsync();

            data = _dataTransformer.Reverse(data);
            
            _dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(data, GetSettings(_logger, Formatting.Indented));

            return true;
        }
        
        public void ReadFromJson(string json)
        {
            json = _dataTransformer.Reverse(json);
            _dictionary =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(json,
                    GetSettings(_logger, Formatting.Indented));
        }

        public async Task WriteToStreamingAssets()
        {
            var fileSystem = new FileSystem();
            var content = JsonConvert.SerializeObject(_dictionary, GetSettings(_logger, Formatting.Indented));
            content = _dataTransformer.Transform(content);
            var path = Path.Combine(_loadPath, $"{_fileName}.{Extension}");
            
            using (var stream = fileSystem.OpenWrite(path))
            using (var writer = new StreamWriter(stream))
                await writer.WriteAsync(content);
        }
    }
}
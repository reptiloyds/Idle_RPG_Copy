#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cathei.BakingSheet.Unity;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Google;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Balance
{
    public abstract class BaseEditorBalanceParser : ScriptableObject
    {
        [Serializable]
        public class SheetConfig
        {
            public string Name;
            public string SheetId;
            [ReadOnly] public BaseEditorBalanceParser Parser;

            [Button]
            public void Parse() => 
                Parser.Parse(SheetId);
        }
        
        public TextAsset Credentials;
        public List<SheetConfig> Sheets;
        public bool Validation;

        private void OnValidate()
        {
            foreach (var sheet in Sheets) 
                sheet.Parser = this;
        }

        protected virtual void Parse(string sheetId)
        {
            if (Validation)
                SheetExt.EnableValidation();
            else
                SheetExt.DisableValidation();
        }
        
        protected async Task Parse(List<CustomSheetContainer> containers, string sheetId)
        {
#if UNITY_EDITOR
            var logger = new UnityLogger();
            var jsonConverter = new CustomJsonSheetConverter(ParseConst.JSONPath, BalanceConst.FileName);

            foreach (var container in containers)
            {
                var googleConverter = new ExtendedGoogleSheetConverter(sheetId, Credentials.ToString(), logger, container.GetSubSheets());
                await container.Bake(googleConverter);
                await container.Store(jsonConverter);
            }

            await jsonConverter.WriteToStreamingAssets();
#else
            return;
#endif
        }

        private Dictionary<string, List<string>> GetSubSheets(List<CustomSheetContainer> containers)
        {
            var subSheets = new Dictionary<string, List<string>>();
            foreach (var container in containers)
            {
                subSheets = subSheets.Concat(container.GetSubSheets())
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return subSheets;
        }
    }
}

#endif
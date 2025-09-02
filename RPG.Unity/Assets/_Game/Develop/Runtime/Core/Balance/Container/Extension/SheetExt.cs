using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension
{
    public static class SheetExt
    {
        private static bool _isValidationEnabled;
        public static bool IsValidationNeeded => !Application.isPlaying && _isValidationEnabled;

        public static void EnableValidation() => 
            _isValidationEnabled = true;

        public static void DisableValidation() => 
            _isValidationEnabled = false;
        
        public static List<string> ParseToStringList(string json)
        {
            if(json == null) return new List<string>();
            return JsonConvertLog.DeserializeObject<List<string>>(json);
        }

        public static void CheckAsset(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogError("Asset key is null or whitespace");
                return;
            }

            AssetProvider.HasAsset(key);
        }

        public static void CheckSprite(string atlasName, string spriteName)
        {
            if (string.IsNullOrWhiteSpace(atlasName) || string.IsNullOrWhiteSpace(spriteName))
            {
                Debug.LogError("Asset key is null or whitespace");
                return;
            }
        
            SpriteProvider.HasSprite(atlasName, spriteName);
        }
    }
}
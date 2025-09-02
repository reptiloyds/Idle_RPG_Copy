using System;
using Newtonsoft.Json;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Balance.Json
{
    public static class JsonConvertLog
    {
        public static T DeserializeObject<T>(string value)
        {
#if UNITY_EDITOR
            T result;
            try
            {
                result = JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {
                Debug.LogError($"Can`t parse {value}");
                throw;
            }

            return result;
#endif
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
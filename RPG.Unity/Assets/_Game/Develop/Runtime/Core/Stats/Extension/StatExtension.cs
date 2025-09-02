using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Stats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Stats.Extension
{
    public static class StatExtension
    {
        public static StatModifierData<T> DeserializeModifier<T>(string statModifier) where T : Enum => 
            JsonConvertLog.DeserializeObject<StatModifierData<T>>(statModifier);

        public static List<StatModifierData<T>> DeserializeModifiers<T>(string json) where T : Enum
        {
            if (string.IsNullOrWhiteSpace(json) || json == ParseConst.NullCell)
                return new List<StatModifierData<T>>(0);
                
            return JsonConvertLog.DeserializeObject<List<StatModifierData<T>>>(json);
        }
        
        public static ManualStatData<T> DeserializeManualStat<T>(string statModifier) where T : Enum => 
            JsonConvertLog.DeserializeObject<ManualStatData<T>>(statModifier);
        
        public static List<ManualStatData<T>> DeserializeManualStats<T>(string json) where T : Enum
        {
            if (string.IsNullOrWhiteSpace(json) || json == ParseConst.NullCell)
                return new List<ManualStatData<T>>(0);
            return JsonConvertLog.DeserializeObject<List<ManualStatData<T>>>(json);
        }

        public static List<UnitStatType> DeserializeStatList(string statList) =>
            JsonConvertLog.DeserializeObject<List<UnitStatType>>(statList);
    }
}
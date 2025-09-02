using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.BuyItem;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.ClearDungeon;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.ClearLevel;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.CompleteDailies;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.DefeatEnemy;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.SpinRoulette;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Model.WatchAd;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Type;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Extension
{
    public static class DailiesExtenstion
    {
        public static void TryDeserialize(this DailyType type, string json)
        {
            switch (type)
            {
                case DailyType.DefeatEnemy:
                    var defeatEnemyData = JsonConvertLog.DeserializeObject<DDefeatEnemyData>(json);
                    if(defeatEnemyData.Amount <= 0)
                        Debug.LogError("Amount must be > 0");
                    break;
                case DailyType.ClearLevel:
                    var clearLevelData = JsonConvertLog.DeserializeObject<DClearLevelData>(json);
                    if (clearLevelData.Amount <= 0) 
                        Debug.LogError("Amount must be > 0");
                    break;
                case DailyType.ClearDungeon:
                    var clearDungeonData = JsonConvertLog.DeserializeObject<DClearDungeonData>(json);
                    if (clearDungeonData.Amount <= 0) 
                        Debug.LogError("Amount must be > 0");
                    if (clearDungeonData.Type == GameModeType.None) 
                        Debug.LogError("Type must be defined");
                    break;
                case DailyType.BuyItem:
                    var buyItemData = JsonConvertLog.DeserializeObject<DBuyItemData>(json);
                    if (buyItemData.Amount <= 0) 
                        Debug.LogError("Amount must be > 0");
                    break;
                case DailyType.WatchAd:
                    var watchAdData = JsonConvertLog.DeserializeObject<DWatchAdData>(json);
                    if(watchAdData.Amount <= 0)
                        Debug.LogError("Amount must be > 0");
                    break;
                case DailyType.CompleteDailies:
                    var completeDailies = JsonConvertLog.DeserializeObject<DCompleteDailiesData>(json);
                    if(completeDailies.Amount <= 0)
                        Debug.LogError("Amount must be > 0");
                    break;
                case DailyType.SpinRoulette:
                    var spinRoulette = JsonConvertLog.DeserializeObject<DSpinRouletteData>(json);
                    if(spinRoulette.Amount <= 0)
                        Debug.LogError("Amount must be > 0");
                    break;
            }
        }
    }
}
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.BuyItem;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.CompleteDungeon;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.CompleteLevel;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.DefeatEnemy;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.EnhanceStat;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.SpinRoulette;
using PleasantlyGames.RPG.Runtime.Core.Quests.Type;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Extension
{
    public static class QuestExtension
    {
        public static void TryDeserialize(this QuestType type, string json)
        {
            switch (type)
            {
                case QuestType.DefeatEnemy:
                    var defeatEnemyData = JsonConvertLog.DeserializeObject<QDefeatEnemyData>(json);
                    if (defeatEnemyData.Amount <= 0) 
                        Debug.LogError("Amount must be > 0");
                    break;
                case QuestType.EnhanceStat:
                    var enhanceData = JsonConvertLog.DeserializeObject<QEnhanceStatData>(json);
                    if (enhanceData.Amount <= 0) 
                        Debug.LogError("Amount must be > 0");
                    if (enhanceData.Type == UnitStatType.None) 
                        Debug.LogError("Type must be defined");
                    break;
                case QuestType.BuyItem:
                    var buyItemData = JsonConvertLog.DeserializeObject<QBuyItemData>(json);
                    if (buyItemData.Amount <= 0) 
                        Debug.LogError("Amount must be > 0");
                    if (buyItemData.Type == ItemType.None) 
                        Debug.LogError("Type must be defined");
                    break;
                case QuestType.CompleteLevel:
                    var completeLevelData = JsonConvertLog.DeserializeObject<QCompleteLevelData>(json);
                    if (completeLevelData.Level <= 0) 
                        Debug.LogError("Level must be > 0");
                    if (completeLevelData.Id < 0 ) 
                        Debug.LogError("Id must be >= 0");
                    break;
                case QuestType.CompleteDungeon:
                    var completeDungeonData = JsonConvertLog.DeserializeObject<QCompleteDungeonData>(json);
                    if (completeDungeonData.Level <= 0) 
                        Debug.LogError("Level must be > 0");
                    if (completeDungeonData.Type == GameModeType.None || completeDungeonData.Type == GameModeType.Main) 
                        Debug.LogError("Type must be a LeveledDungeon Type");
                    break;
                case QuestType.SpinRoulette:
                    var spinRouletteData = JsonConvertLog.DeserializeObject<QSpinRouletteData>(json);
                    if (spinRouletteData.Amount <= 0) 
                        Debug.LogError("Spin amount must be > 0");
                    break;
            }
        }
    }
}
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition.MainMode.Complete;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition.MainMode.Enter;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Branch;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Companion;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Dungeon;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Lootbox;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.PopupWindow;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Product;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Skill;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Stuff;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.UI;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Type;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Quest;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Extension
{
    public static class ContentControlExtension
    {
        public static void DeserializeData(this ContentType type, string dataJson)
        {
            switch (type)
            {
                case ContentType.StuffSlot:
                    JsonConvertLog.DeserializeObject<StuffContentData>(dataJson);
                    break;
                case ContentType.CompanionSlot:
                    JsonConvertLog.DeserializeObject<CompanionContentData>(dataJson);
                    break;
                case ContentType.SkillSlot:
                    JsonConvertLog.DeserializeObject<SkillContentData>(dataJson);
                    break;
                case ContentType.Branch:
                    JsonConvertLog.DeserializeObject<BranchContentData>(dataJson);
                    break;
                case ContentType.Lootbox:
                    JsonConvertLog.DeserializeObject<LootboxContentData>(dataJson);
                    break;
                case ContentType.SubMode:
                    JsonConvertLog.DeserializeObject<SubModeData>(dataJson);
                    break;
                case ContentType.PopupWindow:
                    JsonConvertLog.DeserializeObject<PopupWindowContentData>(dataJson);
                    break;
                case ContentType.Product:
                    JsonConvertLog.DeserializeObject<ProductContentData>(dataJson);
                    break;
                default:
                    break;
            }
        }

        public static void DeserializeData(this UnlockType type, string dataJson)
        {
            switch (type)
            {
                case UnlockType.MainModeEnter:
                    JsonConvertLog.DeserializeObject<MainModeEnterConditionData>(dataJson);
                    break;
                case UnlockType.MainModeComplete:
                    JsonConvertLog.DeserializeObject<MainModeCompleteConditionData>(dataJson);
                    break;
                case UnlockType.QuestCollect:
                    JsonConvertLog.DeserializeObject<QuestCollectConditionData>(dataJson);
                    break;
            }
        }
    }
}
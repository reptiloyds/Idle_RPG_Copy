using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Quests.Extension;
using PleasantlyGames.RPG.Runtime.Core.Quests.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Sheet
{
    public class QuestSheet : Sheet<int, QuestRow>
    {
        [Preserve] public QuestSheet() { }
    }
    
    public class QuestRow : SheetRow<int>
    {
        [Preserve] public QuestType Type { get; private set; }
        [Preserve] public string DataJSON { get; private set; }
        [Preserve] public ResourceType RewardType { get; private set; }
        [Preserve] public int RewardAmount { get; private set; }
        
        [Preserve] public QuestRow() { }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);
            
            if (SheetExt.IsValidationNeeded) 
                Validate();
        }

        private void Validate()
        {
            Type.TryDeserialize(DataJSON);
        }
    }
}

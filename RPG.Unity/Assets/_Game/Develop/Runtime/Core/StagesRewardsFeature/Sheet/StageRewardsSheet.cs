using Cathei.BakingSheet;
using Cathei.BakingSheet.Internal;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.DebugUtilities;

namespace PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Sheet
{
    public class StageRewardsSheet : Sheet<int, StageRewardsSheet.Row>
    {
        [Preserve] public StageRewardsSheet() { }
        
        public class Row : SheetRowArray<int, Elem>
        {
            [Preserve] public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
            
                if (SheetExt.IsValidationNeeded) 
                    Validate();
            }

            private void Validate()
            {
                if (Id < 0)
                    Logger.LogError("Invalid stage rewards id");
            }
        }
        
        public class Elem : SheetRowElem
        {
            [Preserve] public int Level { get; private set; }
            [Preserve] public ResourceType RewardType { get; private set; }
            [Preserve] public int RewardAmount { get; private set; }

            [Preserve] public Elem() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                if (Level < 0)
                    Logger.LogError("Invalid stage rewards level");
            
                if (RewardType == ResourceType.None)
                    Logger.LogError("Invalid stage reward type");
            
                if (RewardAmount <= 0)
                    Logger.LogError("Invalid stage reward amount");
            }
        }
    }
}
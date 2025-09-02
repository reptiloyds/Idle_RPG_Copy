using Cathei.BakingSheet;
using Cathei.BakingSheet.Internal;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Extension;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet
{
    public class DailiesSheet : Sheet<string, DailyRow>
    {
        [Preserve]
        public DailiesSheet() { }
    }
    
    public class DailyRow : SheetRow<string>
    {
        [Preserve] public DailyType Type { get; private set; }
        [Preserve] public string DataJSON { get; private set; }
        [Preserve] public ResourceType RewardType { get; private set; }
        [Preserve] public int RewardAmount { get; private set; }
        [Preserve] public bool Bonus { get; private set; }
            
        [Preserve]
        public DailyRow() { }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (SheetExt.IsValidationNeeded)
                Validate();
        }

        private void Validate() => 
            Type.TryDeserialize(DataJSON);
    }
}
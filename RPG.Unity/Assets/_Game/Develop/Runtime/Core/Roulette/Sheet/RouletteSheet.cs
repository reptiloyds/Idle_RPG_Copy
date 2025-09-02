using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Extension;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.Sheet
{
    public class RouletteSheet : Sheet<RouletteType, RouletteSheet.Row> 
    {
        [Preserve] public RouletteSheet() { }
        
        public class Row : SheetRowArray<RouletteType, Elem>
        {
            [Preserve] public string AdditionalData { get; private set; }
            
            [Preserve] public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                Id.TryDeserialize(AdditionalData);
            }
        }
        
        public class Elem : SheetRowElem
        {
            [Preserve] public ResourceType ResourceType { get; private set; }
            [Preserve] public int Amount { get; private set; }
            [Preserve] public int Weight { get; private set; }
            
            [Preserve] public Elem() { }
        }
    }
}
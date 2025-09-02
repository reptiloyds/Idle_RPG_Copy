using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Extension;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Type;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet
{
    public class PeriodicRewardSheet : Sheet<PeriodicRewardVariant, PeriodicRewardSheet.Row>
    {
        [Preserve] public PeriodicRewardSheet() { }
        
        public class Row : SheetRowArray<PeriodicRewardVariant, Elem>
        {
            [Preserve] public string NameToken { get; private set; } 
            [Preserve] public int RewardPeriod { get; private set; }
            [Preserve] public int ResetProgressPeriod { get; private set; }
            [Preserve] public bool ResetOnComplete { get; private set; }
            
            [Preserve] public Row() { }
        }
        
        public class Elem : SheetRowElem
        {
            [Preserve] public PeriodicRewardType RewardType { get; private set; }
            [Preserve] public string RewardJSON { get; private set; }
            [Preserve] public string Image { get; private set; }
            [Preserve] public string HexColor { get; private set; }

            private Color _color;

            public Color Color
            {
                get
                {
                    if (_color == default) 
                        ColorUtility.TryParseHtmlString(HexColor, out _color);
                    return _color;
                }
            }

            [Preserve] public Elem() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded) 
                    Validate();
            }

            private void Validate()
            {
                RewardType.TryDeserialize(RewardJSON);
                if(!string.IsNullOrEmpty(Image))
                    SheetExt.CheckSprite(Asset.MainAtlas, Image);
            }
        }
    }
}
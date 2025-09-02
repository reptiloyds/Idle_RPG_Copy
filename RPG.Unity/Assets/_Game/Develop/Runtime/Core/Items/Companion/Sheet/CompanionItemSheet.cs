using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Items.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Stats.Extension;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Companion.Sheet
{
    public class CompanionItemSheet : Sheet<string, CompanionItemRow>
    {
        [Preserve] public CompanionItemSheet() { }
    }
    
    public class CompanionItemRow : ItemRow
    {
        [Preserve] public string UnitId { get; private set; }
        [Preserve] public string PresentStatsJSON { get; private set; }
        
        private List<UnitStatType> _presentStats;
        public List<UnitStatType> PresentStats => _presentStats ??= StatExtension.DeserializeStatList(PresentStatsJSON);
        
        [Preserve] public CompanionItemRow() { }

        protected override void Validate()
        {
            base.Validate();
            SheetExt.CheckAsset(UnitId);
            SheetExt.CheckSprite(Asset.CompanionAtlas, ImageName);
            _presentStats = StatExtension.DeserializeStatList(PresentStatsJSON);
        }
    }
}
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model
{
    public class CompanionItem : Item
    {
        public override ItemType Type => ItemType.Companion;
        
        public string UnitId { get; }
        public List<UnitStatType> PresentStats { get; }

        public CompanionItem(ITranslator translator, CompanionItemRow config, ItemData data, ItemConfiguration configuration, Sprite sprite, BaseValueFormula baseValueFormula) :
            base(translator, config, data, configuration, sprite, baseValueFormula)
        {
            UnitId = config.UnitId;
            PresentStats = config.PresentStats;
        }
    }
}
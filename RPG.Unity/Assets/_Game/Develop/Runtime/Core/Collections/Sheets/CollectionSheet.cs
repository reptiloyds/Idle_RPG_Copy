using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine.Device;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.Sheets
{
    public class CollectionSheet : Sheet<string, CollectionRow>
    {
        [Preserve] public CollectionSheet() { }
    }

    public class CollectionRow : SheetRow<string>
    {
        [Preserve] public string LocalizationToken { get; private set; }
        [Preserve] public int MaxLevel { get; private set; }
        [Preserve] public ItemType Type { get; private set; }
        [Preserve] public string Items { get; private set; }
        [Preserve] public UnitStatType EffectType { get; private set; }
        [Preserve] public StatModType EffectModType { get; private set; } 
        [Preserve] public FormulaType EnhanceFormulaType { get; private set; }
        [Preserve] public string EnhanceFormulaJSON { get; private set; }

        private HashSet<string> _itemsIds;
        public HashSet<string> ItemIds => _itemsIds ??= DeserializeItems();
        
        [Preserve] public CollectionRow() { }
        
        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (!Application.isPlaying) 
                Validate();
        }

        private void Validate()
        {
            EnhanceFormulaType.DeserializeFormula(EnhanceFormulaJSON);
            _itemsIds = DeserializeItems();
        }

        private HashSet<string> DeserializeItems() => 
            JsonConvertLog.DeserializeObject<HashSet<string>>(Items);
    }
}
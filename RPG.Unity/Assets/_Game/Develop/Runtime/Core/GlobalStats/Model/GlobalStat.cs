using System;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Sheets;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;

namespace PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model
{
    public class GlobalStat : BaseStat
    {
        public GlobalStatType Type { get; private set; }
        
        public event Action OnValueChanged;

        public GlobalStat(GlobalStatSheet.Row config)
        {
            Type = config.Id;
            BaseValue = config.Value;
        }

        public override void AddModifier(StatModifier mod)
        {
            base.AddModifier(mod);
            
            OnValueChanged?.Invoke();
        }

        public override bool RemoveModifier(StatModifier mod)
        {
            var result = base.RemoveModifier(mod);
            OnValueChanged?.Invoke();
            return result;
        }

        public override bool RemoveAllModifiersFromSource(object source)
        {
            var result = base.RemoveAllModifiersFromSource(source);
            OnValueChanged?.Invoke();
            return result;
        }
        
        public bool ReplaceModifier(StatModifier oldMod, StatModifier newMod)
        {
            var result = base.RemoveModifier(oldMod);
            AddModifier(newMod);
            return result;
        }
    }
}
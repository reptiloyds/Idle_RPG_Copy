using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model.Effects
{
    public class GlobalStatBlessingEffect : BlessingEffect
    {
        private readonly GlobalStat _stat;

        public GlobalStatBlessingEffect(StatModType statModType, BaseValueFormula valueFormula,
            GlobalStat stat, string statName, bool zeroOnFirstLevel, GroupOrder groupOrder)
            : base(statModType, valueFormula, statName, zeroOnFirstLevel, groupOrder) => 
            _stat = stat;

        public override void Enable()
        {
            base.Enable();
            
            _stat.AddModifier(StatModifier);
        }

        public override void Disable()
        {
            base.Disable();
            
            _stat.RemoveModifier(StatModifier);
        }

        protected override void UpdateModifier()
        {
            var oldModifier = StatModifier;
            base.UpdateModifier();
            if(IsEnabled)
                _stat.ReplaceModifier(oldModifier, StatModifier);
        }
    }
}
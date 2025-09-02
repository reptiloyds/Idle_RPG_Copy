using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model.Effects
{
    public class UnitStatBlessingEffect : BlessingEffect
    {
        private readonly UnitStat _stat;

        public UnitStatBlessingEffect(StatModType statModType, BaseValueFormula valueFormula,
            UnitStat stat, string statName, bool zeroOnFirstLevel, GroupOrder groupOrder) :
            base(statModType, valueFormula, statName, zeroOnFirstLevel, groupOrder) => 
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
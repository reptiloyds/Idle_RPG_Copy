using System;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock.Base;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock
{
    public class StatLevelUnlock : BaseUnlock
    {
        private readonly StatLevelUnlockData _data;
        private readonly UnitStat _targetStat;

        public override bool IsUnlocked => _targetStat.Level >= _data.Level;
        public override string Condition => String.Format(Translator.Translate(ConditionPrefix + StatUnlockType.StatLevel),
            Translator.Translate(_data.Type.ToString()), _data.Level);

        public StatLevelUnlock(UnitStat stat, ITranslator translator, UnitStat targetTargetStat, StatLevelUnlockData data) : base(stat, translator)
        {
            _targetStat = targetTargetStat;
            _data = data;
        }

        public override void Initialize()
        {
            base.Initialize();
            _targetStat.OnLevelUp += OnTargetStatLevelUp;
        }

        public override void Dispose()
        {
            base.Dispose();
            _targetStat.OnLevelUp -= OnTargetStatLevelUp;
        }

        private void OnTargetStatLevelUp()
        {
            if(IsUnlocked)
                TriggerOnUnlock();
        }
    }
}
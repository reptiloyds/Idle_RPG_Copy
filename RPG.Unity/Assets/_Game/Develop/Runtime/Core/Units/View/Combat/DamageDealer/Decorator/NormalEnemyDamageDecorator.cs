using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer.Decorator
{
    public sealed class NormalEnemyDamageDecorator : DamageDecorator
    {
        private UnitStat _normalEnemyDamage;

        private bool _isReady;
        
        public NormalEnemyDamageDecorator(UnitView unitView, IDamageProvider wrappedEntity) : base(unitView, wrappedEntity)
        {
            UnitView.SpawnEvent += UpdateStats;
        }

        private void UpdateStats()
        {
            ClearStats();
            
            _normalEnemyDamage = UnitView.GetStat(UnitStatType.NormalEnemyDamage);
            AppendStat(_normalEnemyDamage);
            UpdateReadyStatus();
        }

        protected override BigDouble.Runtime.BigDouble GetDamageInternal(UnitView targetUnitView)
        {
            var damage = WrappedEntity.GetDamage(targetUnitView);
            if (!IsReady) return damage;
            if (targetUnitView.Has(UnitSubType.Boss)) return damage;
            
            damage *= 1 + _normalEnemyDamage.Value / 100;
            
            return damage;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            UnitView.SpawnEvent -= UpdateStats;
            ClearStats();
        }
    }
}
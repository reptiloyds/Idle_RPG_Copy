using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer.Decorator
{
    public class PercentDamageDecorator : DamageDecorator
    {
        private UnitStat _damagePercent;
        
        public PercentDamageDecorator(UnitView unitView, IDamageProvider wrappedEntity) : base(unitView, wrappedEntity) => 
            UnitView.SpawnEvent += UpdateStats;

        private void UpdateStats()
        {
            ClearStats();
            
            _damagePercent = UnitView.GetStat(UnitStatType.DamagePercent);
            
            AppendStat(_damagePercent);
            UpdateReadyStatus();
        }

        protected override BigDouble.Runtime.BigDouble GetDamageInternal(UnitView targetUnitView)
        {
            var damage = WrappedEntity.GetDamage(targetUnitView);
            if (!IsReady) return damage;

            damage *= 1 + _damagePercent.Value;
            
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
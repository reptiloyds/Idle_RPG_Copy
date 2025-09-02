using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer.Decorator
{
    public sealed class AdvancedDamageDecorator : DamageDecorator
    {
        private UnitStat _advancedDamage;
        
        public AdvancedDamageDecorator(UnitView unitView, IDamageProvider wrappedEntity) : base(unitView, wrappedEntity)
        {
            UnitView.SpawnEvent += UpdateStats;
        }

        private void UpdateStats()
        {
            ClearStats();
            
            _advancedDamage = UnitView.GetStat(UnitStatType.AdvancedDamage);
            AppendStat(_advancedDamage);
            UpdateReadyStatus();
        }

        protected override BigDouble.Runtime.BigDouble GetDamageInternal(UnitView targetUnitView)
        {
            var damage = WrappedEntity.GetDamage(targetUnitView);
            if (!IsReady) return damage;

            damage *= 1 + _advancedDamage.Value / 100;
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
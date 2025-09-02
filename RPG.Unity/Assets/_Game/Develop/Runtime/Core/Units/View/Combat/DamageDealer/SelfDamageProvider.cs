using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer
{
    public class SelfDamageProvider : IDamageProvider
    {
        private readonly UnitView _unitView;
        private UnitStat _damage;
        private readonly Color _color = Color.white;
        private readonly int _priority = 0;

        public SelfDamageProvider(UnitView unitView)
        {
            _unitView = unitView;
            _unitView.SpawnEvent += UpdateStats;
        }

        private void UpdateStats() => 
            _damage = _unitView.GetStat(UnitStatType.Damage);

        public void Enable() { }

        public void Disable() { }

        public BigDouble.Runtime.BigDouble GetDamage(UnitView targetUnitView) => 
            _damage.Value;

        public (Color color, int priority) GetColor() => 
            (_color, _priority);
    }
}
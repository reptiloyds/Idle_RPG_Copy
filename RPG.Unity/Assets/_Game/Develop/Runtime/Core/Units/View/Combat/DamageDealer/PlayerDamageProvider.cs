using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer
{
    public class PlayerDamageProvider : IDamageProvider
    {
        private readonly UnitStatService _statService;
        private UnitStat _playerDamageStat;
        
        private readonly Color _color = Color.white;
        private readonly int _priority = 0;

        public PlayerDamageProvider(UnitStatService statService) => 
            _statService = statService;

        public void Enable() => 
            _playerDamageStat = _statService.GetPlayerStat(UnitStatType.Damage);

        public void Disable()
        {
        }

        public BigDouble.Runtime.BigDouble GetDamage(UnitView targetUnitView) => 
            _playerDamageStat.Value;

        public (Color color, int priority) GetColor() => 
            (_color, _priority);
    }
}
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer
{
    public interface IDamageProvider
    {
        void Enable();
        void Disable();
        
        BigDouble.Runtime.BigDouble GetDamage(UnitView targetUnitView);
        (Color color, int priority) GetColor();
    }
}
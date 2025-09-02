using PleasantlyGames.RPG.Runtime.Core.Projectile.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Projectile.Model.Impact
{
    public abstract class BaseImpact : MonoBehaviour
    {
        public virtual ProjectileImpactType Type { get; }
    }
}
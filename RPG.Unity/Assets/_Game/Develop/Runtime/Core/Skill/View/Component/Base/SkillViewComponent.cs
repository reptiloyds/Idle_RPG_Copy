using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.View.Component.Base
{
    [HideMonoScript]
    public abstract class SkillViewComponent : MonoBehaviour
    {
        public abstract void OnSpawn();
        public abstract void OnDespawn();

        public virtual float GetDespawnTime() => 
            0;
    }
}
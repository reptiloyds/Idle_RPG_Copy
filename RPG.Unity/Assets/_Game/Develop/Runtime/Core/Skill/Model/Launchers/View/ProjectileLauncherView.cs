using PleasantlyGames.RPG.Runtime.Audio.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class ProjectileLauncherView : MonoBehaviour
    {
        [SerializeField] private AudioLauncher _audioLauncher;
        
        public abstract void Focus(Vector3 position);
        public abstract Transform GetShootPoint();
        public virtual void PlayAnimation() => 
            _audioLauncher?.Play();
    }
}
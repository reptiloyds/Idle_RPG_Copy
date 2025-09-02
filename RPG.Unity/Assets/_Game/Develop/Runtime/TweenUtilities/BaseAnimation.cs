using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.TweenUtilities
{
    [HideMonoScript]
    public abstract class BaseAnimation : MonoBehaviour
    {
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _stopOnDisable = true;

        protected virtual void OnEnable()
        {
            if (_playOnEnable) 
                Play();
        }

        protected virtual void OnDisable()
        {
            if(_stopOnDisable)
                Stop();
        }

        [Button]
        public abstract void Play();
        [Button]
        public abstract void Stop();
    }
}
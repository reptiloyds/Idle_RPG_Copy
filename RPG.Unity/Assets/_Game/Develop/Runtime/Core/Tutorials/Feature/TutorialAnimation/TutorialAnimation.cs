using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.TutorialAnimation
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class TutorialAnimation : MonoBehaviour
    {
        private Action _callback;
        private Action<TutorialAnimation> _onComplete;
        
        public virtual void Play(Action callback, Action<TutorialAnimation> onComplete)
        {
            _callback = callback;
            _onComplete = onComplete;
        }

        public virtual void Stop()
        {
            _callback = null;
            _onComplete = null;
        }

        protected virtual void Complete()
        {
            _callback?.Invoke();
            _onComplete?.Invoke(this);
        }
    }
}
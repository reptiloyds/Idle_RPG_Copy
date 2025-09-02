using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UIImpact.Variants 
{
    [HideMonoScript, DisallowMultipleComponent]
    public abstract class BaseUIImpact : MonoBehaviour
    {
        [SerializeField, Min(0)] private float _invokeDelay;

        private float _nextInvokeTime;

        public void InvokePlay()
        {
            if (!CanInvoke()) return;
            Play();
            _nextInvokeTime = Time.time + _invokeDelay;
        }
        
        protected abstract void Play();

        private bool CanInvoke()
        {
            return Time.time >= _nextInvokeTime;
        }
    }
}
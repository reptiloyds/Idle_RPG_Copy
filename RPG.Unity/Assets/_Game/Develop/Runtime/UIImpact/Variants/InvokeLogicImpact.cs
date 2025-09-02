using UnityEngine;
using UnityEngine.Events;

namespace PleasantlyGames.RPG.Runtime.UIImpact.Variants
{
    public class InvokeLogicImpact : BaseUIImpact
    {
        [SerializeField] private UnityEvent _onPlay;
        
        protected override void Play()
        {
            _onPlay?.Invoke();
        }
    }
}

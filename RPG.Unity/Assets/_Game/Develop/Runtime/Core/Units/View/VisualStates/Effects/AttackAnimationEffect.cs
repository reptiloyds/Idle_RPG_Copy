using Animancer;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    public class AttackAnimationEffect : AnimationEffect
    {
        [SerializeField] private StringAsset _attackEvent;
        
        protected override AnimancerState Play()
        {
            var state = base.Play();
            state.Speed = GetSpeed();
            return state;
        }
        
        private float GetSpeed()
        {
            var clip = Clips[CurrentClipIndex];
            var eventTime = clip.Events[_attackEvent].normalizedTime * clip.MaximumDuration;
            if (CurrentUnitView.AttackSpeed.Delay.Value < eventTime)
                return eventTime / CurrentUnitView.AttackSpeed.Delay.Value;
            return 1;
        }
    }
}
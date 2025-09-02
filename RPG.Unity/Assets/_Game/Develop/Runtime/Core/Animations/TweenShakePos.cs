using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.TweenUtilities;
using PrimeTween;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Animations
{
    public class TweenShakePos : BaseAnimation
    {
        [SerializeField] private List<Transform> _targets;
        [SerializeField] private ShakeSettings _shakeSettings;
        [SerializeField] private bool _resetPositionOnStop = true;
        
        private readonly List<Tween> _tweens = new();
        
        public override void Play()
        {
            foreach (var target in _targets)
            {
                var tween = Tween.ShakeLocalPosition(target, _shakeSettings);
                _tweens.Add(tween);
            }
        }

        public override void Stop()
        {
            foreach (var tween in _tweens) 
                tween.Stop();
            _tweens.Clear();

            if (!_resetPositionOnStop) return;
            foreach (var target in _targets)
                target.localPosition = Vector3.zero;
        }
    }
}
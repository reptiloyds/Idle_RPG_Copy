using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.TweenUtilities;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Animations
{
    [DisallowMultipleComponent]
    public class TweenObjectRotation : BaseAnimation
    {
        [SerializeField, Required] private List<Transform> _targets;
        [SerializeField, Required] private RotationAxis _axis = RotationAxis.Y;
        [SerializeField] private TweenSettings<float> _tweenSettings;
        [SerializeField] private bool _resetRotationOnStop = true;

        private readonly List<Tween> _tweens = new();
        
        public bool IsPlaying { get; private set; }
        
        public override void Play()
        {
            IsPlaying = true;
            foreach (var target in _targets)
            {
                var tween = Tween.LocalEulerAngles(target,
                    Vector3.zero,
                    GetEndVector(),
                    _tweenSettings.settings.duration,
                    _tweenSettings.settings.ease,
                    _tweenSettings.settings.cycles,
                    _tweenSettings.settings.cycleMode);
                _tweens.Add(tween);
            }
        }

        private Vector3 GetEndVector()
        {
            return new Vector3()
            {
                x = _axis == RotationAxis.X ? _tweenSettings.endValue : 0,
                y = _axis == RotationAxis.Y ? _tweenSettings.endValue : 0,
                z = _axis == RotationAxis.Z ? _tweenSettings.endValue : 0
            };
        }
        
        public override void Stop()
        {
            IsPlaying = false;
            foreach (var tween in _tweens) 
                tween.Stop();
            _tweens.Clear();

            if (!_resetRotationOnStop) return;
            foreach (var target in _targets) 
                target.transform.rotation = Quaternion.identity;
        }
    }
}
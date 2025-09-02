using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations.Old
{
    [DisallowMultipleComponent, HideMonoScript]
    public sealed class GunAnimator : WeaponAnimator
    {
        [SerializeField, Required] private Transform _gun;
        [SerializeField, Required] private TweenSettings<float> _gunRecoil;
        [SerializeField, MinValue(0)] private float _defaultAttackDelay;

        private Sequence _attackSequence;

        public override void Play()
        {
            _attackSequence.Stop();
            _attackSequence = Sequence.Create();
            _attackSequence.Group(Tween.Delay(_defaultAttackDelay / AttackSpeedK, ExecuteOnShoot));
            _attackSequence.Group(Tween.LocalPositionZ(_gun, _gunRecoil.startValue, _gunRecoil.endValue, _gunRecoil.settings.duration / AttackSpeedK,
                _gunRecoil.settings.ease));
            _attackSequence.Group(Tween.LocalPositionZ(_gun, _gunRecoil.endValue, _gunRecoil.startValue, _gunRecoil.settings.duration / AttackSpeedK,
                _gunRecoil.settings.ease));
        }
    }
}
using PleasantlyGames.RPG.Runtime.TweenUtilities;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations.Old
{
    public class TankAnimationLauncher : AnimationLauncher
    {
        [SerializeField, FoldoutGroup("Attack"), Required] private Transform _turret;
        [SerializeField, FoldoutGroup("Attack"), Required] private Transform _gun;
        [SerializeField, FoldoutGroup("Attack"), Required] private TweenSettings<float> _turretRecoil;
        [SerializeField, FoldoutGroup("Attack"), Required] private TweenSettings<float> _gunRecoil;
        [SerializeField, FoldoutGroup("Attack"), MinValue(0)] private float _defaultAttackDelay;
        
        [SerializeField, FoldoutGroup("Movement")] private BaseAnimation _wheels;

        private Sequence _attackSequence;
        private float _attackSpeedK = 1;

        public override void MultiplyAttackSpeed(float k) => 
            _attackSpeedK = k;

        protected override void Idle()
        {
            _wheels.Stop();
        }

        protected override void Move()
        {
            _wheels.Play();
        }

        protected override void Attack()
        {
            _attackSequence.Stop();
            _attackSequence = Sequence.Create();
            _attackSequence.Group(Tween.Delay(_defaultAttackDelay / _attackSpeedK, InvokeAttackTrigger));
            _attackSequence.Group(Tween.LocalPositionZ(_turret, _turretRecoil.startValue, _turretRecoil.endValue, _turretRecoil.settings.duration / _attackSpeedK,
                _turretRecoil.settings.ease));
            _attackSequence.Group(Tween.LocalPositionZ(_gun, _gunRecoil.startValue, _gunRecoil.endValue, _gunRecoil.settings.duration / _attackSpeedK,
                _gunRecoil.settings.ease));
            _attackSequence.Chain(Tween.LocalPositionZ(_turret, _turretRecoil.endValue, _turretRecoil.startValue, _turretRecoil.settings.duration / _attackSpeedK,
                _turretRecoil.settings.ease));
            _attackSequence.Group(Tween.LocalPositionZ(_gun, _gunRecoil.endValue, _gunRecoil.startValue, _gunRecoil.settings.duration / _attackSpeedK,
                _gunRecoil.settings.ease));
        }

        protected override void StopAttack()
        {
        }
    }
}
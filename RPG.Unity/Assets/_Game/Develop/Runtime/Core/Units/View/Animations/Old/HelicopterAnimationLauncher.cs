using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Animations;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.Weapons;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations.Old
{
    public sealed class HelicopterAnimationLauncher : AnimationLauncher
    {
        [Serializable]
        private class HelicopterGun
        {
            public Transform View;
            public Transform ShootPoint;
        }
        
        [SerializeField, FoldoutGroup("Attack"), MinValue(0)] private float _defaultAttackDelay;
        [SerializeField, FoldoutGroup("Attack"), Required] private List<HelicopterGun> _guns;
        [SerializeField, FoldoutGroup("Attack"), Required, HideIf("@this._guns == null")] private TweenSettings<float> _gunRecoil;
        [SerializeField, FoldoutGroup("Attack"), Required, HideIf("@this._guns == null")] private RangeWeapon _rangeWeapon;
        [SerializeField, FoldoutGroup("Movement")] private List<TweenObjectRotation> _propellers;
        
        private int _gunIndex = 0;
        
        private Sequence _attackSequence;
        private float _attackSpeedK = 1;

        public override void MultiplyAttackSpeed(float k)
        {
            _attackSpeedK = k;
        }

        protected override void Idle()
        {
        }

        protected override void Move()
        {
            foreach (var propeller in _propellers)
            {
                if(propeller.IsPlaying) continue;
                propeller.Play();
            } 
        }

        protected override void Attack()
        {
            _attackSequence.Stop();
            _attackSequence = Sequence.Create();
            _attackSequence.Group(Tween.Delay(_defaultAttackDelay / _attackSpeedK, InvokeAttackTrigger));
            if (_guns.Count == 0) return;

            var gun = _guns[_gunIndex];
            _rangeWeapon.SetShootPoint(gun.ShootPoint);
            _gunIndex++;
            if (_gunIndex >= _guns.Count) _gunIndex = 0;
            
            _attackSequence.Group(Tween.LocalPositionZ(gun.View, _gunRecoil.startValue, _gunRecoil.endValue, _gunRecoil.settings.duration / _attackSpeedK,
                _gunRecoil.settings.ease));
            _attackSequence.Group(Tween.LocalPositionZ(gun.View, _gunRecoil.endValue, _gunRecoil.startValue, _gunRecoil.settings.duration / _attackSpeedK,
                _gunRecoil.settings.ease));
        }

        protected override void StopAttack()
        {
        }
    }
}

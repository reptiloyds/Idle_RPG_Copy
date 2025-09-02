using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Building;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.Weapons;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    public class ShootEffect : VisualEffect, IUpdateableBuildElement
    {
        [Serializable]
        public class ShootConfig
        {
            public Transform Turret;
            [Required] public Transform Gun;
            [Required] public Transform ShootPoint;
        }

        [SerializeField] private List<ShootConfig> _configs;
        [SerializeField] private RangeWeapon _rangeWeapon;
        [SerializeField, Required] private TweenSettings<float> _turretRecoil;
        [SerializeField, Required] private TweenSettings<float> _gunRecoil;
        [SerializeField, MinValue(0)] private float _defaultAttackDelay;
        
        private Sequence _attackSequence;
        private float _speedK = 1;
        private int _index;
        
        public void LogIfWrong(ref int errorCount)
        {
            if (_rangeWeapon != null) return;
            errorCount++;
            Logger.LogError($"No {nameof(RangeWeapon)} found on the unit", gameObject);
        }

        public void UpdateState(UnitView unitView)
        {
            if(_rangeWeapon == null)
                _rangeWeapon = unitView.GetComponentInChildren<RangeWeapon>();
        }

        public override void Activate(UnitView unitView)
        {
            base.Activate(unitView);

            _attackSequence.Stop();
            
            _speedK = GetSpeed();
            _attackSequence.Stop();
            _attackSequence = Sequence.Create();
            _attackSequence.Group(Tween.Delay(_defaultAttackDelay / _speedK, TriggerAttack));
            if (_configs.Count == 0) return;
            
            var config = _configs[_index];
            _rangeWeapon.SetShootPoint(config.ShootPoint);
            _index++;
            if (_index >= _configs.Count) _index = 0;

            if (config.Turret != null)
                _attackSequence.Group(Tween.LocalPositionZ(config.Turret, _turretRecoil.startValue, _turretRecoil.endValue, _turretRecoil.settings.duration / _speedK,
                    _turretRecoil.settings.ease));
            _attackSequence.Group(Tween.LocalPositionZ(config.Gun, _gunRecoil.startValue, _gunRecoil.endValue, _gunRecoil.settings.duration / _speedK,
                _gunRecoil.settings.ease));
            if (config.Turret != null)
                _attackSequence.Chain(Tween.LocalPositionZ(config.Turret, _turretRecoil.endValue, _turretRecoil.startValue, _turretRecoil.settings.duration / _speedK,
                    _turretRecoil.settings.ease));
            _attackSequence.Group(Tween.LocalPositionZ(config.Gun, _gunRecoil.endValue, _gunRecoil.startValue, _gunRecoil.settings.duration / _speedK,
                _gunRecoil.settings.ease));
        }
        
        private float GetSpeed()
        {
            var eventTime = _defaultAttackDelay;
            if (CurrentUnitView.AttackSpeed.Delay.Value < eventTime)
                return eventTime / CurrentUnitView.AttackSpeed.Delay.Value;
            return 1;
        }
        
        private void TriggerAttack() => 
            CurrentUnitView.Combat.TriggerAttack();
    }
}
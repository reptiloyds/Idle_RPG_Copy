using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.View
{
    public class GunProjectileLauncher : ProjectileLauncherView
    {
        [SerializeField, Required] private Transform _lookObject;
        [SerializeField, MinValue(0)] private float _rotationSpeed = 90; 
        [SerializeField, Required] private Transform _shootPoint;
        [SerializeField] private ParticleSystem _muzzle;
        [SerializeField, Required] private Transform _gun;
        [SerializeField, Required] private TweenSettings<float> _gunRecoil;
        
        private Sequence _sequence;
        private Tween _lookTween;

        private void OnEnable()
        {
            _lookObject.localRotation = Quaternion.identity;
        }

        public override void Focus(Vector3 position)
        {
            var direction = position - _lookObject.position;
            var flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
            var targetRotation = Quaternion.LookRotation(flatDirection);
            var angleY = Mathf.Abs(Mathf.DeltaAngle(_lookObject.eulerAngles.y, targetRotation.eulerAngles.y));
            
            var duration = angleY / _rotationSpeed;
            if (duration > 0)
            {
                _lookTween.Stop();
                _lookTween = Tween.Rotation(_lookObject, targetRotation, angleY / _rotationSpeed);
            }
            else
                _lookObject.forward = flatDirection;
        }

        public override Transform GetShootPoint() => 
            _shootPoint;

        public override void PlayAnimation()
        {
            base.PlayAnimation();
            _sequence = Sequence.Create();
            _sequence.Group(Tween.LocalPositionZ(_gun, _gunRecoil.startValue, _gunRecoil.endValue, _gunRecoil.settings.duration,
                _gunRecoil.settings.ease));
            _sequence.Group(Tween.LocalPositionZ(_gun, _gunRecoil.endValue, _gunRecoil.startValue, _gunRecoil.settings.duration,
                _gunRecoil.settings.ease));
            
            _muzzle.Play();
        }

        private void OnDisable()
        {
            _lookTween.Stop();
            _sequence.Stop();
        }
    }
}
using Cinemachine;
using PleasantlyGames.RPG.Runtime.CameraServices.Type;
using PrimeTween;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Defenition.Animation
{
    [CreateAssetMenu(menuName = "SO/Camera/RotationCameraAnimation", fileName = "RotationCameraAnimation")]
    public class RotationCameraAnimationData : CameraAnimationData
    {
        public CameraAnimationType OperationType;
        public Vector3 Rotation;
        
        public override CameraAnimationModel GetAnimationModel(CinemachineVirtualCamera camera) => 
            new RotationCameraAnimationModel(camera, this);
    }

    internal class RotationCameraAnimationModel : CameraAnimationModel
    {
        private Tween _tween;
        private readonly RotationCameraAnimationData _data;

        private Vector3 _originalRotation;

        public RotationCameraAnimationModel(CinemachineVirtualCamera camera, RotationCameraAnimationData data) : base(camera) => 
            _data = data;

        public override void Play(float delay = 0)
        {
            _tween.Stop();
            
            _originalRotation = _camera.transform.rotation.eulerAngles;
            Vector3 endRotation;
            
            switch (_data.OperationType)
            {
                case CameraAnimationType.Additional:
                    endRotation = _originalRotation + _data.Rotation;
                    break;
                case CameraAnimationType.Override:
                    endRotation = _data.Rotation;
                    break;
                default:
                    endRotation = Vector3.zero;
                    break;
            }
            
            _tween = Tween.EulerAngles(_camera.transform,
                _originalRotation,
                endRotation,
                _data.Duration,
                _data.Ease,
                startDelay: delay);
        }

        public override void Stop()
        {
            _tween.Stop();
            
            // if (duration < 0)
            //     duration = _data.Duration;
            //
            // var startRotation = _camera.transform.rotation.eulerAngles;
            // _tween = Tween.EulerAngles(_camera.transform,
            //      startRotation,
            //      _originalRotation,
            //      duration,
            //      _data.Ease);
        }
    }
}
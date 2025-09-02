using Cinemachine;
using PleasantlyGames.RPG.Runtime.CameraServices.Type;
using PrimeTween;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Defenition.Animation
{
    [CreateAssetMenu(menuName = "SO/Camera/ZoomCameraAnimation", fileName = "ZoomCameraAnimation")]
    public class ZoomCameraAnimationData : CameraAnimationData
    {
        public CameraAnimationType OperationType;
        public float Distance;
        
        public override CameraAnimationModel GetAnimationModel(CinemachineVirtualCamera camera) => 
            new ZoomCameraAnimationModel(camera, this);
    }
    
    internal class ZoomCameraAnimationModel : CameraAnimationModel
    {
        private readonly ZoomCameraAnimationData _data;
        private Tween _tween;
        private float _originalDistance;
        
        public ZoomCameraAnimationModel(CinemachineVirtualCamera camera, ZoomCameraAnimationData data) : base(camera) => 
            _data = data;

        public override void Play(float delay = 0)
        {
            _tween.Stop();
            
            _originalDistance = _transposer.m_CameraDistance;
            float distance;
            
            switch (_data.OperationType)
            {
                case CameraAnimationType.Additional:
                    distance = _transposer.m_CameraDistance + _data.Distance;
                    break;
                case CameraAnimationType.Override:
                    distance = _data.Distance;
                    break;
                default:
                    distance = 0;
                    break;
            }
            
            _tween = Tween.Custom(_transposer.m_CameraDistance,
                distance,
                _data.Duration,
                SetCameraDistance,
                _data.Ease,
                startDelay: delay);
        }

        public override void Stop()
        {
            _tween.Stop();
            
            // if (duration < 0)
            //     duration = _data.Duration;
            //
            // _tween = Tween.Custom(_transposer.m_CameraDistance,
            //     _originalDistance,
            //     duration,
            //     SetCameraDistance,
            //     _data.Ease);
        }
        
        private void SetCameraDistance(float value) => 
            _transposer.m_CameraDistance = value;
    }
}
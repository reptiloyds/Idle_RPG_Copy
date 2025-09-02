using System.Collections.Generic;
using Cinemachine;
using PleasantlyGames.RPG.Runtime.CameraServices.Contract;
using PleasantlyGames.RPG.Runtime.CameraServices.Defenition.Animation;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using CameraType = PleasantlyGames.RPG.Runtime.CameraServices.Type.CameraType;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Model
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CameraAnimator : MonoBehaviour, ICameraAnimator
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        private CinemachineFramingTransposer _transposer;

        private readonly List<CameraAnimationModel> _models = new();

        [Inject] private IVirtualCameraService _virtualCameraService;

        private void Awake() => 
            _transposer = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();

        void ICameraAnimator.Execute(List<CameraAnimationData> animations, bool playSequentially)
        {
            var currentCameraType = _virtualCameraService.GetCurrentCameraType();
            if(currentCameraType == CameraType.Animation) return;
            StopAnimation();
            
            var currentCamera = _virtualCameraService.GetCurrentCamera();
            
            _virtualCameraService.SwitchTo(CameraType.Animation, 0);
            
            _transposer.m_CameraDistance = currentCamera.GetCameraDistance();
            _transposer.m_TrackedObjectOffset = currentCamera.GetFollowOffset();
            _camera.m_Lens.FieldOfView = currentCamera.GetFOV();
            _camera.Follow = currentCamera.Instance.Follow;
            
            Tween.Delay(this, 0.01f, target => ExecuteAnimations(animations, playSequentially));
        }

        private void ExecuteAnimations(List<CameraAnimationData> animations, bool playSequentially)
        {
            var delay = 0f;
            foreach (var animationData in animations)
            {
                var animationModel = animationData.GetAnimationModel(_camera);
                animationModel.Play(delay);
                if (playSequentially)
                    delay += animationData.Duration;
                _models.Add(animationModel);
            } 
        }

        void ICameraAnimator.Revert(float duration)
        {
            StopAnimation();
            _virtualCameraService.SwitchTo(CameraType.Main, duration);
        }

        private void StopAnimation()
        {
            foreach (var model in _models) 
                model.Stop();
            
            _models.Clear();
        }
    }
}
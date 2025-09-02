using Cinemachine;
using PrimeTween;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Defenition.Animation
{
    public abstract class CameraAnimationData : ScriptableObject
    {
        public Ease Ease = Ease.InOutSine;
        [Min(0)] public float Duration = 0.5f;

        public abstract CameraAnimationModel GetAnimationModel(CinemachineVirtualCamera camera);
    }

    public abstract class CameraAnimationModel
    {
        protected CinemachineVirtualCamera _camera;
        protected CinemachineFramingTransposer _transposer;

        protected CameraAnimationModel(CinemachineVirtualCamera camera)
        {
            _camera = camera;
            _transposer = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        public abstract void Play(float delay = 0);
        public abstract void Stop();
    } 
}
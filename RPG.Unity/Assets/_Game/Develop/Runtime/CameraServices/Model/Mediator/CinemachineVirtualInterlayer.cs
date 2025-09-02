using Cinemachine;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Model.Mediator
{
    public class CinemachineVirtualInterlayer : CinemachineInterlayer
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        private CinemachineFramingTransposer _transposer;
        public override CinemachineVirtualCameraBase Instance => _camera;
        
        public override void Follow(Transform target)
        {
            _camera.m_Follow = target;
        }

        public override void LookAt(Transform target)
        {
            _camera.m_LookAt = target;
        }

        private void Reset() => 
            _camera = GetComponent<CinemachineVirtualCamera>();

        private void Awake() => 
            _transposer = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();

        public override float GetFOV() => 
            _camera.m_Lens.FieldOfView;

        public override float GetCameraDistance() => 
            _transposer.m_CameraDistance;

        public override Vector3 GetFollowOffset() => 
            _transposer.m_TrackedObjectOffset;

        public override void Zoom(float delta) => 
            _transposer.m_CameraDistance += delta;
    }
}
using Cinemachine;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Model.Mediator
{
    public class CinemachineFreeLookInterlayer : CinemachineInterlayer
    {
        [SerializeField] private CinemachineFreeLook _camera;
        
        public override CinemachineVirtualCameraBase Instance => _camera;

        private void Awake() => 
            _camera.m_Transitions.m_OnCameraLive.AddListener(OnCameraLive);

        private void OnDestroy() => 
            _camera.m_Transitions.m_OnCameraLive.RemoveListener(OnCameraLive);

        public override void Follow(Transform target)
        {
            _camera.m_Follow = target;
            ResetAxis();
        }

        private void OnCameraLive(ICinemachineCamera arg0, ICinemachineCamera cinemachineCamera) => 
            ResetAxis();

        private void ResetAxis()
        {
            if(_camera.Follow == null) return;

            var target = _camera.Follow;
            _camera.ForceCameraPosition(target.position, target.rotation);
            _camera.m_YAxis.Value = 0.5f;
            _camera.m_XAxis.Value = 0;
        }

        public override void LookAt(Transform target) => 
            _camera.m_LookAt = target;

        private void Reset() => 
            _camera = GetComponent<CinemachineFreeLook>();

        public override float GetFOV() => 
            _camera.m_Lens.FieldOfView;

        public override float GetCameraDistance() => 
            Vector3.Distance(_camera.m_Follow.position, _camera.transform.position);

        public override Vector3 GetFollowOffset() => 
            Vector3.zero;

        public override void Zoom(float delta)
        {
            for (var i = 0; i < _camera.m_Orbits.Length; i++) 
                _camera.m_Orbits[i].m_Radius += delta;   
        }
    }
}
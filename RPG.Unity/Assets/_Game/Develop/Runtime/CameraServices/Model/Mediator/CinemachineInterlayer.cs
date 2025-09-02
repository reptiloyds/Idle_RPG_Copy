using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Model.Mediator
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class CinemachineInterlayer : MonoBehaviour
    {
        public abstract CinemachineVirtualCameraBase Instance { get; }

        public abstract void Follow(Transform target);
        public abstract void LookAt(Transform target);
        public abstract float GetFOV();
        public abstract float GetCameraDistance();
        public abstract Vector3 GetFollowOffset();
        public abstract void Zoom(float delta);
    }
}
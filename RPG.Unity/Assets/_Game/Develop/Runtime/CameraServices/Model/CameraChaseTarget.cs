using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Model
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CameraChaseTarget : MonoBehaviour
    {
        [SerializeField] private bool _follow = true;
        [SerializeField, HideIf("@this._follow == false")]
        private Transform _followTarget;
        
        [SerializeField] private bool _lookAt;
        [SerializeField, HideIf("@this._lookAt == false")]
        private Transform _lookAtTarget;

        public bool Follow => _follow;
        public Transform FollowTarget => _followTarget;

        public bool LookAt => _lookAt;
        public Transform LookAtTarget => _lookAtTarget;
    }
}

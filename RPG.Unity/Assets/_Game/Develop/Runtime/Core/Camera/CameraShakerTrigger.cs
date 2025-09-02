using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Camera
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CameraShakerTrigger : MonoBehaviour
    {
        [SerializeField, MinValue(0)] private float _amplitude = 1f;
        [SerializeField, MinValue(0)] private float _time = 0.1f;
        [SerializeField] private bool _triggerOnEnable = true;
        
        [Inject] private CameraShaker _shaker;

        private void OnEnable()
        {
            if(_triggerOnEnable)
                TriggerShake();
        }

        public void TriggerShake() => 
            _shaker.Shake(_amplitude, _time);
    }
}
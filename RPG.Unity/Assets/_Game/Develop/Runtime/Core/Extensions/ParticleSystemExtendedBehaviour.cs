using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Extensions
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ParticleSystemExtendedBehaviour : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _cleanOnDisable = true;
        [SerializeField] private bool _stopOnDisable = true;

        private void Reset() => 
            _particleSystem = GetComponent<ParticleSystem>();

        private void OnEnable()
        {
            if(_playOnEnable)
                _particleSystem.Play();
        }

        private void OnDisable()
        {
            if(_cleanOnDisable)
                _particleSystem.Clear();
            if(_stopOnDisable)
                _particleSystem.Stop();
        }
    }
}
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension
{
    [HideMonoScript, DisallowMultipleComponent]
    public class LateComponentActivator : MonoBehaviour
    {
        [SerializeField] private List<Behaviour> _components;
        [SerializeField, Min(0)] private float _delay;

        private int _index;
        private bool _enableOnNextFrame;
        private float _enableTime;
        
        private void Awake()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].enabled = false;
            }
        }

        private void OnEnable()
        {
            _index = 0;
            _enableOnNextFrame = true;
            _enableTime = Time.time + _delay;
        }
        
        private void OnDisable()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].enabled = false;
            }
        }

        private void Update()
        {
            if (!_enableOnNextFrame) return;
            if (!(Time.time >= _enableTime)) return;
                
            _enableTime = Time.time + _delay;
            _components[_index].enabled = true;
            _index++;
            
            if (_index < _components.Count) return;
                
            _enableOnNextFrame = false;
        }
    }
}

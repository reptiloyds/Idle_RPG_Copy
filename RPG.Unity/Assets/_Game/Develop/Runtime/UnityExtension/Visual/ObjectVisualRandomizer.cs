using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Visual
{
    public class ObjectVisualRandomizer : VisualRandomizer
    {
        [FormerlySerializedAs("_visuals")] [SerializeField] private List<ObjectVisual> _visualGroups;

        private ObjectVisual _currentVisual;

        private void Awake()
        {
            foreach (var visual in _visualGroups)
            {
                visual.Disable();
            }
        }

        protected override void ChangeVisual()
        {
            _currentVisual?.Disable();
            _currentVisual = _visualGroups.GetRandomElement();
            _currentVisual.Enable();
        }
    }
    
    [Serializable]
    public class ObjectVisual
    {
        public List<GameObject> Visuals;

        public void Disable()
        {
            foreach (var visual in Visuals)
            {
                visual.Off();
            }
        }

        public void Enable()
        {
            foreach (var visual in Visuals)
            {
                visual.On();
            }
        }
    }
}

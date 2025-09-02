using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PlaceTool
{
    public class RandomRotator : MonoBehaviour
    {
        [SerializeField] private List<Transform> _transforms = new();
        [SerializeField] private float _appendYRotation;

        private void Reset()
        {
            foreach (var child in transform) 
                _transforms.Add(child as Transform);
        }

        [Button]
        public void RotateRandomElements()
        {
            var randomRange = new Vector2Int(0, 1);
            foreach (var transformElement in _transforms)
            {
                var random = randomRange.Random();
                if(random == 0) continue;
                transformElement.localRotation = Quaternion.Euler(new Vector3(transformElement.localRotation.eulerAngles.x, transformElement.localRotation.eulerAngles.y + _appendYRotation, transformElement.localRotation.eulerAngles.z));
            }
        }
    }
}

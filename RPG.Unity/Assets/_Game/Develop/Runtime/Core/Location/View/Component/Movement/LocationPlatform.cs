using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Location.View.Component.Movement
{
    [DisallowMultipleComponent, HideMonoScript]
    public class LocationPlatform : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private List<(Transform child, Transform originalParent)> _children = new();
        
        public void AddLocalPosition(Vector3 moveDelta) => 
            transform.localPosition += moveDelta;
        
        public void SetLocalPosition(Vector3 position) =>
            transform.localPosition = position;

        public void AppendChild(Transform childTransform)
        {
            _children.Add((childTransform, childTransform.parent));
            childTransform.SetParent(transform);
        }
        
        private void ClearChildren()
        {
            foreach (var tuple in _children)
            {
                if(tuple.child.parent != transform) continue;
                tuple.child.SetParent(tuple.originalParent);  
            } 
            _children.Clear();
        }

        public void Clear() => 
            ClearChildren();
    }
}
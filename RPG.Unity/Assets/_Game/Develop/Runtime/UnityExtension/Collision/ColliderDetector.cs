using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Collision
{
    [HideMonoScript, DisallowMultipleComponent]
    public class ColliderDetector : MonoBehaviour
    {
        [ShowInInspector, HideInEditorMode]
        private readonly HashSet<GameObject> _objects = new ();

        public UnityEvent OnFirstUnit;
        public UnityEvent OnZeroUnit;

        private void OnTriggerEnter(Collider other) =>
            Enter(other.gameObject);

        private void OnTriggerExit(Collider other) =>
            Exit(other.gameObject);
        
        protected virtual void Enter(GameObject obj)
        {
            _objects.Add(obj);
            if (_objects.Count == 1) 
                OnFirstUnit?.Invoke();
        }

        protected virtual void Exit(GameObject obj)
        {
            _objects.Remove(obj);
            if (_objects.Count == 0) 
                OnZeroUnit?.Invoke();
        }
    }
}
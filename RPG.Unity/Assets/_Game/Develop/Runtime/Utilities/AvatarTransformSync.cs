using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Utilities
{
    [DisallowMultipleComponent, HideMonoScript]
    public class AvatarTransformSync : MonoBehaviour
    {
        [System.Serializable]
        public class TransformData
        {
            public string name;
            public Vector3 position;
            public Quaternion rotation;

            public TransformData(string name, Vector3 position, Quaternion rotation)
            {
                this.name = name;
                this.position = position;
                this.rotation = rotation;
            }
        }

        [SerializeField] private List<TransformData> savedTransforms = new();
        [SerializeField] private bool _ignoreSpaceInNames = true;

   
        [Button]
        public void SaveTransforms()
        {
            savedTransforms.Clear();
            Append(transform);
            SaveChildTransforms(transform);
            Debug.Log($"Transform data saved");
        }

        private void SaveChildTransforms(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Append(child);
                if (child.childCount > 0) 
                    SaveChildTransforms(child);
            }
        }

        private void Append(Transform target)
        {
            var transformName = _ignoreSpaceInNames ? target.name.Replace(" ", "") : target.name;
            savedTransforms.Add(new TransformData(transformName, target.localPosition,
                target.localRotation));
        }

        [Button]
        public void ApplyTransforms()
        {
            Apply(transform);
            ApplyChildTransforms(transform);
            Debug.Log("Transform data applied");
        }

        private void ApplyChildTransforms(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Apply(child);
                if (child.childCount > 0) 
                    ApplyChildTransforms(child);
            }
        }

        private void Apply(Transform target)
        {
            var targetName = _ignoreSpaceInNames ? target.name.Replace(" ", "") : target.name;
            TransformData data = savedTransforms.Find(t => t.name == targetName);
            if (data != null)
            {
                target.localPosition = data.position;
                target.localRotation = data.rotation;
            }
            else
                Debug.LogError($"Can`t find target data by name {target.name}");
        }
    }
}
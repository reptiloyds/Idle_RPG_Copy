using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Visual
{
    public class MeshVisualRandomizer : VisualRandomizer
    {
        [SerializeField, Required] private MeshFilter _meshFilter;
        [SerializeField] private Mesh[] _meshes;
        
        protected override void ChangeVisual()
        {
            _meshFilter.mesh = _meshes.GetRandomElement();;
        }
    }
}
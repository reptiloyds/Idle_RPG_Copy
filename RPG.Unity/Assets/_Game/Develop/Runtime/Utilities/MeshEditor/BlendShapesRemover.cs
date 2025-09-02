#if  UNITY_EDITOR
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Utilities.MeshEditor
{
    public class BlendShapesRemover : MonoBehaviour
    {
        [SerializeField] private Mesh _mesh;
        [SerializeField] private string savePath = "Assets/";

        [Button]
        private void Execute()
        {
            var mesh = RemoveBlendShapes(_mesh);
            var path = $"{savePath}{mesh.name}.asset";
            SaveMesh(mesh, path);
        }
        
        private Mesh RemoveBlendShapes(Mesh originalMesh)
        {
            var newMesh = new Mesh
            {
                vertices = originalMesh.vertices,
                triangles = originalMesh.triangles,
                normals = originalMesh.normals,
                uv = originalMesh.uv,
                colors = originalMesh.colors,
                tangents = originalMesh.tangents,
                bindposes = originalMesh.bindposes, // Сохраняем кости
                boneWeights = originalMesh.boneWeights, // Сохраняем веса костей
                name = originalMesh.name + "_NoBlendShapes"
            };

            return newMesh;
        }
        
        private void SaveMesh(Mesh mesh, string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Debug.LogWarning("Directory does not exist");
                return;
                //Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Mesh сохранён по пути: {path}");
        }
    }
}
#endif
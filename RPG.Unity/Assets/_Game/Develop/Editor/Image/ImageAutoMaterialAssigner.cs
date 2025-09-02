using UnityEditor;
using UnityEngine;

namespace PleasantlyGames.IdleRPG.Editor.Image
{
    [InitializeOnLoad]
    public static class ImageAutoMaterialAssigner
    {
        static readonly Material MyDefaultMaterial;

        static ImageAutoMaterialAssigner()
        {
            MyDefaultMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Game/UI/_SharedMaterials/UI_Custom.mat");

            ObjectFactory.componentWasAdded += OnComponentAdded;
        }

        private static void OnComponentAdded(Component component)
        {
            if (component is UnityEngine.UI.Image image && MyDefaultMaterial != null)
            {
                image.material = MyDefaultMaterial;
                EditorUtility.SetDirty(image);
            }
        }
    }
}

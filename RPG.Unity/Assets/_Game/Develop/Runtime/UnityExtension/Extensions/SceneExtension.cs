using UnityEngine.SceneManagement;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Extensions
{
    public static class SceneExtension
    {
        public static void UpdateAllSceneHasher(this ref Scene scene)
        {
            var rootGameObjects = scene.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                var editorUpdatables = rootGameObject.GetComponentsInChildren<IEditorUpdatable>();
                foreach (var editorUpdatable in editorUpdatables)
                {
                    editorUpdatable.InvokeUpdate();
                }
            }
        }
    }
}
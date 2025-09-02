using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PleasantlyGames.IdleRPG.Editor
{
    [InitializeOnLoad]
    public static class Preloader
    {      
        const string playFromFirstMenuStr = "Edit/Always Start From Scene 0 &p";

        static bool playFromFirstScene
        {
            get{return EditorPrefs.HasKey(playFromFirstMenuStr) && EditorPrefs.GetBool(playFromFirstMenuStr);}
            set{EditorPrefs.SetBool(playFromFirstMenuStr, value);}
        }

        [MenuItem(playFromFirstMenuStr, false, 150)]
        static void PlayFromFirstSceneCheckMenu() 
        {
            playFromFirstScene = !playFromFirstScene;
            Menu.SetChecked(playFromFirstMenuStr, playFromFirstScene);

            ShowNotifyOrLog(playFromFirstScene ? "Play from scene 0" : "Play from current scene");
        }

        // The menu won't be gray out, we use this validate method for update check state
        [MenuItem(playFromFirstMenuStr, true)]
        static bool PlayFromFirstSceneCheckMenuValidate()
        {
            Menu.SetChecked(playFromFirstMenuStr, playFromFirstScene);
            return true;
        }

#if UNITY_EDITOR
        // his method is called before any Awake. It's the perfect callback for this feature
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void LoadFirstSceneAtGameBegins()
        {
            if (!playFromFirstScene)
                return;
        
            if (EditorBuildSettings.scenes.Length == 0)
            {
                Debug.LogWarning("The scene build list is empty. Can't play from first scene.");
                return;
            }
        
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects()) 
                    root.SetActive(false);
                
                SceneManager.LoadScene(0);   
            }
        }
#endif
        
        static void ShowNotifyOrLog(string msg)
        {
            if(Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
                EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent(msg));
            else
                Debug.Log(msg); // When there's no scene view opened, we just print a log
        }
    }
}
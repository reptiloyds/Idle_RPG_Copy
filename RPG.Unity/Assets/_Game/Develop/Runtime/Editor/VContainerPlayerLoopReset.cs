#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;

namespace PleasantlyGames.RPG.Runtime.Editor
{
    [InitializeOnLoad]
    public static class VContainerPlayerLoopReset
    {
        static VContainerPlayerLoopReset()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode) 
                PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
        }
    }
}
#endif
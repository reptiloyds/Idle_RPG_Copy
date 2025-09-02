using System.Diagnostics;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.DebugUtilities
{
    public static class Logger
    {
#if RPG_DEV && LOG_DISABLED
        [Conditional("DUMMY_UNUSED_DEFINE")]
#endif
        public static void Log(string message, GameObject gameObject = null)
        {
            if (gameObject == null)
                UnityEngine.Debug.Log(message);
            else
                UnityEngine.Debug.Log(message, gameObject);
        }

#if RPG_DEV && LOG_DISABLED
        [Conditional("DUMMY_UNUSED_DEFINE")]
#endif
        public static void LogError(string message, GameObject gameObject = null)
        {
            if (gameObject == null)
                UnityEngine.Debug.LogError(message);
            else
                UnityEngine.Debug.LogError(message, gameObject);
        }

#if RPG_DEV && LOG_DISABLED
        [Conditional("DUMMY_UNUSED_DEFINE")]
#endif
        public static void LogWarning(string message, GameObject gameObject = null)
        {
            if (gameObject == null)
                UnityEngine.Debug.LogWarning(message);
            else
                UnityEngine.Debug.LogWarning(message, gameObject);
        }
    }
}
using System.IO;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Utilities
{
    public class LogToFile : MonoBehaviour
    {
        private static string logFilePath;

        void Awake()
        {
            logFilePath = Path.Combine(Application.persistentDataPath, "logs.txt");
        
            Application.logMessageReceived += HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            string logEntry = $"[{type}] {logString}\n{stackTrace}\n";
            File.AppendAllText(logFilePath, logEntry);
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }
}
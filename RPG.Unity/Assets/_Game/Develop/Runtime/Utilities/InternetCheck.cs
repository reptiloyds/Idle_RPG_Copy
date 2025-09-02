using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace PleasantlyGames.RPG.Runtime.Utilities
{
    public class InternetCheck : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start() => 
            StartCoroutine(CheckInternet());

        IEnumerator CheckInternet()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                DebugUtilities.Logger.LogError("Нет подключения к интернету");
            else
                DebugUtilities.Logger.Log("Интернет есть");
            
            UnityWebRequest request = UnityWebRequest.Head("https://www.google.com");
            request.timeout = 5;

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                DebugUtilities.Logger.LogError("Интернет недоступен: " + request.error);
            else
                DebugUtilities.Logger.Log("Интернет доступен");
        }
    }

}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using _Game.Scripts.Systems.Server.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace _Game.Scripts.Systems.Server.Requesters
{
    public class ServerPostRequester
    {
        private readonly string _apiAddress;
        
        private readonly ConcurrentQueue<ServerRequestData> _requestQueue = new ();
        private bool _isProcessing = false;

        public ServerPostRequester(string apiAddress)
        {
            _apiAddress = apiAddress;
        }
        
        public void EnqueuePostRequest(ServerRequestData data)
        {
            _requestQueue.Enqueue(data);
            
            if (!_isProcessing)
            {
                ProcessQueue();
            }
        }

        private async void ProcessQueue()
        {
            _isProcessing = true;

            while (_requestQueue.TryDequeue(out ServerRequestData data))
            {
                await SendPostRequest(data);
            }

            _isProcessing = false;
        }

        private async Task SendPostRequest(ServerRequestData requestData)
        {
            string jsonData = requestData.Data;
            
            using UnityWebRequest request = new UnityWebRequest(_apiAddress + requestData.AdditionalUrlPath, "POST");
            
            if (requestData.HeaderData != null)
            {
                request.SetRequestHeader(requestData.HeaderData.HeaderName, requestData.HeaderData.HeaderData);
            }
            
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            var operation = request.SendWebRequest();

#if UNITY_EDITOR
            Debug.Log($"Send POST request: URI: [{request.uri.AbsoluteUri}]");
#endif

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Request POST error: URI: [{request.uri.AbsoluteUri}]\nError: {request.error}\nText: {request.downloadHandler.text}");
                var resultData = new ServerRequestResultData()
                {
                    ResultType = ServerRequestResult.Error,
                    Result = request.error,
                    Text = request.downloadHandler.text,
                };
                
                requestData.CallBack?.Invoke(resultData);
            }
            else
            {
                // Debug.Log("Response: " + request.downloadHandler.text);
                requestData.CallBack?.Invoke(new ServerRequestResultData()
                {
                    ResultType = ServerRequestResult.Success, 
                    Result = request.downloadHandler.text
                });
            }
        }
    }
}
using System.Collections.Concurrent;
using System.Threading.Tasks;
using _Game.Scripts.Systems.Server.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace _Game.Scripts.Systems.Server.Requesters
{
    public class ServerGetRequester
    {
        private readonly string _apiAddress;
        
        private readonly ConcurrentQueue<ServerRequestData> _requestQueue = new ();
        private bool _isProcessing = false;

        public ServerGetRequester(string apiAddress)
        {
            _apiAddress = apiAddress;
        }
        
        public void EnqueueGetRequest(ServerRequestData queryParams)
        {
            _requestQueue.Enqueue(queryParams);
            
            if (!_isProcessing)
            {
                ProcessQueue();
            }
        }

        private async void ProcessQueue()
        {
            _isProcessing = true;

            while (_requestQueue.TryDequeue(out ServerRequestData queryParams))
            {
                await SendGetRequest(queryParams);
            }

            _isProcessing = false;
        }

        private async Task SendGetRequest(ServerRequestData data)
        {
            using UnityWebRequest request = UnityWebRequest.Get(_apiAddress + data.Data);
            
            if (data.HeaderData != null)
            {
                request.SetRequestHeader(data.HeaderData.HeaderName, data.HeaderData.HeaderData);
            }
            
            var operation = request.SendWebRequest();

#if UNITY_EDITOR
            Debug.Log($"Send GET request: URI: [{request.uri.AbsoluteUri}]");
#endif

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Request GET error: URI: [{request.uri.AbsoluteUri}]\nError: {request.error}\nText: {request.downloadHandler.text}");
                data.CallBack?.Invoke(new ServerRequestResultData()
                {
                    ResultType = ServerRequestResult.Error, 
                    Result = request.error
                });
            }
            else
            {
                // Debug.Log("Response: " + request.downloadHandler.text);
                data.CallBack?.Invoke(new ServerRequestResultData()
                {
                    ResultType = ServerRequestResult.Success, 
                    Result = request.downloadHandler.text
                });
            }
        }
    }
}
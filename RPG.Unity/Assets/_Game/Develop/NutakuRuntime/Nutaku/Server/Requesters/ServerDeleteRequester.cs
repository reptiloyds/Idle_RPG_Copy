using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using _Game.Scripts.Systems.Server.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace _Game.Scripts.Systems.Server.Requesters
{
    public class ServerDeleteRequester
    {
        private readonly string _apiAddress;
        
        private readonly ConcurrentQueue<ServerRequestData> _requestQueue = new ();
        private bool _isProcessing = false;

        public ServerDeleteRequester(string apiAddress)
        {
            _apiAddress = apiAddress;
        }
        
        public void EnqueueDeleteRequest(ServerRequestData data)
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
                await SendDeleteRequest(data);
            }

            _isProcessing = false;
        }

        private async Task SendDeleteRequest(ServerRequestData requestData)
        {
            string jsonData = requestData.Data;
            
            using UnityWebRequest request = new UnityWebRequest(_apiAddress + requestData.Data, "DELETE");
            
            if (requestData.HeaderData != null)
            {
                request.SetRequestHeader(requestData.HeaderData.HeaderName, requestData.HeaderData.HeaderData);
            }
            
            request.downloadHandler = new DownloadHandlerBuffer();
            
            var operation = request.SendWebRequest();

#if UNITY_EDITOR
            Debug.Log($"Send DELETE request: URI: [{request.uri.AbsoluteUri}]");
#endif

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Request DELETE error: URI: [{request.uri.AbsoluteUri}]\nError: {request.error}\nText: {request.downloadHandler.text}");
                requestData.CallBack?.Invoke(new ServerRequestResultData()
                {
                    ResultType = ServerRequestResult.Error, 
                    Result = request.error
                });
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
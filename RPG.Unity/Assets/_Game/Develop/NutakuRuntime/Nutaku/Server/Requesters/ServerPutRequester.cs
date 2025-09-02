using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using _Game.Scripts.Systems.Server.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace _Game.Scripts.Systems.Server.Requesters
{
    public class ServerPutRequester
    {
        private readonly string _apiAddress;
        
        private readonly ConcurrentQueue<ServerRequestData> _requestQueue = new ();
        private bool _isProcessing = false;

        public ServerPutRequester(string apiAddress)
        {
            _apiAddress = apiAddress;
        }
        
        public void EnqueueRequest(ServerRequestData data)
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
                await SendRequest(data);
            }

            _isProcessing = false;
        }

        private async Task SendRequest(ServerRequestData requestData)
        {
            using UnityWebRequest request = new UnityWebRequest(_apiAddress + requestData.Data, "PUT");
            
            if (requestData.HeaderData != null)
            {
                request.SetRequestHeader(requestData.HeaderData.HeaderName, requestData.HeaderData.HeaderData);
            }
            
            if (!string.IsNullOrEmpty(requestData.Body))
            {
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestData.Body));
            }
            
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            var operation = request.SendWebRequest();

#if UNITY_EDITOR
            Debug.Log($"Send PUT request: URI: [{request.uri.AbsoluteUri}]");
#endif

            while (!operation.isDone)
            {
                await Task.Yield();
            }
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Request PUT error: URI: [{request.uri.AbsoluteUri}]\nError: {request.error}\nText: {request.downloadHandler.text}");
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
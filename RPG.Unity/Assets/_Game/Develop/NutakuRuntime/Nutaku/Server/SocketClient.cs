using System;
using NativeWebSocket;
using UnityEngine;

namespace _Game.Scripts.Systems.Server
{
    public class SocketClient
    {
        public event Action<string> OnMessageReceived;
        public event Action<string> OnConnectionWithServerProblem;

        private WebSocket _webSocket;

        private string _address;
        private string _accessToken;
        private bool _isReconnect = false;
        private bool _isConnected = false;
        private Action<bool> _reconnectCallBack;
        private Action _connectionCallBack;
        
        public async void Connect(string address, string accessToken, Action connectionCallBack = null)
        {
            CloseConnection();

            _connectionCallBack = connectionCallBack;
            _address = address.Replace("http", "ws").Replace("https", "wss");
            _accessToken = accessToken;
            
            _webSocket = new WebSocket(_address);
            
            _webSocket.OnOpen += OnWebSocketOpen;
            _webSocket.OnMessage += OnWebSocketMessage;
            _webSocket.OnError += OnWebSocketError;
            _webSocket.OnClose += OnWebSocketClosed;
            
            await _webSocket.Connect();
        }

        public void InitiateReconnect(Action<bool> callBack)
        {
            _isReconnect = true;
            _reconnectCallBack = callBack;
            Connect(_address, _accessToken, _connectionCallBack);
        }

        public void Tick(float deltaTime)
        {
            if (!_isConnected)
            {
                return;
            }
            
#if !UNITY_WEBGL || UNITY_EDITOR
            _webSocket.DispatchMessageQueue();
#endif
        }

        public void SendMessage(string message)
        {
            _webSocket.SendText(message);
        }
        
        private void OnWebSocketOpen()
        {
            Debug.Log("WebSocket соединение установлено!");
            if (_isReconnect)
            {
                _isReconnect = false;
                _reconnectCallBack?.Invoke(true);
            }
            
            _isConnected = true;
            _connectionCallBack?.Invoke();
        }

        private void OnWebSocketMessage(byte[] data)
        {
            var text = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log("Получено сообщение: " + text);
            OnMessageReceived?.Invoke(text);
        }

        private void OnWebSocketError(string errorMsg)
        {
            Debug.Log("Ошибка WebSocket: " + errorMsg);
            if (_isReconnect)
            {
                _isReconnect = false;
                _reconnectCallBack?.Invoke(true);
            }
            else
            {
                OnConnectionWithServerProblem?.Invoke(errorMsg); 
            }
        }

        private void OnWebSocketClosed(WebSocketCloseCode closeCode)
        {
            Debug.Log("WebSocket закрыт с кодом: " + closeCode + " и сообщением: ");
            if (_isReconnect)
            {
                _isReconnect = false;
                _reconnectCallBack?.Invoke(true);
            }
            else
            {
                OnConnectionWithServerProblem?.Invoke("ConnectionClosed"); 
            }
        }
        
        public async void CloseConnection()
        {
            if (_webSocket == null)
            {
                return;
            }
            
            _webSocket.OnOpen -= OnWebSocketOpen;
            _webSocket.OnMessage -= OnWebSocketMessage;
            _webSocket.OnError -= OnWebSocketError;
            _webSocket.OnClose -= OnWebSocketClosed;

            _isConnected = false;
            await _webSocket.Close();
            _webSocket = null;
        }
    }
}
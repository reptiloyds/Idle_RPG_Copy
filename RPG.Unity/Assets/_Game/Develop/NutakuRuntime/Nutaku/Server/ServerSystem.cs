// #undef UNITY_EDITOR
// #define UNITY_WEBGL
// #undef UNITY_ANDROID
using System;
using System.Collections.Generic;
using _Game.Scripts.Systems.Nutaku;
using _Game.Scripts.Systems.Server.Data;
using _Game.Scripts.Systems.Server.Data.Events;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PleasantlyGames.RPG.NutakuRuntime.TechnicalMessages;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using PleasantlyGames.RPG.Runtime.Utilities.ApplicationCloser.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.Model;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace _Game.Scripts.Systems.Server
{
    public class ServerSystem : IApplicationCloser, ITickable
    {
        [Inject] private NutakuSystem _nutakuSystem;
        [Inject] private ServerApi _serverApi;
        [Inject] private TechnicalMessageService _technicalMessageService;
        [Inject] private IPauseService _pauseService;
        
        public event Action OnAuthSuccess;
        public string ServerNumericId { get; private set; }
        
        private string _accessToken;
        private bool _isAuthFromAnotherDevice = false;
        
        private SocketClient _socketClient;
        private bool _socketClientInit = false;
        private const PauseType PauseTypes = PauseType.Time;
        public readonly bool IsActive = true;

        [Preserve]
        public ServerSystem()
        {
            
        }
        
        public void SetAuthData(string accessToken, string serverNumericId)
        {
            if (!IsActive)
                return;
            
            _accessToken = accessToken;
            ServerNumericId = serverNumericId;
            ServerApi.SetAccessToken(_accessToken);
            _socketClient = new SocketClient();
            
            InitSocketClient();
        }
        
        private void OnAuthFinished(ServerRequestResultData resultData)
        {
            // if (resultData.ResultType is ServerRequestResult.Error)
            // {
            //     try
            //     {
            //         var infoData = JsonConvert.DeserializeObject<Dictionary<string, object>>(resultData.Text);
            //         if (infoData.TryGetValue("code", out var code) && infoData.TryGetValue("error", out var sourceError))
            //         {
            //             var errorData = JsonConvert.DeserializeObject<Dictionary<string, string>>(sourceError.ToString());
            //             if (code as string == "user has been blocked" && errorData.TryGetValue("user_id", out string UID))
            //             {
            //                 ServerNumericId = UID;
            //                 _windowsSystem.OpenWindow<CriticalErrorWindow>("USER_BLOCKED_TITLE", "USER_BLOCKED_MSG", "USER_BLOCKED_ERROR_CODE");
            //                 return;
            //             }
            //         }
            //     }
            //     catch (Exception e)
            //     {
            //         Debug.LogError(e.Message);
            //     }
            //     
            //     _windowsSystem.OpenWindow<AppUpdateRequiredWindow>();
            //     Debug.Log($"Auth failed with ERROR: {resultData.Result}");
            //     return;
            // }
            //
            // // Debug.Log($"Access token obtained: [{resultData.Result}]");
            // var accessToken = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultData.Result);
            // if (accessToken == null || !accessToken.ContainsKey("access_token") || !accessToken.ContainsKey("user_numeric_id"))
            // {
            //     Debug.LogError("Failed to read access token");
            //     return;
            // }
            //
            // _accessToken = accessToken["access_token"];
            // ServerNumericId = accessToken["user_numeric_id"];
            // _nutakuSystem.RequestNickNameCallback(accessToken["nick"]);
            // ServerApi.SetAccessToken(_accessToken);
            // _socketClient = new SocketClient();
            //
            // InitSocketClient();
        }

        public void Close()
        {
            //_saveSystem.SetBlockOnQuitSaveLoad();
            
#if !UNITY_EDITOR && UNITY_ANDROID
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                .GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("finish");
            AndroidJavaObject pm = activity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);
            intent.Call<AndroidJavaObject>("addFlags", 0x20000000); // FLAG_ACTIVITY_CLEAR_TOP
            activity.Call("startActivity", intent);
            System.Diagnostics.Process.GetCurrentProcess().Kill(); // Завершение процесса
#elif !UNITY_EDITOR && UNITY_WEBGL
            Debug.Log("WebGl Reload page call");
            Application.ExternalCall("reloadPage");
#else
            EditorApplication.isPlaying = false;
#endif
        }

        #region SOCKET

        private void InitSocketClient()
        {
            _socketClient.OnMessageReceived += OnMessageReceived;
            _socketClient.OnConnectionWithServerProblem += OnServerConnectionProblem;
            _socketClient.Connect(ServerApi.API_ADDRESS + "/api/ws", _accessToken, AuthSocket);
            _socketClientInit = true;
        }

        private void AuthSocket()
        {
            var data = new Dictionary<string, object>
            {
                { "event", "auth" },
                { "payload", new Dictionary<string, string> { { "token", _accessToken } } }
            };
            var dataJson = JsonConvert.SerializeObject(data);
            _socketClient.SendMessage(dataJson);
        }
        
        private void OnMessageReceived(string msg)
        {
            var msgData = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg);
            if (!msgData.TryGetValue("event", out var value))
            {
                Debug.Log("msgData do not contains EVENT key");
                return;
            }
            
            switch (value)
            {
                case "Authorization complete":
                    OnAuthSuccess?.Invoke();
                    break;
                
                case "Authorization from another device":
                    _isAuthFromAnotherDevice = true;
                    _socketClient.CloseConnection();
                    _pauseService.Pause(PauseTypes);
                    _technicalMessageService.Open<AnotherDeviceAuthView>().Forget();
                    break;
            }
        }

        private void OnServerConnectionProblem(string info)
        {
            _pauseService.Pause(PauseTypes);
            
            Logger.Log($"Server connection problem with reason: [{info}]");
            _technicalMessageService.Open<RetryConnectionView>().Forget();
        }

        #endregion

        #region MAIN_SNAPSHOT

        public async UniTask<string> RequestCloudMainSaveJson()
        {
            var tcs = new UniTaskCompletionSource<string>();
            
            void RequestCallBack(ServerRequestResultData requestResultData)
            {
                if (requestResultData.ResultType is ServerRequestResult.Error)
                {
                    Logger.Log($"Json request failed with ERROR [{requestResultData.Result}]");
                    return;
                }

                tcs.TrySetResult(requestResultData.Result);
            }

            if (!IsActive)
            {
                tcs.TrySetResult("");
                return await tcs.Task;
            }
            
            ServerApi.RequestMainSnapShot(RequestCallBack);
            
            return await tcs.Task;
        }

        public async void LoadJsonToCloud(string json, bool forceLoad = false)
        {
            if (!IsActive && !forceLoad)
            {
                return;
            }
            
            await LoadToServer(json);
        }

        public async UniTask LoadMainSaveJsonToCloud(string json, bool forceLoad = false)
        {
            if (!IsActive && !forceLoad)
            {
                return;
            }
            
            async UniTask LoadToServer()
            {
                var tcs = new UniTaskCompletionSource<bool>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    tcs.TrySetResult(requestResultData.ResultType is ServerRequestResult.Success);
                }
            
                ServerApi.SendMainSnapShot(json, RequestCallBack);
            
                await tcs.Task;
            }
            
            await LoadToServer();
        }
        
        private async UniTask LoadToServer(string json)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            
            void RequestCallBack(ServerRequestResultData requestResultData)
            {
                tcs.TrySetResult(requestResultData.ResultType is ServerRequestResult.Success);
            }
            
            ServerApi.SendMainSnapShot(json, RequestCallBack);
            
            await tcs.Task;
        }

        #endregion

        #region EVENT_SNAPSHOT

        public async UniTask<string> RequestCloudEventSaveJson(string eventName)
        {
            var tcs = new UniTaskCompletionSource<string>();
            
            void RequestCallBack(ServerRequestResultData requestResultData)
            {
                if (requestResultData == null || requestResultData.ResultType is not ServerRequestResult.Success)
                {
                    tcs.TrySetResult("");
                }
                else
                {
                    tcs.TrySetResult(requestResultData.Result);
                }
            }

            if (!IsActive)
            {
                tcs.TrySetResult("");
                return await tcs.Task;
            }
            
            ServerApi.RequestEventSnapShot(RequestCallBack, eventName);
            
            return await tcs.Task;
        }

        public async void LoadEventSaveJsonToCloud(string json, string eventName, bool forceLoad = false)
        {
            if (!IsActive && !forceLoad)
            {
                return;
            }
            
            async UniTask LoadToServer()
            {
                var tcs = new UniTaskCompletionSource<bool>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    tcs.TrySetResult(requestResultData.ResultType is ServerRequestResult.Success);
                }
            
                ServerApi.SendEventSnapShot(json, eventName, RequestCallBack);
            
                await tcs.Task;
            }
            
            await LoadToServer();
        }

        #endregion

        #region EVENTS

        public async UniTask<ServerEventsData> GetEvents()
        {
            if (!IsActive)
                return new ServerEventsData();

            async UniTask<ServerEventsData> AwaitResult()
            {
                var tcs = new UniTaskCompletionSource<ServerEventsData>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    var resultData = new ServerEventsData();
                    
                    if (requestResultData.ResultType is ServerRequestResult.Success)
                    {
                        var json = requestResultData.Result;
                        var paramData = JsonConvert.DeserializeObject<ServerEventsData>(json);
                        
                        if (paramData != null)
                        {
                            resultData = paramData;
                        }
                    }
                    
                    tcs.TrySetResult(resultData);
                }
            
                ServerApi.RequestEvents(RequestCallBack);
            
                return await tcs.Task;
            }
            
            return await AwaitResult();
        }

        public async UniTask<ServerEnteredEventsData> GetEnteredEvents()
        {
            if (!IsActive)
            {
                return new ServerEnteredEventsData();
            }

            async UniTask<ServerEnteredEventsData> AwaitResult()
            {
                var tcs = new UniTaskCompletionSource<ServerEnteredEventsData>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    var resultData = new ServerEnteredEventsData();
                    
                    if (requestResultData.ResultType is ServerRequestResult.Success)
                    {
                        var json = requestResultData.Result;
                        var paramData = JsonConvert.DeserializeObject<ServerEnteredEventsData>(json);
                        
                        if (paramData != null)
                        {
                            resultData = paramData;
                        }
                    }
                    
                    tcs.TrySetResult(resultData);
                }
            
                ServerApi.RequestEnteredEvents(RequestCallBack);
            
                return await tcs.Task;
            }
            
            return await AwaitResult();
        }

        public async UniTask<ServerEventsRewardData> GetEventsRewards()
        {
            if (!IsActive)
            {
                return new ServerEventsRewardData();
            }

            async UniTask<ServerEventsRewardData> AwaitResult()
            {
                var tcs = new UniTaskCompletionSource<ServerEventsRewardData>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    var resultData = new ServerEventsRewardData();
                    
                    if (requestResultData.ResultType is ServerRequestResult.Success)
                    {
                        var json = requestResultData.Result;
                        var paramData = JsonConvert.DeserializeObject<ServerEventsRewardData>(json);
                        
                        if (paramData != null)
                        {
                            resultData = paramData;
                        }
                    }
                    
                    tcs.TrySetResult(resultData);
                }
            
                ServerApi.RequestEventsRewards(RequestCallBack);
            
                return await tcs.Task;
            }
            
            return await AwaitResult();
        }

        public void JoinToEvent(string eventName)
        {
            ServerApi.JoinToEvent(eventName);
        }

        public async UniTask ClaimEventRewards(string eventName, Action<bool> callBack)
        {
            if (!IsActive)
            {
                callBack?.Invoke(true);
                return;
            }

            async UniTask<bool> AwaitResult()
            {
                var tcs = new UniTaskCompletionSource<bool>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    var resultData = requestResultData.ResultType == ServerRequestResult.Success;

                    tcs.TrySetResult(resultData);
                }
            
                ServerApi.RequestClaimEventRewards(eventName, RequestCallBack);
            
                return await tcs.Task;
            }

            var result = await AwaitResult();
            
            callBack?.Invoke(result);
        }

        public void RequestIncrementEventMilestones(string eventName, byte milestones = 1)
        {
            ServerApi.IncrementEventMilestones(eventName, milestones);
        }

        public  async UniTask RequestIncrementEventPoints(string eventName, ulong points, Action<bool> callBack = null)
        {
            if (!IsActive)
            {
                callBack?.Invoke(true);
                return;
            }

            async UniTask<bool> AwaitResult()
            {
                var tcs = new UniTaskCompletionSource<bool>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    var resultData = requestResultData.ResultType == ServerRequestResult.Success;

                    tcs.TrySetResult(resultData);
                }
            
                ServerApi.IncrementEventPoints(eventName, points, RequestCallBack);
            
                return await tcs.Task;
            }

            var result = await AwaitResult();
            
            callBack?.Invoke(result);
        }

        public async UniTask<ServerLeaderboardData> GetLeaderboard(string eventName)
        {
            if (!IsActive)
            {
                return new ServerLeaderboardData();
            }

            async UniTask<ServerLeaderboardData> AwaitResult()
            {
                var tcs = new UniTaskCompletionSource<ServerLeaderboardData>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    var resultData = new ServerLeaderboardData();
                    
                    if (requestResultData.ResultType is ServerRequestResult.Success)
                    {
                        var json = requestResultData.Result;
                        var paramData = JsonConvert.DeserializeObject<ServerLeaderboardData>(json);
                        
                        if (paramData != null)
                        {
                            resultData = paramData;
                        }
                    }
                    
                    tcs.TrySetResult(resultData);
                }
            
                ServerApi.RequestLeaderboard(RequestCallBack, eventName);
            
                return await tcs.Task;
            }
            
            return await AwaitResult();
        }

        #endregion

        #region MAIL

        public async void RequestMailsList(Action<string> callBack, bool readFlag = false, bool claimFlag = false)
        {
            var tcs = new UniTaskCompletionSource<string>();
            
            void RequestCallBack(ServerRequestResultData requestResultData)
            {
                if (requestResultData.ResultType is ServerRequestResult.Error)
                {
                    Debug.Log($"Mails request failed with ERROR [{requestResultData.Result}]");
                    return;
                }

                tcs.TrySetResult(requestResultData.Result);
            }

            if (!IsActive)
            {
                callBack?.Invoke("");
                return;
            }
            
            ServerApi.RequestMailsList(RequestCallBack, readFlag, claimFlag);
            var result = await tcs.Task;
            
            callBack?.Invoke(result);
        }

        public void RequestSetMailReadFlag(int mailId)
        {
            ServerApi.RequestSetMailRead(mailId);
        }

        public async UniTask<bool> RequestMailRewardsObtain(int id)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            
            void RequestCallBack(ServerRequestResultData requestResultData)
            {
                if (requestResultData.ResultType is ServerRequestResult.Error)
                {
                    Debug.Log($"Mails request failed with ERROR [{requestResultData.Result}]");
                    tcs.TrySetResult(false);
                    return;
                }

                tcs.TrySetResult(true);
                
            }

            if (!IsActive)
            {
                tcs.TrySetResult(false);
                return await tcs.Task;
            }
            
            ServerApi.RequestSetMailRewardObtained(id, RequestCallBack);
            return await tcs.Task;
        }
        
        public void RequestMailDelete(int mailId)
        {
            ServerApi.RequestMailDelete(mailId);
        }
        
        #endregion

        #region REMOTE_PARAM

        public async UniTask<RemoteParamRequestResult> GetRemoteParamValue(string paramName)
        {
            if (!IsActive)
            {
                return new RemoteParamRequestResult()
                {
                    Result = ServerRequestResult.Error,
                    ParamName = paramName,
                };
            }
            
            async UniTask<RemoteParamRequestResult> AwaitResult()
            {
                var tcs = new UniTaskCompletionSource<RemoteParamRequestResult>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    var resultData = new RemoteParamRequestResult()
                    {
                        Result = ServerRequestResult.Error,
                    };
                    
                    if (requestResultData.ResultType is ServerRequestResult.Success)
                    {
                        string json = requestResultData.Result;
                        var paramData = JsonConvert.DeserializeObject<RemoteParamData>(json);
                        if (paramData != null)
                        {
                            resultData.Result = ServerRequestResult.Success;
                            resultData.ParamName = paramData.name;
                            resultData.Value = paramData.value;
                        }
                    }
                    
                    tcs.TrySetResult(resultData);
                }
            
                ServerApi.RequestRemoteParamValue(RequestCallBack, paramName);
            
                return await tcs.Task;
            }
            
            return await AwaitResult();
        }
        
        public async UniTask<RemoteParamRequestResult> ChangeRemoteParamValue(string paramName, double changeValue)
        {
            if (!IsActive)
            {
                return new RemoteParamRequestResult()
                {
                    Result = ServerRequestResult.Success,
                    ParamName = paramName,
                    Value = 0,
                };
            }
            
            async UniTask<RemoteParamRequestResult> AwaitResult()
            {
                var tcs = new UniTaskCompletionSource<RemoteParamRequestResult>();
            
                void RequestCallBack(ServerRequestResultData requestResultData)
                {
                    string json = requestResultData.Result;
                    var paramData = JsonConvert.DeserializeObject<RemoteParamData>(json);
                    
                    RemoteParamRequestResult result = new RemoteParamRequestResult
                    {
                        Result = requestResultData.ResultType,
                        ParamName = paramData.name,
                        Value = paramData.value,
                    };
                    
                    tcs.TrySetResult(result);
                }
            
                ServerApi.SendRemoteParamChange(RequestCallBack, paramName, changeValue);
            
                return await tcs.Task;
            }
            
            return await AwaitResult();
        }

        #endregion

        public void ChangeNickname(string nickname)
        {
            void RequestCallBack(ServerRequestResultData requestResultData)
            {
                if (requestResultData.ResultType is ServerRequestResult.Error)
                {
                    Debug.Log($"Change nickname request failed with ERROR [{requestResultData.Result}]");
                    return;
                }
                
                _nutakuSystem.RequestNickNameCallback(nickname);
            }
            
            ServerApi.RequestChangeNickname(nickname, RequestCallBack);
        }

        public void Tick()
        {
            if (!_socketClientInit)
            {
                return;
            }
            
            _socketClient.Tick(Time.deltaTime);   
        }
    }
}
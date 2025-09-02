using System;
using System.Collections.Generic;
using _Game.Scripts.Systems.Server.Data;
using _Game.Scripts.Systems.Server.Data.Events;
using _Game.Scripts.Systems.Server.Requesters;
using Newtonsoft.Json;
using PleasantlyGames.RPG.NutakuRuntime.Nutaku.Server.Exceptions;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Game.Scripts.Systems.Server
{
    public class ServerApi
    {
        public static string API_ADDRESS { get; private set; }

        private static ServerPostRequester _postRequester;
        private static ServerGetRequester _getRequester;
        private static ServerPatchRequester _patchRequester;
        private static ServerDeleteRequester _deleteRequester;
        private static ServerPutRequester _putRequester;
        private static string _accessToken;
        private static RequestHeaderData _header;

        [Preserve]
        public ServerApi(string serverUrl)
        {
            API_ADDRESS = serverUrl;
#if RPG_PROD
            //DownloadHelper.SetBaseStorageUrl("https://prodamazedgames.ams3.digitaloceanspaces.com/");
#elif RPG_DEV
            //DownloadHelper.SetBaseStorageUrl("https://dev.amazed.games/");
#endif

            _postRequester = new ServerPostRequester(API_ADDRESS);
            _getRequester = new ServerGetRequester(API_ADDRESS);
            _patchRequester = new ServerPatchRequester(API_ADDRESS);
            _deleteRequester = new ServerDeleteRequester(API_ADDRESS);
            _putRequester = new ServerPutRequester(API_ADDRESS);
        }

        public static void SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
            _header = new RequestHeaderData() { HeaderName = "Authorization", HeaderData = $"Bearer {_accessToken}" };
        }

        public static void GetServerTime(Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData("/api/utils/serverTime", headerData: _header, callBack: callBack);

            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void Auth(PostAuthData authData, Action<ServerRequestResultData> callBack)
        {
            if (authData == null)
            {
                Debug.LogError("Auth data can't be null!");
                return;
            }

            var json = JsonConvert.SerializeObject(authData);
            var requestData = new ServerRequestData(json, callBack, additionalUrlPath: "/api/auth/login");
            _postRequester.EnqueuePostRequest(requestData);
        }

        public static void SendVersionCheckRequest(string version, string platform,
            Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData($"/api/me/clients_compatible?platform={platform}&version={version}",
                callBack, headerData: _header);
            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void SendMainSnapShot(string jsonData, Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData(jsonData, callBack, additionalUrlPath: "/api/snapshot",
                headerData: _header);
            _postRequester.EnqueuePostRequest(requestData);
        }

        public static void RequestMainSnapShot(Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData($"/api/snapshot?limit=20&page=1", callBack, headerData: _header);
            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void RequestTemporaryOffers(Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData($"/api/temporary_contents?page=1&limit=100", callBack,
                headerData: _header);
            _getRequester.EnqueueGetRequest(requestData);
        }

        #region REMOTE_PARAM

        public static void RequestRemoteParamValue(Action<ServerRequestResultData> callBack, string remoteParam)
        {
            var requestData = new ServerRequestData($"/api/me/params/{remoteParam}", callBack, headerData: _header);
            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void SendRemoteParamChange(Action<ServerRequestResultData> callBack, string remoteParam,
            double value)
        {
            var data = new RemoteParamData()
            {
                name = remoteParam,
                value = (int)Math.Floor(value),
            };
            var jsonData = JsonConvert.SerializeObject(data);

            var requestData = new ServerRequestData(jsonData, callBack, additionalUrlPath: $"/api/me/params",
                headerData: _header);
            _postRequester.EnqueuePostRequest(requestData);
        }

        #endregion

        #region MAIL

        public static void RequestMailsList(Action<ServerRequestResultData> callBack, bool readFlag = false,
            bool claimFlag = false)
        {
            string endPointUrl = $"/api/emails";
            if (readFlag || claimFlag)
            {
                endPointUrl += "?";
                if (readFlag)
                {
                    endPointUrl += claimFlag ? "read=true&" : "read=true";
                }

                if (claimFlag)
                {
                    endPointUrl += "got_rewards=true";
                }
            }

            var requestData = new ServerRequestData(endPointUrl, callBack, headerData: _header);
            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void RequestSetMailRead(int mailId)
        {
            var requestData = new ServerRequestData($"/api/emails/{mailId}", null, headerData: _header);
            _patchRequester.EnqueuePatchRequest(requestData);
        }

        public static void RequestSetMailRewardObtained(int id, Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData($"/api/emails/{id}/rewards", callBack, headerData: _header);
            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void RequestMailDelete(int mailId)
        {
            var requestData = new ServerRequestData($"/api/emails/{mailId}", null, headerData: _header);
            _deleteRequester.EnqueueDeleteRequest(requestData);
        }

        #endregion

        public static void SendEventSnapShot(string jsonData, string eventName,
            Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData(jsonData, callBack,
                additionalUrlPath: $"/api/snapshot/event/{eventName}", headerData: _header);
            _postRequester.EnqueuePostRequest(requestData);
        }

        public static void RequestEventSnapShot(Action<ServerRequestResultData> callBack, string eventName)
        {
            var requestData = new ServerRequestData($"/api/snapshot/event/{eventName}", callBack, headerData: _header);
            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void RequestEvents(Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData($"/api/events", callBack)
                .SetHeader(_header);

            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void RequestEnteredEvents(Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData($"/api/events/entered", callBack)
                .SetHeader(_header);

            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void RequestEventsRewards(Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData($"/api/events/rewards", callBack)
                .SetHeader(_header);

            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void RequestClaimEventRewards(string eventName, Action<ServerRequestResultData> callBack)
        {
            var requestData = new ServerRequestData("", callBack, $"/api/events/{eventName}/rewards")
                .SetHeader(_header);

            _postRequester.EnqueuePostRequest(requestData);
        }

        public static void JoinToEvent(string eventName)
        {
            var requestData = new ServerRequestData("", null, $"/api/events/{eventName}/rooms")
                .SetHeader(_header);

            _postRequester.EnqueuePostRequest(requestData);
        }

        public static void IncrementEventMilestones(string eventName, byte milestones)
        {
            var data = new ServerEventIncrementMilestonesData
            {
                Milestones = milestones
            };
            var body = JsonConvert.SerializeObject(data);

            var requestData = new ServerRequestData($"/api/events/{eventName}/rooms/milestones", null)
                .SetHeader(_header)
                .SetBody(body);

            _patchRequester.EnqueuePatchRequest(requestData);
        }

        public static void IncrementEventPoints(string eventName, ulong points,
            Action<ServerRequestResultData> callBack)
        {
            var data = new ServerEventIncrementPointsData
            {
                Points = points
            };
            var body = JsonConvert.SerializeObject(data);

            var requestData = new ServerRequestData($"/api/events/{eventName}/rooms/points", callBack)
                .SetHeader(_header)
                .SetBody(body);

            _patchRequester.EnqueuePatchRequest(requestData);
        }

        public static void RequestLeaderboard(Action<ServerRequestResultData> callBack, string eventName)
        {
            var requestData = new ServerRequestData($"/api/events/{eventName}/rooms", callBack)
                .SetHeader(_header);

            _getRequester.EnqueueGetRequest(requestData);
        }

        public static void SendExceptionEvent(ExceptionData exceptionData)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("snapshot", new[] { exceptionData.DataJson });
            data.Add("text", exceptionData.Text);
            data.Add("type", exceptionData.Type.ToString());
            var json = JsonConvert.SerializeObject(data);

            var requestData = new ServerRequestData(json, additionalUrlPath: "/api/exceptions").SetHeader(_header);
            _postRequester.EnqueuePostRequest(requestData);
        }

        public static void RequestChangeNickname(string nickname, Action<ServerRequestResultData> callBack)
        {
            var data = new ServerEventChangeNicknameData
            {
                nick = nickname
            };

            var body = JsonConvert.SerializeObject(data);

            var requestData = new ServerRequestData($"/api/me", callBack)
                .SetHeader(_header)
                .SetBody(body);

            _putRequester.EnqueueRequest(requestData);
        }
    }
}
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.Networking;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.TimeUtilities.LoadUnits
{
    public class WebRequestTimeLoadUnit : ILoadUnit
    {
        [Inject] private TimeService _timeService;
        
        private const string TimeApi = "https://timeapi.io/api/Time/current/zone?timeZone=UTC";
        private const string Google = "https://www.google.com/generate_204";
        
        public string DescriptionToken => "TimeLoading";
        
        [Preserve]
        public WebRequestTimeLoadUnit()
        {
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            using var operation = UnityWebRequest.Get(TimeApi);
            operation.timeout = 2;
            try
            {
                await operation.SendWebRequest().WithCancellation(token);
                if (operation.result == UnityWebRequest.Result.Success) 
                    SetupServerTime(operation.downloadHandler.text);
                else
                {
                    Logger.LogWarning("Failed to fetch time: " + operation.error);
                    SetupLocalTime();
                }
            }
            catch (UnityWebRequestException requestException)
            {
                Logger.LogWarning("Request failed by timeout");
                SetupLocalTime();
            }
            
            progress?.Report(1f);
        }

        private void SetupServerTime(string json)
        {
            var utcTimeStr = JsonConvert.DeserializeObject<TimeData>(json).datetime;
            _timeService.SetupServerStartTime(DateTime.Parse(utcTimeStr));
        }

        private void SetupLocalTime() => 
            _timeService.SetupServerStartTime(DateTime.UtcNow);

        [Serializable]
        public class TimeData
        {
            public string datetime;
        }
    }
}
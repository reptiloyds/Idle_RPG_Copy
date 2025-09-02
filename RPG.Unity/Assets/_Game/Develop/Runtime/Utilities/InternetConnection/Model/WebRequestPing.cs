using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Model
{
    public class WebRequestPing
    {
        private readonly string _uri;

        public WebRequestPing(string uri) => 
            _uri = uri;

        public async UniTask<bool> Ping()
        {
            using var request = UnityWebRequest.Head(_uri);
            request.timeout = 2;
            try
            {
                await request.SendWebRequest();
                return request.result == UnityWebRequest.Result.Success;
            }
            catch(UnityWebRequestException requestException)
            {
                return false;
            }
        }
    }
}
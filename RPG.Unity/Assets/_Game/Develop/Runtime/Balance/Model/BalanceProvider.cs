using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Balance.Const;
using PleasantlyGames.RPG.Runtime.Balance.Contract;
using PleasantlyGames.RPG.Runtime.Balance.Definition;
using PleasantlyGames.RPG.Runtime.Balance.Type;
using UnityEngine.Networking;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Balance.Model
{
    public class BalanceProvider : IBalanceProvider
    {
        [Inject] private IBalanceLoader _balanceLoader;
        [Inject] private BalanceConfiguration _configuration;

        public string FileName => BalanceConst.DefaultFileName;
        public event Action OnFileNameChanged;
        
        [UnityEngine.Scripting.Preserve]
        public BalanceProvider()
        {
        }

        public async UniTask<bool> LoadAsync()
        {
            switch (_configuration.Source)
            {
                case BalanceSource.Local:
                    return await LoadLocalAsync();
                case BalanceSource.CDN:
#if UNITY_EDITOR
                    if (_configuration.UseCDNOnlyInBuild)
                        return await LoadLocalAsync();
                    return await LoadFromCDNAsync();
#else 
                    return await LoadFromCDNAsync();
#endif
                default:
                    return false;
            }
        }

        private async UniTask<bool> LoadLocalAsync()
        {
            await _balanceLoader.Load();
            return true;
        }

        private async UniTask<bool> LoadFromCDNAsync()
        {
            using UnityWebRequest request = UnityWebRequest.Get($"{_configuration.CDNRoot}{FileName}.json");
            await request.SendWebRequest().ToUniTask();


            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Logger.LogError("Error while downloading: " + request.error);
                return false;
            }

            var jsonText = request.downloadHandler.text;
            await _balanceLoader.Load(jsonText);
            return true;
        }
    }
}
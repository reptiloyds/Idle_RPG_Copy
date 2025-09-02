using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.PrivacyPolicy.LoadUnit
{
    public class PrivacyPolicyLoadUnit : ILoadUnit
    {
        private readonly PrivacyPolicyService _service;
        public string DescriptionToken => "PrivacyPolicy";

        [Preserve]
        public PrivacyPolicyLoadUnit(PrivacyPolicyService service) => 
            _service = service;

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            if(_service.IsViewed)
                progress?.Report(1);
            else
            {
                progress?.Report(0.5f);
                await _service.ShowPrivacyPolicyAsync();
                progress?.Report(1);
            }
        }
    }
}
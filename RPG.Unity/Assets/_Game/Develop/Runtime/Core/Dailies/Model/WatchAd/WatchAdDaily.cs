using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.WatchAd
{
    public class WatchAdDaily : Daily
    {
        private readonly IAdService _adService;
        private readonly DWatchAdData _configData;

        public WatchAdDaily(DailyRow config, DailyData data, ResourceService resourceService, ITranslator translator, IAdService adService) : base(config, data, resourceService, translator)
        {
            _adService = adService;
            _configData = JsonConvert.DeserializeObject<DWatchAdData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!IsComplete) 
                _adService.OnRewardClosed += OnRewardClosed;
        }

        public override void Dispose()
        {
            base.Dispose();
            _adService.OnRewardClosed -= OnRewardClosed;
        }

        private void OnRewardClosed(string adId, bool success)
        {
            if(!success) return;
            Progress++;
        }

        protected override int GetTargetValue() => 
            _configData.Amount;

        protected override void OnComplete()
        {
            _adService.OnRewardClosed -= OnRewardClosed;
            base.OnComplete();
        }
    }
}
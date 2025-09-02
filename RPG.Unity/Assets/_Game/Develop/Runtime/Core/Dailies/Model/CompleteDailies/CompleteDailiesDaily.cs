using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.CompleteDailies
{
    public class CompleteDailiesDaily : Daily
    {
        private readonly DailyService _dailyService;
        private readonly DCompleteDailiesData _configData;

        public CompleteDailiesDaily(DailyRow config, DailyData data, ResourceService resourceService, ITranslator translator, DailyService dailyService) : base(config, data, resourceService, translator)
        {
            _dailyService = dailyService;
            _configData = JsonConvert.DeserializeObject<DCompleteDailiesData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!IsComplete) 
                _dailyService.OnDailyCompleted += OnDailyCompleted;
        }

        public override void Dispose()
        {
            _dailyService.OnDailyCompleted -= OnDailyCompleted;
            base.Dispose();
        }

        private void OnDailyCompleted(Daily daily)
        {
            Progress++;
        }

        protected override int GetTargetValue() => 
            _configData.Amount;

        protected override void OnComplete()
        {
            _dailyService.OnDailyCompleted -= OnDailyCompleted;
            base.OnComplete();
        }
    }
}
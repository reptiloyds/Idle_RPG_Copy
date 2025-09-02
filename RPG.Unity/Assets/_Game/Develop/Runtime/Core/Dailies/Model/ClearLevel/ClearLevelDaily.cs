using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.ClearLevel
{
    public class ClearLevelDaily : Daily
    {
        private readonly MainMode _mainMode;

        private readonly DClearLevelData _configData;

        public ClearLevelDaily(DailyRow config, DailyData data, ResourceService resourceService, ITranslator translator, MainMode mainMode)
            : base(config, data, resourceService, translator)
        {
            _mainMode = mainMode;
            _configData = JsonConvert.DeserializeObject<DClearLevelData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!IsComplete) 
                _mainMode.OnLevelChanged += OnLevelChanged;
        }

        public override void Dispose()
        {
            base.Dispose();
            _mainMode.OnLevelChanged -= OnLevelChanged;
        }

        private void OnLevelChanged() => 
            Progress++;

        protected override int GetTargetValue() => 
            _configData.Amount;

        protected override void OnComplete()
        {
            _mainMode.OnLevelChanged -= OnLevelChanged;
            base.OnComplete();
        }
    }
}
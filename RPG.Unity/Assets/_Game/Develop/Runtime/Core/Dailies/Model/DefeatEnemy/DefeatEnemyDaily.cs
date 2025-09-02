using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.DefeatEnemy
{
    public class DefeatEnemyDaily : Daily
    {
        private readonly MainMode _mainMode;
        private readonly DDefeatEnemyData _configData;

        public DefeatEnemyDaily(DailyRow config, DailyData data, ResourceService resourceService, ITranslator translator, MainMode mainMode) : base(config, data, resourceService, translator)
        {
            _configData = JsonConvert.DeserializeObject<DDefeatEnemyData>(config.DataJSON);
            _mainMode = mainMode;
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!IsComplete) 
                _mainMode.OnEnemyDied += OnEnemyDied;
        }

        public override void Dispose()
        {
            base.Dispose();
            _mainMode.OnEnemyDied -= OnEnemyDied;
        }

        private void OnEnemyDied(UnitView unitView)
        {
            Progress++;
        }

        protected override int GetTargetValue() => 
            _configData.Amount;

        protected override void OnComplete()
        {
            _mainMode.OnEnemyDied -= OnEnemyDied;
            base.OnComplete();
        }
    }
}
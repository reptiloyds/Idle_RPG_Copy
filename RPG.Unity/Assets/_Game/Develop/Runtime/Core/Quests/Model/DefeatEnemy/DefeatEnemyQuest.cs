using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Quests.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.DefeatEnemy
{
    public class DefeatEnemyQuest : Quest
    {
        [Inject] private MainMode _mainMode;

        private readonly QDefeatEnemyData _data;
        
        public DefeatEnemyQuest(QuestRow config, int progress = 0) : base(config, progress)
        {
            _data = JsonConvert.DeserializeObject<QDefeatEnemyData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();
            _mainMode.OnEnemyDied += OnEnemyDied;
            if (Progress >= _data.Amount && !IsComplete) 
                Complete();
        }

        public override void Dispose()
        {
            _mainMode.OnEnemyDied -= OnEnemyDied;
            base.Dispose();
        }

        private void OnEnemyDied(UnitView enemy)
        {
            Progress++;
            if (Progress >= _data.Amount && !IsComplete) 
                Complete();
        }

        public override string GetDescription() =>
            Description;

        public override (float progress, string progressText) GetProgress() => 
            ((float)Progress / _data.Amount, $"{Progress.ToString()}/{_data.Amount.ToString()}");
    }
}
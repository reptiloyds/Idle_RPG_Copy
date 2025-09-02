using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Quests.Sheet;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.CompleteLevel
{
    public class CompleteLevelQuest : Quest
    {
        [Inject] private MainMode _mainMode;

        private readonly QCompleteLevelData _data;

        public CompleteLevelQuest(QuestRow config, int progress) : base(config, progress) => 
            _data = JsonConvert.DeserializeObject<QCompleteLevelData>(config.DataJSON);

        public override void Initialize()
        {
            base.Initialize();
            
            Description += $": {_mainMode.GetFormattedStageName(_data.Id)} {_mainMode.GetStageNumber(_data.Id)} - {_data.Level}";
            _mainMode.OnLevelChanged += OnLevelChanged;
            if(_mainMode.Id > _data.Id || _mainMode.Id == _data.Id && _mainMode.Level > _data.Level && !IsComplete)
                Complete();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _mainMode.OnLevelChanged -= OnLevelChanged;
        }

        private void OnLevelChanged()
        {
            if(_mainMode.Id > _data.Id || _mainMode.Id == _data.Id && _mainMode.Level > _data.Level && !IsComplete)
                Complete();
        }

        public override string GetDescription() => 
            Description;

        public override (float progress, string progressText) GetProgress()
        {
            var currentLevel = _mainMode.GetLevelsTo(_mainMode.Id, _mainMode.Level);
            var targetLevel = _mainMode.GetLevelsTo(_data.Id, _data.Level);
            return ((float)(currentLevel) / targetLevel, string.Empty);
        }
    }
}
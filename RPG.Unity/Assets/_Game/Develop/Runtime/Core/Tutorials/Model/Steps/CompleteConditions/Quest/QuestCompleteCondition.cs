using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Quest
{
    public class QuestCompleteCondition : TutorialCondition
    {
        private readonly QuestService _questService;
        private readonly QuestCollectConditionData _data;

        public QuestCompleteCondition(QuestService questService, string dataJson)
        {
            _questService = questService;
            _data = JsonConvert.DeserializeObject<QuestCollectConditionData>(dataJson);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            if(_questService.QuestId > _data.QuestId || _questService.QuestId == _data.QuestId && _questService.IsCompleteCurrent)
                Complete();
            else
                _questService.OnQuestComplete += OnQuestComplete;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _questService.OnQuestComplete -= OnQuestComplete;
        }

        public override void Pause() => 
            _questService.OnQuestComplete -= OnQuestComplete;

        public override void Continue()
        {
            if(_questService.QuestId > _data.QuestId || _questService.QuestId == _data.QuestId && _questService.IsCompleteCurrent)
                Complete();
            else
                _questService.OnQuestComplete += OnQuestComplete;
        }

        private void OnQuestComplete(int questId)
        {
            if(questId != _data.QuestId) return;
            Complete();
        }
    }
}
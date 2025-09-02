using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Quest
{
    internal class QuestCompleteTrigger : TutorialTrigger
    {
        private readonly QuestService _questService;
        private readonly QuestTutorialData _data;
        
        public QuestCompleteTrigger(QuestService questService, string dataJSON)
        {
            _questService = questService;
            _data = JsonConvert.DeserializeObject<QuestTutorialData>(dataJSON);
        }

        public override void Initialize()
        {
            _questService.OnQuestComplete += CheckQuest;

            if (_questService.IsCompleteCurrent)
            {
                if(_questService.QuestId >= _data.QuestId)
                    Execute();
            }
            else
            {
                if(_questService.QuestId - 1 >= _data.QuestId)
                    Execute();
            }
        }

        public override void Dispose() => 
            _questService.OnQuestComplete -= CheckQuest;

        private void CheckQuest(int questId)
        {
            if (questId != _data.QuestId) return;
            Execute();
        }
    }
}
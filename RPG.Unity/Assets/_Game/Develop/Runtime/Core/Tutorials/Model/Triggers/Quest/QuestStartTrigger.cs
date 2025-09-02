using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Quest
{
    internal class QuestStartTrigger : TutorialTrigger
    {
        private readonly QuestService _questService;
        private readonly QuestTutorialData _data;
        
        public QuestStartTrigger(QuestService questService, string dataJSON)
        {
            _questService = questService;
            _data = JsonConvert.DeserializeObject<QuestTutorialData>(dataJSON);
        }

        public override void Initialize()
        {
            _questService.OnQuestStart += CheckQuest;
            CheckQuest(_questService.QuestId);
        }

        public override void Dispose() => 
            _questService.OnQuestStart -= CheckQuest;

        private void CheckQuest(int questId)
        {
            if (questId < _data.QuestId) return;
            Execute();
        }
    }
}
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Quest
{
    internal class QuestCollectTrigger : TutorialTrigger
    {
        private readonly QuestService _questService;
        private readonly QuestTutorialData _data;

        public QuestCollectTrigger(QuestService questService, string dataJSON)
        {
            _questService = questService;
            _data = JsonConvert.DeserializeObject<QuestTutorialData>(dataJSON);
        }

        public override void Initialize()
        {
            _questService.OnQuestRewardCollected += CheckQuestReward;
        }

        public override void Dispose() =>
            _questService.OnQuestRewardCollected -= CheckQuestReward;

        private void CheckQuestReward(int questId)
        {
            if (questId != _data.QuestId) return;
            Execute();
        }
    }
}
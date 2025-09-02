using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Quest;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition.Quest
{
    public class QuestCollectCondition : UnlockCondition
    {
        private QuestCollectConditionData _data;
        private readonly QuestService _questService;

        public QuestCollectCondition(ContentSheet.Row config, ITranslator translator, QuestService questService) : base(config, translator) => 
            _questService = questService;

        public override string GetDescription()
        {
            var localizedDescription = Translator.Translate(LocalizationKey);
            return string.Format(localizedDescription, _data.QuestId);
        }

        public override void Initialize()
        {
            base.Initialize();

            _questService.OnQuestRewardCollected += OnQuestRewardCollected;
            Check();
        }

        protected override void Clear()
        {
            base.Clear();
            
            _questService.OnQuestRewardCollected -= OnQuestRewardCollected;
        }

        protected override void CreateData(string dataJson) => 
            _data = JsonConvertLog.DeserializeObject<QuestCollectConditionData>(dataJson);

        private void OnQuestRewardCollected(int id)
        {
            RedrawProgress();
            if(id == _data.QuestId)
                Complete();
        }

        private void Check()
        {
            RedrawProgress();
            if (IsConditionFulfilled())
                Complete();
        }

        private void RedrawProgress()
        {
            var rawProgress = (float) _questService.QuestId / _data.QuestId;
            Progress = Mathf.Min(rawProgress, 1);
        }

        private bool IsConditionFulfilled() => 
            _questService.QuestId > _data.QuestId;
    }
}
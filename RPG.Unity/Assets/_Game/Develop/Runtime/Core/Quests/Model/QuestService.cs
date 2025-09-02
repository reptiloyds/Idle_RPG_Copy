using System;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.BuyItem;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.CompleteDungeon;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.CompleteLevel;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.DefeatEnemy;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.EnhanceStat;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model.SpinRoulette;
using PleasantlyGames.RPG.Runtime.Core.Quests.Save;
using PleasantlyGames.RPG.Runtime.Core.Quests.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Quests.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model
{
    public class QuestService
    {
        [Inject] private BalanceContainer _balanceContainer;
        [Inject] private IObjectResolver _resolver;
        [Inject] private StatImprover _statImprover;
        [Inject] private LootboxService _lootboxService;
        [Inject] private DungeonModeFacade _dungeonModeFacade;
        [Inject] private ResourceService _resourceService;
        [Inject] private IAudioService _audioService;
        [Inject] private RouletteFacade _facade;
        
        private QuestSheet _sheet;
        private QuestDataContainer _data;
        private Quest _quest;

        public bool IsCompleteCurrent => _quest is { IsComplete: true };
        public int QuestId => _data.Id;
        public Quest Quest => _quest;

        public event Action<int> OnQuestStart; 
        public event Action<int> OnQuestComplete;
        public event Action<int> OnQuestRewardCollected;
        public event Action<int> OnQuestDisposed;
        public event Action OnProgressChanged;

        [Preserve]
        public QuestService() { }

        public void SetData(QuestDataContainer data) => 
            _data = data;

        public void Initialize()
        {
            _sheet = _balanceContainer.Get<QuestSheet>();
            CreateQuest();
        }

        public void ConfirmReward(Vector3 rewardPosition)
        {
            if (_quest.RewardType != ResourceType.None) 
                _resourceService.AddResource(_quest.RewardType, _quest.RewardAmount, ResourceFXRequest.Create(spawnPosition: rewardPosition));
            _audioService.CreateLocalSound(UI_Effect.UI_QuestComplete).Play();
            OnQuestRewardCollected?.Invoke(_quest.Id);
            
            NextQuest();
        }

        private void NextQuest()
        {
            DisposeQuest();
            _data.Id++;
            _data.Progress = 0;
            CreateQuest();
        }

        public bool IsComplete(int id)
        {
            if (_data.Id > id) return true;
            if (_data.Id < id) return false;

            return IsCompleteCurrent;
        }

        public void WarpTo(int questId)
        {
            if (questId > _sheet.Count)
                questId = _sheet.Count;
            
            if(questId <= _quest.Id) return;
            if(IsCompleteCurrent)
                OnQuestRewardCollected?.Invoke(_quest.Id);
            
            DisposeQuest();
            _data.Id++;
            _data.Progress = 0;
            for (int i = _data.Id; i < questId; i++)
            {
                _data.Id++;
                _data.Progress = 0;
            }
            
            CreateQuest();
            _quest.Complete();
        }

        private void CreateQuest()
        {
            if (!_sheet.Contains(_data.Id)) return;
            
            var questConfig = _sheet[_data.Id];

            switch (questConfig.Type)
            {
                case QuestType.DefeatEnemy:
                    _quest = new DefeatEnemyQuest(questConfig, _data.Progress);
                    break;
                case QuestType.BuyItem:
                    _quest = new BuyItemQuest(_lootboxService.PurchaseStatistic, questConfig, _data.Progress);
                    break;
                case QuestType.EnhanceStat:
                    _quest = new EnhanceStatQuest(_statImprover.ImproveStatistic, questConfig, _data.Progress);
                    break;
                case QuestType.CompleteLevel:
                    _quest = new CompleteLevelQuest(questConfig, _data.Progress);
                    break;
                case QuestType.CompleteDungeon:
                    _quest = new CompleteDungeonQuest(_dungeonModeFacade, questConfig, _data.Progress);
                    break;
                case QuestType.SpinRoulette:
                    _quest = new SpinRouletteQuest(questConfig, _data.Progress, _facade);
                    break;
            }
            
            _resolver.Inject(_quest);
            _quest.OnComplete += OnComplete;
            _quest.OnProgressChanged += OnQuestProgressChanged;
            _quest.Initialize();
            
            OnQuestStart?.Invoke(_quest.Id);
        }

        private void OnComplete() => 
            OnQuestComplete?.Invoke(_quest.Id);

        private void DisposeQuest()
        {
            var id = _quest.Id;
            _quest.OnComplete -= OnComplete;
            _quest.OnProgressChanged -= OnQuestProgressChanged;
            _quest.Dispose();
            _quest = null;
            
            OnQuestDisposed?.Invoke(id);
        }

        private void OnQuestProgressChanged()
        {
            _data.Progress = _quest.Progress;
            OnProgressChanged?.Invoke();
        }
    }
}
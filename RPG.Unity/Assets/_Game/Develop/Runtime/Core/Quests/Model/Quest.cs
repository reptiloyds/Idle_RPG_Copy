using System;
using PleasantlyGames.RPG.Runtime.Core.Quests.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Quests.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model
{
    public abstract class Quest : IDisposable
    {
        private readonly QuestRow _config;
        
        [Inject] protected ITranslator Translator;
        [Inject] protected ResourceService ResourceService;
        protected string Description;

        private int _progress;
        
        public int Progress
        {
            get => _progress;
            protected set
            {
                _progress = value;
                OnProgressChanged?.Invoke();
            }
        }

        public bool IsComplete { get; private set; }
        public int Id => _config.Id;
        public ResourceType RewardType => _config.RewardType;
        public int RewardAmount => _config.RewardAmount;
        public Sprite RewardSprite { get; private set; } 
        
        public event Action OnProgressChanged;
        public event Action OnComplete;

        protected Quest(QuestRow config, int progress)
        {
            _config = config;
            Progress = progress;
        }

        public virtual void Initialize()
        {
            Description = Translator.Translate($"Quest_{_config.Type}");
            RewardSprite = ResourceService.GetResource(RewardType)?.Sprite;
        }

        public virtual void Dispose()
        {
        }

        public abstract string GetDescription();
        
        public abstract (float progress, string progressText) GetProgress();

        public void Complete()
        {
            IsComplete = true;
            OnComplete?.Invoke();
        }
    }
}
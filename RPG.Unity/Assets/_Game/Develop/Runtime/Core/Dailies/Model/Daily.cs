using System;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model
{
    public abstract class Daily : IDisposable
    {
        protected readonly ITranslator Translator;
        protected readonly ResourceService ResourceService;

        protected readonly DailyRow Config;
        protected readonly DailyData Data;
        protected string Description;

        public ResourceType RewardType => Config.RewardType;
        public int RewardAmount => Config.RewardAmount;
        public DailyType Type => Config.Type;
        public bool IsBonus => Config.Bonus;
        public Sprite RewardSprite { get; private set; }
        public bool IsComplete => Progress >= TargetValue;
        public bool HasReward => IsComplete && !Data.Collected;

        public int Progress
        {
            get => Data.Progress;
            protected set
            {
                Data.Progress = value;
                OnProgressChanged?.Invoke();
                if(Data.Progress >= TargetValue)
                    OnComplete();
            }
        }
        
        public int TargetValue { get; private set; }

        public event Action<Daily> OnCompleted;
        public event Action OnRewardCollected;
        public event Action OnProgressChanged;

        protected Daily(DailyRow config, DailyData data,
            ResourceService resourceService, ITranslator translator)
        {
            Translator = translator;
            ResourceService = resourceService;
            Config = config;
            Data = data;
        }

        public virtual void Initialize()
        {
            Description = Translator.Translate($"Daily_{Type}");
            if (Config.Bonus) 
                Description = $"{Translator.Translate(TranslationConst.BonusDaily)} " + Description;
            RewardSprite = ResourceService.GetResource(RewardType)?.Sprite;
            TargetValue = GetTargetValue();
        }

        public virtual void Dispose()
        {
        }

        public void CollectReward()
        {
            Data.Collected = true;
            OnRewardCollected?.Invoke();
        }

        public string GetDescription()
        {
            var targetValue = GetTargetValue();
            var progress = Math.Clamp(Progress, 0, targetValue);
            return $"{Description} ({progress}/{targetValue})";
        }
        
        protected abstract int GetTargetValue();

        protected virtual void OnComplete() => 
            OnCompleted?.Invoke(this);

        public void ResetProgress()
        {
            Progress = 0;
            Data.Collected = false;
        }
    }
}
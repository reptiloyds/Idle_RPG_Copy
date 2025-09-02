using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Save;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using IUnlockable = PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract.IUnlockable;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Model
{
    public abstract class SubMode : IUnlockable
    {
        [Inject] protected ITranslator Translator;
        [Inject] protected BalanceContainer Balance;
        [Inject] protected TimeService Time;
        private IUnlockable _unlockable;
        private string _name;

        [Inject] protected ContentService ContentService;
        [Inject] protected ResourceService ResourceService;
        
        protected abstract SubModeSetup Setup { get; }
        protected abstract SubModeDataContainer SubModeData { get; }
        
        public bool IsUnlocked => _unlockable == null || _unlockable.IsUnlocked;
        public string Condition => _unlockable == null ? string.Empty : _unlockable.Condition;
        public event Action<IUnlockable> OnUnlocked;

        protected abstract string SubModeId { get; }
        
        public int BonusEnterAmount => SubModeData.BonusEnterAmount;
        public bool IsEnterResourceEnough => EnterResource.Value >= 1;
        public Color MainColor => Setup.Color;
        public int DailyEnterBonusAmount => Setup.DailyEnterBonusAmount;
        public int DailyEnterResourceAmount => Setup.DailyEnterResourceAmount;
        public AssetReferenceTexture2D BackgroundRef => Setup.BackgroundRef;
        public ResourceModel EnterResource { get; private set; }
        public abstract Sprite RewardImage { get; }
        
        public string Name
        {
            get => _name;
            protected set
            {
                _name = value;
                OnNameChanged?.Invoke();
            }
        }

        public event Action OnBonusEnterSpent;
        public event Action OnNameChanged;
        
        [Preserve]
        protected SubMode()
        {
        }

        public virtual void Initialize()
        {
            EnterResource = ResourceService.GetResource(Setup.EnterPriceType);
            _unlockable = ContentService.GetSubMode(SubModeId);
            if (!IsUnlocked) 
                _unlockable.OnUnlocked += OnUnlock;

            if (IsUnlocked)
            {
                if (Time.IsFirstSessionToday) 
                    ApplyDailyBonus();
                Time.OnNewDay += ApplyDailyBonus;
            } 
        }

        public virtual void Dispose()
        {
            if(IsUnlocked)
                Time.OnNewDay -= ApplyDailyBonus;
        }
        
        private void OnUnlock(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlock;
            Time.OnNewDay += ApplyDailyBonus;
            ApplyDailyBonus();
            OnUnlocked?.Invoke(this);
        }
        
        private void ApplyDailyBonus()
        {
            SubModeData.BonusEnterAmount = Setup.DailyEnterBonusAmount;
            ApplyDailyEnterResource();
        }
        
        protected virtual void ApplyDailyEnterResource() => 
            ResourceService.AddResource(EnterResource.Type, DailyEnterResourceAmount);

        public void SpendEnterResource() => 
            EnterResource.Spend(1);

        public void AddEnterResource() =>
            EnterResource.Add(1);

        public abstract UniTask Select();

        public void SpendBonusEnter()
        {
            SubModeData.BonusEnterAmount--;
            OnBonusEnterSpent?.Invoke();
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Ad.Definitions;
using PleasantlyGames.RPG.Runtime.Ad.Save;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Definition;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Ad.Model
{
    public abstract class BaseAdService : IAdService
    {
        [Inject] private IPauseService _pauseService;
        [Inject] private PauseConfiguration _pauseConfiguration;
        [Inject] private AdDataProvider _dataProvider;
        [Inject] private ISaveService _saveService;
        [Inject] private AdDefinition _adDefinition;
         
        protected string RewardId { get; private set; }
        
        private AdDataContainer _data;

        private ReactiveProperty<bool> _isDisabled;
        public ReadOnlyReactiveProperty<bool> IsDisabled => _isDisabled;

        public event Action<string, bool> OnRewardClosed;
        public event Action OnRewardRefreshed;

        [Preserve]
        protected BaseAdService()
        {
        }

        public virtual void Initialize()
        {
            _data = _dataProvider.GetData();
            if (_adDefinition.DisabledByDefault) 
                _data.IsDisabled = true;
            _isDisabled = new ReactiveProperty<bool>(_data.IsDisabled);
            _isDisabled.Subscribe(value => _data.IsDisabled = value);
        }

        public virtual void Dispose() { }

        public void Disable() => 
            _isDisabled.Value = true;

        #region Rewarded

        public virtual void ShowReward(string id) => 
            RewardId = id;

        public bool CanShowReward()
        {
            if(_isDisabled.Value)
                return string.IsNullOrEmpty(RewardId);
            
            return string.IsNullOrEmpty(RewardId) && IsRewardAdReady();
        }

        protected abstract bool IsRewardAdReady();

        protected virtual void RewardResult(bool success)
        {
            string rewardId = RewardId;
            RewardId = null;
            OnRewardClosed?.Invoke(rewardId, success);
            if (success)
            {
                _data.RewardAdViews++;
                _saveService.SaveAndLoadToCloudAsync();   
            }
        }

        protected virtual void RefreshReward() => 
            OnRewardRefreshed?.Invoke();

        #endregion
    }
}
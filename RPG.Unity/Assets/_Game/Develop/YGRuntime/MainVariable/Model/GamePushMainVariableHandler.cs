using System;
using GamePush;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.YGRuntime.Const;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.YGRuntime.MainVariable.Model
{
    public class GamePushMainVariableHandler : IInitializable, IDisposable, ILateTickable
    {
        [Inject] private IAdService _adService;
        [Inject] private IInAppProvider _purchaseProvider;
        [Inject] private TimeService _timeService;

        private bool _isInitialized;
        
        private int _inAppAmount;
        private int _rewardAdAmount;
        private readonly float _setPlaytimePeriod = 5;
        private float _setPlaytimeTimer;
        
        [UnityEngine.Scripting.Preserve]
        public GamePushMainVariableHandler() => 
            _setPlaytimeTimer = _setPlaytimePeriod;

        public void Initialize()
        {
            _adService.OnRewardClosed += OnAdRewardClosed;
            _purchaseProvider.OnPurchaseProcessed += OnPurchaseProcessed;

            _inAppAmount = GP_Player.GetInt(GamePushVariables.IAPAmount);
            _rewardAdAmount = GP_Player.GetInt(GamePushVariables.RewardAdAmount);
        }

        public void Dispose()
        {
            _adService.OnRewardClosed -= OnAdRewardClosed;
            _purchaseProvider.OnPurchaseProcessed -= OnPurchaseProcessed;
        }

        private void OnAdRewardClosed(string id, bool success)
        {
            if (!success) return;
            _rewardAdAmount++;
            GP_Player.Set(GamePushVariables.RewardAdAmount, _rewardAdAmount);
        }

        private void OnPurchaseProcessed(string id, bool success)
        {
            if (!success) return;
            _inAppAmount++;
            GP_Player.Set(GamePushVariables.IAPAmount, _inAppAmount);
        }

        public void LateTick()
        {
            if(!_timeService.IsInitialized) return;
            
            _setPlaytimeTimer -= Time.unscaledDeltaTime;
            if (_setPlaytimeTimer <= 0)
            {
                _setPlaytimeTimer = _setPlaytimePeriod;
                GP_Player.Set(GamePushVariables.Playtime, _timeService.TotalPlaytime);
            }
        }
    }
}
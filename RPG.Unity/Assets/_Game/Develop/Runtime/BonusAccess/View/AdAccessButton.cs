using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.BonusAccess.Type;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.SOValues;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.BonusAccess.View
{
    public partial class BonusAccessButton
    {
        [PropertyOrder(99)]
        [SerializeField, HideIf("@this._accessType != BonusAccessType.Ad")]
        private StringAsset _adId;
        [Inject] private IAdService _adService;

        private void InitializeAdAccess()
        {
            _adService.IsDisabled
                .Subscribe(_ => UpdateState())
                .AddTo(this);
            _adService.OnRewardRefreshed += OnAdRewardRefreshed;
        }

        private void DisposeAdAccess() => 
            _adService.OnRewardRefreshed -= OnAdRewardRefreshed;

        private void TryEnterAdState()
        {
            if (_adService.IsDisabled.CurrentValue)
                EnterState(BonusButtonState.FreeAccess);
            else
            {
                EnterState(BonusButtonState.Ad);
                SetInteractable(_adService.CanShowReward() && CanExecute());
            }
        }

        private void ClickOnAd()
        {
            if (!_adService.CanShowReward()) return;
            _adService.OnRewardClosed += OnRewardClosed;
            _adService.ShowReward(GetAdId());
        }

        private void OnRewardClosed(string adId, bool success)
        {
            _adService.OnRewardClosed -= OnRewardClosed;
            if (success) 
                Execute();
        }

        private void OnAdRewardRefreshed()
        {
            if(_state == BonusButtonState.Main)
                SetInteractable(_adService.CanShowReward());
        }

        private string GetAdId() => 
            _adId == null ? AdId.Undefined : _adId.Value;
    }
}
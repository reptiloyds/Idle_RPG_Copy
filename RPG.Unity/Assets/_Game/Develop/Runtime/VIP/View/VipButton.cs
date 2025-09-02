using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.VIP.Contract;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.VIP.View
{
    public class VipButton : BaseButton
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _inactiveSprite;
        [SerializeField] private Sprite _activeSprite;

        [Inject] private IWindowService _windowService;
        [Inject] private VipService _vipService;

        protected override void Awake()
        {
            base.Awake();
            _vipService.IsActive.Subscribe((value) => Redraw())
                .AddTo(this);
        }

        protected override async void Click()
        {
            base.Click();

            SetInteractable(false);
            await _windowService.OpenAsync<VipWindow>().ToAsyncLazy();
            SetInteractable(true);
        }

        private void Redraw() => 
            _image.sprite = _vipService.IsActive.CurrentValue ? _activeSprite : _inactiveSprite;
    }
}
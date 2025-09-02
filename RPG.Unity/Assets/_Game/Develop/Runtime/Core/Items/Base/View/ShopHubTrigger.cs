using PleasantlyGames.RPG.Runtime.Core.ShopHub.Type;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ShopHubTrigger : MonoBehaviour
    {
        [SerializeField, Required] private BaseButton _baseButton;
        [SerializeField, Required] private ShopElement _shopElement = ShopElement.Lootbox;

        [Inject] private IWindowService _windowService;

        private void Reset() => 
            _baseButton = GetComponent<BaseButton>();

        private void Awake() => 
            _baseButton.OnClick += OnClick;

        private async void OnClick()
        {
            _windowService.CloseAll();
            var window = await _windowService.OpenAsync<ShopHubWindow>();
            window.Select(_shopElement);
        }

        private void OnDestroy() => 
            _baseButton.OnClick -= OnClick;
    }
}
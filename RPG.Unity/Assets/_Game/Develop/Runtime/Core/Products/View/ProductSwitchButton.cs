using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    public class ProductSwitchButton : MonoBehaviour
    {
        [SerializeField] private BaseButton _button;
        [SerializeField] private ProductView _productView;
        
        [Inject] private IWindowService _windowService;
        
        private void Reset()
        {
            _button = GetComponentInChildren<BaseButton>();
            _productView = GetComponentInParent<MergedProductView>();
        }

        private void Awake() => 
            _button.OnClick += OnButtonClicked;

        private void OnDestroy() => 
            _button.OnClick -= OnButtonClicked;

        private async void OnButtonClicked()
        {
            var window = await _windowService.OpenAsync<ProductSwitchWindow>();
            window.Setup(_productView);
        }
    }
}
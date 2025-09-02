using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class ProductView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _background;
        [SerializeField] private PurchaseProductButton _purchaseButton;
        [SerializeField] private BadgeView _badge;
        [SerializeField] private RectTransform _rect;
        
        [Inject] protected ProductService Service;
        
        protected Image Background => _background;
        [ShowInInspector, HideInEditorMode, ReadOnly]
        public Product Product { get; protected set; }
        public BaseButton Button => _purchaseButton.Button;
        public RectTransform Rect => _rect;

        protected virtual void Awake() =>
            _purchaseButton.OnClick += OnPurchaseButtonClick;

        protected virtual void OnDestroy()
        {
            _purchaseButton.OnClick -= OnPurchaseButtonClick;
            ClearProduct();
        }

        public virtual void Setup(Product product)
        {
            ClearProduct();
            Product = product;
            if (Product.Visual.VisualData.BadgeEnabled)
                _badge.Show(Product.Visual.VisualData);
            else
                _badge.Hide();
            
            _purchaseButton.Setup(Product);
            Product.Access.OnLimitIsOver += OnLimitIsOver;
            if (!Product.Access.IsUnlocked) 
                Product.Access.OnUnlocked += OnProductUnlocked;
            
            Redraw();
        }

        private void Redraw()
        {
            gameObject.SetActive(Product.Access.IsUnlocked);
            if(!Product.Access.IsUnlocked) return;
            RedrawVisual();
        }

        protected virtual void RedrawVisual()
        {
            if (Product.Visual.VisualData.HideWhenOver) 
                gameObject.SetActive(!Product.Access.LimitIsOver);
            _purchaseButton.Redraw();
            if(_nameText != null)
                _nameText.SetText(Product.GetName());
        }

        protected void ClearProduct()
        {
            if (Product == null) return;
            Product.Access.OnLimitIsOver -= OnLimitIsOver;
            if (!Product.Access.IsUnlocked) 
                Product.Access.OnUnlocked -= OnProductUnlocked;
            Product = null;
            _purchaseButton.ClearProduct();
        }
        
        protected virtual void OnLimitIsOver() => 
            _purchaseButton.Redraw();

        protected string GetFormattedRewardName(ProductReward reward)
        {
            switch (reward.Type)
            {
                case ProductItemType.Resource:
                    return $"X{reward.Name}";
                default:
                    return reward.Name;
            }
        }

        private void OnProductUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnProductUnlocked;
            Redraw();
        }

        private async void OnPurchaseButtonClick()
        {
            var purchaseSuccess = await Service.Purchase(Product.Id);
            if (!purchaseSuccess) return;
            Redraw();
        }
    }
}
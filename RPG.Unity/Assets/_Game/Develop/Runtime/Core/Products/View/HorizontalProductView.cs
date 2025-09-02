using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.PurchasePresentation;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    public class HorizontalProductView : ProductView
    {
        [SerializeField] private PurchaseContentPresenter _contentPresenter;
        [SerializeField] private PurchaseItemPresenter _gridItemPresenter;
        [SerializeField] private PurchaseItemPresenter _horizontalItemPresenter;

        private HorizontalVisualData _data;
        
        public override void Setup(Product product)
        {
            _data = null;
            if (product != null) 
                _data = product.Visual.HorizontalData;
            base.Setup(product);
        }

        protected override void RedrawVisual()
        {
            base.RedrawVisual();
            
            Background.sprite = _data.BackSprite;
            Background.color = _data.BackColor;

            switch (_data.ItemPresent)
            {
                case PurchaseItemPresentType.None:
                    _gridItemPresenter.gameObject.SetActive(false);
                    _horizontalItemPresenter.gameObject.SetActive(false);
                    break;
                case PurchaseItemPresentType.Grid:
                    _gridItemPresenter.gameObject.SetActive(true);
                    _horizontalItemPresenter.gameObject.SetActive(false);
                    _gridItemPresenter.Clear();
                    foreach (var reward in Product.Rewards.List) 
                        _gridItemPresenter.Append(reward.Sprite, reward.BackColor,
                            GetFormattedRewardName(reward), reward.GetCertainProductType());
                    break;
                case PurchaseItemPresentType.Horizontal:
                    _horizontalItemPresenter.gameObject.SetActive(true);
                    _gridItemPresenter.gameObject.SetActive(false);
                    _horizontalItemPresenter.Clear();
                    foreach (var reward in Product.Rewards.List) 
                        _horizontalItemPresenter.Append(reward.Sprite, reward.BackColor,
                        GetFormattedRewardName(reward), reward.GetCertainProductType());
                    break;
            }
            
            if (_data.ContentSprites.Count == 0)
                _contentPresenter.gameObject.SetActive(false);
            else
            {
                _contentPresenter.gameObject.SetActive(true);
                _contentPresenter.Redraw(_data.ContentSprites, _data.ContentColor, _data.ContentBackground, _data.ContentLabelText);
            }
        }
    }
}
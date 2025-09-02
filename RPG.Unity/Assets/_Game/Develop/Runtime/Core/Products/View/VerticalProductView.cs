using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    public class VerticalProductView : ProductView
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private BonusRewardView _bonusRewardView;
        [SerializeField] private GameObject _bonusLabelContainer;
        [SerializeField] private UITimer _timer;
        
        private VerticalVisualData _data;
        
        public override void Setup(Product product)
        {
            _data = null;
            if (product != null) 
                _data = product.Visual.VerticalData;
            base.Setup(product);
        }
        
        protected override void RedrawVisual()
        {
            base.RedrawVisual();
            Background.color = _data.BackColor;

            if (_data.ContentSprites.Count > 0) 
                _image.sprite = _data.ContentSprites[0];
            else
                _image.sprite = null;

            _value.SetText(Product.Rewards.List[0].Name);
            if (Product.Rewards.HasBonus)
            {
                _bonusLabelContainer.gameObject.SetActive(true);
                _bonusRewardView.Show(Product.Rewards.BonusList[0]);
                _timer.gameObject.SetActive(false);
            }
            else
            {
                _bonusLabelContainer.gameObject.SetActive(false);
                _bonusRewardView.Hide();
                if (Product.Rewards.BonusList.Count > 0)
                {
                    _timer.gameObject.SetActive(true);
                    _timer.Listen(Service.PeriodicProducts.Refreshes[Product.PeriodicData.Type]);
                }
                else
                    _timer.gameObject.SetActive(false);
            }
        }
    }
}
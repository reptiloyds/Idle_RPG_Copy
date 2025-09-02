using System;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SwitchProductRewardView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private BaseButton _button;
        [SerializeField] private GameObject _selectVisual;
        [SerializeField] private GameObject _blockVisual;

        public Product Product { get; private set; }
        public ProductReward Reward { get; private set; }

        public event Action<SwitchProductRewardView> OnClick;

        private void Awake()
        {
            Unselect();
            _button.OnClick += OnButtonClick;
        }

        private void OnDestroy() =>
            _button.OnClick -= OnButtonClick;

        private void OnButtonClick() =>
            OnClick?.Invoke(this);

        public void Setup(Product product, ProductReward reward)
        {
            Product = product;
            Reward = reward;
            _image.sprite = Reward.Sprite;
            _blockVisual.gameObject.SetActive(!Reward.IsUnlocked);
        }

        public void Unselect() =>
            _selectVisual.SetActive(false);

        public void Select() =>
            _selectVisual.SetActive(true);
    }
}
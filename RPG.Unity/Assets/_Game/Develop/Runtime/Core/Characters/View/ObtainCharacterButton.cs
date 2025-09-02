using System;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ObtainCharacterButton : MonoBehaviour
    {
        [SerializeField] private BaseButton _button;
        [SerializeField] private BaseButton _blockButton;

        private Product _product;

        [Inject] private MessageBuffer _messageBuffer;
        
        public event Action<Product> OnClick; 

        private void Awake()
        {
            _button.OnClick += OnButtonClick;
            _blockButton.OnClick += OnBlockClick;
        }

        private void OnDestroy()
        {
            _button.OnClick -= OnButtonClick;
            _blockButton.OnClick -= OnBlockClick;
        }

        public void Setup(Product product)
        {
            _product = product;
            _blockButton.gameObject.SetActive(!_product.Rewards.Character.IsUnlocked);
        }

        private void OnButtonClick() => 
            OnClick?.Invoke(_product);

        private void OnBlockClick() => 
            _messageBuffer.Send(_product.Rewards.Character.Condition);
    }
}
using System;
using PleasantlyGames.RPG.Runtime.Core.ShopHub.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.ShopHub.View
{
    public class ShopButton : BaseButton
    {
        [SerializeField] private ShopElement _element;
        [SerializeField] private GameObject _selectObject;

        public ShopElement Element => _element;
        
        public event Action<ShopElement> OnSelect;

        protected override void Click()
        {
            base.Click();
            OnSelect?.Invoke(_element);
        }

        public void Select() => 
            _selectObject.SetActive(true);

        public void Unselect() => 
            _selectObject.SetActive(false);
    }
}
using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.View
{
    public class PhotoListButton : BaseButton
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _deselectedColor;

        public event Action<PhotoListButton> OnListClicked;
        
        public void Select() => 
            _image.color = _selectedColor;

        public void Deselect() => 
            _image.color = _deselectedColor;

        protected override void Click()
        {
            base.Click();
            
            OnListClicked?.Invoke(this);
        }
    }
}
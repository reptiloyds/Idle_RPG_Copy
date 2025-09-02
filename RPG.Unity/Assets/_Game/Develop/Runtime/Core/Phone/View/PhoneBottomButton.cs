using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.View
{
    public class PhoneBottomButton : BaseButton
    {
        [SerializeField] private Sprite _backSprite;
        [SerializeField] private Sprite _closeSprite;
        [SerializeField] private Image _image;
        
        public void SetBackVisual() => 
            _image.sprite = _backSprite;

        public void SetCloseVisual() => 
            _image.sprite = _closeSprite;
    }
}
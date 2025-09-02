using System;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Products.Model.Periodic;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View.Periodic
{
    public class PeriodicProductTabButton : BaseButton
    {
        [SerializeField] private Image _background;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _unselectedColor;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private PeriodicType _type;

        [Inject] private ITranslator _translator;
        
        public PeriodicType Type => _type;
        public event Action<PeriodicType> OnClicked;

        protected override void Awake()
        {
            base.Awake();
            
            _text.SetText(_translator.Translate(TranslationConst.PurchasePrefix + _type));
        }

        public void Select() => 
            _background.color = _selectedColor;

        public void Unselect() => 
            _background.color = _unselectedColor;

        protected override void Click()
        {
            base.Click();
            OnClicked?.Invoke(_type);
        }
    }
}
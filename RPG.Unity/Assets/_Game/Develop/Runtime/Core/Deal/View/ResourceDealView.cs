using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Deal.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Deal.View
{
    [DisallowMultipleComponent]
    public class ResourceDealView : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI _label;
        [SerializeField, Required] private BaseButton _button;
        [SerializeField] private bool _showTotalValues;
        [SerializeField] private bool _formatNumbers = true;
        [SerializeField] private List<PriceView> _priceViews;

        public event Action OnEnableEvent;
        public event Action OnDisableEvent;
        
        public bool ShowTotalValues => _showTotalValues;
        public BaseButton Button => _button;
        [Inject] private ITranslator _translator;

        private void OnEnable() => 
            OnEnableEvent?.Invoke();

        private void OnDisable() => 
            OnDisableEvent?.Invoke();

        private void Reset() => 
            _button = GetComponent<BaseButton>();

        public void SetLabelText(string text) => 
            _label.text = text;

        public void RedrawPrice(List<ResourcePrice> priceList)
        {
            int i = 0;
            for (; i < priceList.Count; i++)
            {
                if(i >= _priceViews.Count) break;
                var price = priceList[i];
                if(price.IsEmpty) continue;
                var view = _priceViews[i];
                view.Enable();
                view.SetImage(price.Sprite);
                view.SetText(price.Amount == 0 ? _translator.Translate(TranslationConst.FreeToken) : GetPriceText(price.TotalValue, price.Amount));
            }
            
            for (; i < _priceViews.Count; i++) 
                _priceViews[i].Disable();
        }

        private string GetPriceText(BigDouble.Runtime.BigDouble totalAmount, BigDouble.Runtime.BigDouble amount)
        {
            if (_formatNumbers)
            {
                return _showTotalValues
                    ? $"{StringExtension.Instance.CutBigDouble(totalAmount, true)}/{StringExtension.Instance.CutBigDouble(amount, true)}"
                    : StringExtension.Instance.CutBigDouble(amount, true);
            }

            return _showTotalValues ? $"{totalAmount}/{amount}" : $"{amount}";
        }
    }
}
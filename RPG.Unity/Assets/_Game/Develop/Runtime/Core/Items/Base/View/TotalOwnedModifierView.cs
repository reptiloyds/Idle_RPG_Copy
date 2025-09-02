using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class TotalOwnedModifierView : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI _text;
        [SerializeField] private Color _bonusColor;
        
        private List<ItemView> _views;

        [Inject] private ITranslator _translator;
        
        public void SetViews(List<ItemView> views)
        {
            _views = views;
        }

        public void Redraw()
        {
            BigDouble.Runtime.BigDouble value = 0;
            UnitStatType type = UnitStatType.None;
            foreach (var item in _views)
            {
                if (item.Model == null) continue;
                if(!item.Model.IsUnlocked) continue;
                value += item.Model.OwnedModifier.Value;
                type = item.Model.OwnedEffectType;
            }
            _text.gameObject.SetActive(type != UnitStatType.None);
            if(type == UnitStatType.None) return;
            
            var colorString = ColorUtility.ToHtmlStringRGB(_bonusColor);
            var ownedEffectString = _translator.Translate(TranslationConst.OwnedEffect);
            var typeString = _translator.Translate(type.ToString());
            var formatedValue = StringExtension.Instance.CutBigDouble(value * 100);
            _text.SetText($"{ownedEffectString}: {typeString} <color=#{colorString}>+{formatedValue}%</color>");
        }
    }
}
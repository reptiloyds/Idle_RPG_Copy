using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class GlobalBlessingView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private LevelProgressionView _levelProgressionView;
        [SerializeField] private List<GlobalBlessingEffectView> _effectViews;
        [SerializeField] private Color _bonusColor;

        [Inject] private ITranslator _translator;
        
        private GlobalBlessing _blessing;

        private void OnDestroy()
        {
            if (_blessing != null)
            {
                _blessing.OnLevelUpped -= OnLevelUpped;
                _blessing.OnProgressionChanged -= UpdateLevelProgression;
            }
        }

        public void Setup(GlobalBlessing blessing)
        {
            _blessing = blessing;
            _image.sprite = _blessing.Sprite;
            if (_name != null) 
                _name.SetText(_blessing.Name);
            if(_description != null)
                _description.SetText(_blessing.BonusDescription);
            _blessing.OnLevelUpped += OnLevelUpped;
            _blessing.OnProgressionChanged += UpdateLevelProgression;
            
            UpdateLevel();
            UpdateLevelProgression();
            UpdateBonuses();
        }

        private void OnLevelUpped(BaseBlessing blessing)
        {
            UpdateLevel();
            UpdateBonuses();
        }

        private void UpdateBonuses()
        {
            var colorString = ColorUtility.ToHtmlStringRGB(_bonusColor);
            for (var i = 0; i < _effectViews.Count; i++)
            {
                if(_blessing.Effects.Count <= i) break;
                var effect = _blessing.Effects[i];
                _effectViews[i].Setup($"{effect.StatName} <color=#{colorString}>{effect.StatModifier.Type.GetFormattedModifier(effect.StatModifier.Value)}</color>");
            }
        }

        private void UpdateLevel() => 
            _level.SetText($"{TranslationConst.LevelPrefix} {_blessing.Level}");

        private void UpdateLevelProgression() => 
            _levelProgressionView?.Redraw(_blessing.Level, _blessing.IsLevelMax, _blessing.Progression, _blessing.TargetProgression);
    }
}
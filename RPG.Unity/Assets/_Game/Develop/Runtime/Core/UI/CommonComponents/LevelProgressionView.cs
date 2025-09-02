using PleasantlyGames.RPG.Runtime.Core.Const;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents
{
    [DisallowMultipleComponent, HideMonoScript]
    public class LevelProgressionView : MonoBehaviour
    {
        [SerializeField, Required] private Slider _slider;
        [SerializeField, Required] private TextMeshProUGUI _progressText;
        [SerializeField, Required] private GameObject _maxObject;
        [SerializeField] private bool _displayLevel = true;
        [SerializeField, HideIf("@this._displayLevel == false")] private TextMeshProUGUI _level;
        [SerializeField, HideIf("@this._displayLevel == false")] private bool _useLevelPrefix = true;

        public void Redraw(int level, bool isLevelMax, int amount, int targetAmount, float duration = 0f)
        {
            if (isLevelMax)
            {
                _maxObject.gameObject.SetActive(true);
                _progressText.gameObject.SetActive(false);
                _slider.value = 1;
            }
            else
            {
                _maxObject.gameObject.SetActive(false);
                _progressText.gameObject.SetActive(true);
                _progressText.SetText($"{amount}/{targetAmount}");
                var fillAmount = (float)amount / targetAmount;
                if (duration <= 0)
                    _slider.value = fillAmount;
                else
                    PrimeTween.Tween.UISliderValue(_slider, fillAmount, duration, useUnscaledTime: true);
            }

            if (_displayLevel)
            {
                if (_useLevelPrefix)
                    _level.SetText($"{TranslationConst.LevelPrefixCaps} {level}");
                else
                    _level.SetText(level.ToString());   
            }
        }

        public void ZeroFill() =>
            _slider.value = 0;
    }
}
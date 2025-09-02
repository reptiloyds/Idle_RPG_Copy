using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents
{
    [DisallowMultipleComponent, HideMonoScript]
    public class TextTimer : UITimer
    {
        private enum TimerType
        {
            MinuteSecond = 0,
            HourMinute = 1,
            Seconds = 2,
            DaysHours = 3,
        }

        [SerializeField] private TimerType _type;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private bool _usePrefix;
        [SerializeField, HideIf("@this._usePrefix == false")] private string _prefixToken;
        
        [Inject] private ITranslator _translator;

        private string _prefix;
        private string _shortDay;
        private string _shortHour;
        private string _shortMinute;

        public override void Listen(ReadOnlyReactiveProperty<float> property, bool toEnd = true)
        {
            _prefix = _usePrefix ? _translator.Translate(_prefixToken) : string.Empty;

            _shortHour = _translator.Translate(TranslationConst.ShortHour);
            _shortMinute = _translator.Translate(TranslationConst.ShortMinute);
            _shortDay = _translator.Translate(TranslationConst.ShortDay);
            
            base.Listen(property, toEnd);
        }

        public override void UpdateValue(float leftSeconds)
        {
            switch (_type)
            {
                case TimerType.MinuteSecond:
                    var totalSeconds = Mathf.CeilToInt(leftSeconds);
                    var minutes = totalSeconds / 60;
                    var seconds = totalSeconds % 60;
                    _timerText.SetText($"{_prefix}{minutes:D2}:{seconds:D2}");
                    break;
                case TimerType.HourMinute:
                    var hour = Mathf.FloorToInt(leftSeconds / 3600);
                    var min = Mathf.CeilToInt((leftSeconds - hour * 3600) / 60);
                    _timerText.SetText($"{_prefix}{hour} {_shortHour} {min} {_shortMinute}");
                    break;
                case TimerType.Seconds:
                    _timerText.SetText($"{_prefix}{Mathf.CeilToInt(leftSeconds)}");
                    break;
                case TimerType.DaysHours:
                    var days = Mathf.FloorToInt(leftSeconds / 3600 / 24);
                    var leftHours = Mathf.CeilToInt((leftSeconds - days * 3600 * 24) / 3600);
                    _timerText.SetText($"{_prefix}{days} {_shortDay} {leftHours} {_shortHour}");
                    break;
            }
        }
    }
}
using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Cheats
{
    public class TimeScaleButton : BaseButton
    {
        [SerializeField] private float _timeScale;
        [SerializeField] private TextMeshProUGUI _text;
        
        public event Action<float> OnChangeTimeScale;

        protected override void Awake()
        {
            base.Awake();
            
            _text.SetText($"X{_timeScale}");
        }

        protected override void Click()
        {
            base.Click();
            
            OnChangeTimeScale?.Invoke(_timeScale);
        }
    }
}
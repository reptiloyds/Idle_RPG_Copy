using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Settings.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Settings.View
{
    public class SettingsWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private BaseToggle _musicToggle;
        [SerializeField] private BaseToggle _effectsSoundsToggle;
        
        [Inject] private SettingsService _service;

        protected override void Awake()
        {
            base.Awake();
            
            _musicToggle.Setup(_service.IsMusicEnabled);
            _effectsSoundsToggle.Setup(_service.IsEffectsSoundsEnabled);
            
            _musicToggle.OnStateChanged += OnMusicStateChanged;
            _effectsSoundsToggle.OnStateChanged += OnEffectsSoundsStateChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _musicToggle.OnStateChanged -= OnMusicStateChanged;
            _effectsSoundsToggle.OnStateChanged -= OnEffectsSoundsStateChanged;
        }
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        private void OnMusicStateChanged() => 
            _service.ToggleMusic();

        private void OnEffectsSoundsStateChanged() => 
            _service.ToggleEffectsSounds();
    }
}
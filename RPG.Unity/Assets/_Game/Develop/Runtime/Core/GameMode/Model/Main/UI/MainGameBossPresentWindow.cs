using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.UI
{
    public class MainGameBossPresentWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private CanvasGroup _fadeAnimationTarget;
        [SerializeField] private ShakeSettings _shakeSettings;
        [SerializeField] private List<Transform> _shakeTargets;
        [SerializeField, MinValue(0)] private float _showDuration = 1f;

        [Inject] private MainMode _mainMode;
        
        private Tween _tween;

        protected override void Awake()
        {
            base.Awake();
            _mainMode.OnDisposed += OnModeDisposed;
        }

        protected override void OnDestroy()
        {
            base.Awake();
            _mainMode.OnDisposed -= OnModeDisposed;
        }

        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenFadeTween(_fadeAnimationTarget, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseFadeTween(_fadeAnimationTarget, UseUnscaledTime, callback);

        private void OnModeDisposed(IGameMode mode)
        {
            if (!IsOpened) return;
            _tween.Stop();
            Close();
        }

        public override void Open()
        {
            base.Open();

            foreach (var shakeTarget in _shakeTargets)
            {
                shakeTarget.localScale = Vector3.one;
                Tween.PunchScale(shakeTarget, _shakeSettings);
            }
            
            _tween = Tween.Delay(_showDuration, Complete);
        }

        private async void Complete()
        {
            foreach (var shakeTarget in _shakeTargets) 
                Tween.StopAll(shakeTarget);
            
            await _mainMode.LaunchBoss();
            Close();
        }
    }
}
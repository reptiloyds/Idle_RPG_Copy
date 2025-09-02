using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View
{
    public class BlessingWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private RectTransform _blessingContainer;
        [SerializeField] private BlessingView _blessingPrefab;
        [SerializeField] private BlessingLevelUpView _blessingLevelUp;
        [SerializeField] private GlobalBlessingView _globalBlessingView;
        [SerializeField] private BaseButton _globalBlessingButton;

        [Inject] private BlessingService _service;
        [Inject] private IObjectResolver _resolver;
        [Inject] private IWindowService _windowService;

        private readonly List<BlessingView> _views = new();

        protected override void Awake()
        {
            base.Awake();
            
            _globalBlessingButton.OnClick += OnGlobalBlessingClick;
            _service.OnBlessingLevelUpped += OnBlessingLevelUpped;

            if (_service.GlobalBlessings.Count > 0)
                _globalBlessingView.Setup(_service.GlobalBlessings[0]);
            else
                _globalBlessingView.gameObject.SetActive(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var view in _views)
                view.OnRewarded -= OnRewarded;

            _globalBlessingButton.OnClick -= OnGlobalBlessingClick;
            _service.OnBlessingLevelUpped -= OnBlessingLevelUpped;
        }

        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        private async void OnGlobalBlessingClick()
        {
            if(_service.GlobalBlessings.Count == 0) return;
            var window = await _windowService.OpenAsync<GlobalBlessingWindow>();
            window.Setup(_service.GlobalBlessings[0]);
        }

        private void OnBlessingLevelUpped(BaseBlessing blessing) =>
            _blessingLevelUp.Show(blessing.Name, blessing.Level);

        public override void Open()
        {
            base.Open();

            if (_views.Count == 0)
                CreateViews();
        }

        private void CreateViews()
        {
            foreach (var blessing in _service.Blessings)
            {
                var view = _resolver.Instantiate(_blessingPrefab, _blessingContainer);
                view.Setup(blessing);
                view.OnRewarded += OnRewarded;
                _views.Add(view);
            }
        }

        private void OnRewarded(BlessingView view) => 
            _service.ActivateBlessing(view.Model);
    }
}
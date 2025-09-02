using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Accent.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.ScrollRectController;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Accent;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Accent.View
{
    [DisallowMultipleComponent,  HideMonoScript]
    public class AccentPresenter : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField, Required] private TweenSettings<float> _showSettings;
        [SerializeField, Required] private TweenSettings<float> _hideSettings;
        [SerializeField, Required] private Image _background;
        [SerializeField, Required] private RectTransform _container;
        [SerializeField, Required] private AccentView _viewPrefab;
        [SerializeField, Required] private AccentSettings _defaultSettings;
        
        private Tween _tween;
        
        private ObjectPool<AccentView> _pool;
        private readonly Dictionary<string, AccentView> _activeViews = new();

        [Inject] private AccentService _accentService;
        [Inject] private IButtonService _buttonService;

        void IInitializable.Initialize()
        {
            _pool = new ObjectPool<AccentView>(CreateView, OnGetView, OnReleaseView);
            HideBackground();
            
            _accentService.OnRequestAdded += Present;
            _accentService.OnRequestRemoved += Hide;

            foreach (var request in _accentService.Requests)
            {
                Present(request);
            }
        }
        
        void IDisposable.Dispose()
        {
            _accentService.OnRequestAdded -= Present;
            _accentService.OnRequestRemoved -= Hide;
        }

        private AccentView CreateView() => 
            Instantiate(_viewPrefab, _container);

        private void OnGetView(AccentView view) => 
            view.gameObject.SetActive(true);

        private void OnReleaseView(AccentView view)
        {
            view.RectTransform.SetParent(_container);
            view.gameObject.SetActive(false);
        }
        
        public void Present(AccentTutorialData data)
        {
            if (!_buttonService.IsButtonRegistered(data.ButtonId))
                return;
            
            var view = _pool.Get();
            var button = _buttonService.GetButton(data.ButtonId);

            if (button.TryGetComponent(out ScrollElementNormalizer scrollElementNormalizer))
            {
                scrollElementNormalizer.NormalizeScroll();
                scrollElementNormalizer.DisableScroll();
            }
            
            view.RectTransform.SetParent(button.transform);
            
            if (button.TryGetComponent(out AccentSettingsHolder settingsHolder)) 
                view.ApplySettings(settingsHolder.Target, settingsHolder.Parent, settingsHolder.Settings);
            else
            {
                var target = button.GetComponent<RectTransform>();
                view.ApplySettings(target, null, _defaultSettings);   
            }
            
            _activeViews.Add(data.ButtonId, view);
            button.AddVisualAccent();
            
            //TODO ITS NOT WORKING WITH MULTIPLE BUTTONS
            if(_tween.isAlive)
                _tween.Complete();
            ShowBackground();
            if (_showSettings.settings.duration > 0)
                _tween = Tween.Alpha(_background, _showSettings);
            else
                _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, _showSettings.endValue);
        }

        public void Hide(AccentTutorialData data)
        {
            //TODO ITS NOT WORKING WITH MULTIPLE BUTTONS
            if (!_activeViews.Remove(data.ButtonId, out var view)) return;
            
            var button = _buttonService.GetButton(data.ButtonId);
            
            button.RemoveVisualAccent();
            if (button.TryGetComponent(out ScrollElementNormalizer scrollElementNormalizer)) 
                scrollElementNormalizer.EnableScroll();
            
            if(_tween.isAlive)
                _tween.Complete();
            if (!_tween.isAlive && _activeViews.Count == 0)
            {
                if (_hideSettings.settings.duration > 0)
                    _tween = Tween.Alpha(_background, _hideSettings).OnComplete(() =>
                    {
                        HideBackground();
                        _pool.Release(view);
                    });
                else
                {
                    HideBackground();
                    _pool.Release(view);
                }
            }
        }

        private void ShowBackground()
        {
            _background.gameObject.SetActive(true);
        }

        private void HideBackground()
        {
            _background.gameObject.SetActive(false);
        }
    }
}
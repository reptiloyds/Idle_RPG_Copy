using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.ScrollRectController;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Pointer;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PointerPresenter : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField, Required] private RectTransform _container;
        [SerializeField, Required] private PointerView _viewPrefab;
        [SerializeField, Required] private PointerSettings _defaultSettings;

        private ObjectPool<PointerView> _pool;
        
        private readonly Dictionary<string, PointerView> _activeViews = new();
        
        [Inject] private IButtonService _buttonService;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private PointerService _pointerService;

        void IInitializable.Initialize()
        {
            _pool = new ObjectPool<PointerView>(CreateView, OnGetView, OnReleaseView);
            _pointerService.OnRequestAdded += Present;
            _pointerService.OnRequestRemoved += Hide;

            foreach (var request in _pointerService.Requests) 
                Present(request);
        }

        public void Dispose()
        {
            _pointerService.OnRequestAdded -= Present;
            _pointerService.OnRequestRemoved -= Hide;
        }

        private PointerView CreateView() => 
            Instantiate(_viewPrefab, _container);

        private void OnGetView(PointerView view) => 
            view.gameObject.SetActive(true);

        private void OnReleaseView(PointerView view)
        {
            view.RectTransform.SetParent(_container);
            view.gameObject.SetActive(false);
        }

        private void Present(PointerTutorialData data)
        {
            if (!_buttonService.IsButtonRegistered(data.ButtonId))
                return;
            //var sprite = string.IsNullOrEmpty(data.Sprite) ? null : _spriteProvider.GetSprite(Asset.MainAtlas, data.Sprite);
            
            var view = _pool.Get();
            //view.SetCustomSprite(sprite);
            var button = _buttonService.GetButton(data.ButtonId);
            
            if (button.TryGetComponent(out ScrollElementNormalizer scrollElementNormalizer)) 
                scrollElementNormalizer.NormalizeScroll();
            
            if (button.TryGetComponent(out PointerSettingsHolder settingsHolder)) 
                view.ApplySettings(settingsHolder.Settings);
            else
            {
                _defaultSettings.Target = button.GetComponent<RectTransform>();
                view.ApplySettings(_defaultSettings);
            }
            
            button.AddVisualAccent();
            _activeViews.Add(data.ButtonId, view);
        }

        private void Hide(PointerTutorialData data)
        {
            if (!_activeViews.Remove(data.ButtonId, out var view)) return;

            var button = _buttonService.GetButton(data.ButtonId);
            button.RemoveVisualAccent();
            
            _pool.Release(view);
        }
    }
}
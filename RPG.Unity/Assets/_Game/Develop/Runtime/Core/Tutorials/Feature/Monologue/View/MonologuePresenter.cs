using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Monologue.Model;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Monologue;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Monologue.View
{
    public class MonologuePresenter : MonoBehaviour, IInitializable, IDisposable
    {
        [Serializable]
        private class MonologuePosition
        {
            [ReadOnly]
            public string PositionId;
            public RectTransform Container;
        }

        [SerializeField, Required] private RectTransform _container;
        [SerializeField, Required] private Image _background;
        [SerializeField] private TweenSettings<float> _showBackgroundSettings;
        [SerializeField] private TweenSettings<float> _hideBackgroundSettings;
        
        [SerializeField, Required] private MonologueView _monologePrefab;
        [SerializeField] private TweenSettings<Vector3> _emergencySettings;
        [SerializeField] private List<MonologuePosition> _positions;

        [Inject] private MonologueService _monologueService;
        [Inject] private ITranslator _translator;
        [Inject] private IObjectResolver _objectResolver;

        private ObjectPool<MonologueView> _pool;
        private readonly Dictionary<string, MonologueView> _activeMonologues = new();

        private Color _backgroundColor;
        private bool _isBackgroundActive;
        private Tween _backgroundTween;

        void IInitializable.Initialize()
        {
            _backgroundColor = _background.color;
            _pool = new ObjectPool<MonologueView>(CreateView, OnGetView, OnReleaseView);
            var view = _pool.Get();
            _pool.Release(view);
            DisableBackground();
            
            _monologueService.OnRequestAdded += Present;
            _monologueService.OnRequestRemoved += Hide;
            foreach (var request in _monologueService.Requests) 
                Present(request);
        }

        public void Dispose()
        {
            _monologueService.OnRequestAdded -= Present;
            _monologueService.OnRequestRemoved -= Hide;
        }

        private MonologueView CreateView() => 
            _objectResolver.Instantiate(_monologePrefab, _container);

        private void OnGetView(MonologueView view)
        {
            view.gameObject.SetActive(true);
            Tween.Scale(view.transform, _emergencySettings);
        }

        private void OnReleaseView(MonologueView view)
        {
            view.RectTransform.SetParent(_container);
            view.gameObject.SetActive(false);
        }

        [Button("Validate")]
        private void OnValidate()
        {
            var positionIds = new HashSet<string>();
            for (int i = 0; i < _positions.Count; i++)
            {
                var position = _positions[i];
                if (position.Container != null)
                {
                    var containerName = position.Container.name;
                    if(positionIds.Add(containerName))
                        position.PositionId = containerName;
                    else
                    {
                        Debug.LogWarning($"Position {containerName} already exists");
                        _positions.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void Present(MonologueTutorialData data)
        {
            var view = _pool.Get();
            _activeMonologues.Add(data.MessageToken, view);
            view.SetText(_translator.Translate(data.MessageToken));
            var container = GetContainer(data.Position);
            view.RectTransform.SetParent(container);
            view.RectTransform.localPosition = Vector3.zero;
            view.RectTransform.offsetMin = Vector2.zero;
            view.RectTransform.offsetMax = Vector2.zero;

            if (data.BackgroundAlpha >= 0)
            {
                var color = _background.color;
                color.a = data.BackgroundAlpha;
                _background.color = color;
            }
            else
                _background.color = _backgroundColor;

            if (!_isBackgroundActive && data.Background) 
                ShowBackground();
        }

        private void Hide(MonologueTutorialData data)
        {
            if (_activeMonologues.Remove(data.MessageToken, out var view)) 
                _pool.Release(view);
            
            if(_activeMonologues.Count == 0 && _isBackgroundActive)
                HideBackground();
        }

        private void ShowBackground()
        {
            _isBackgroundActive = true;
            _backgroundTween.Stop();
            EnableBackground();
            _backgroundTween = Tween.Alpha(_background, _showBackgroundSettings);
        }

        private void HideBackground()
        {
            _isBackgroundActive = false;
            _backgroundTween.Stop();
            _backgroundTween = Tween.Alpha(_background, _hideBackgroundSettings).OnComplete(DisableBackground);
        }

        private void EnableBackground() => 
            _background.gameObject.SetActive(true);

        private void DisableBackground() => 
            _background.gameObject.SetActive(false);

        private RectTransform GetContainer(string positionId)
        {
            if (_positions.Count == 0)
            {
                Debug.LogError("Positions count must be > 0");
                return null;
            }
            foreach (var position in _positions)
                if (string.Equals(position.PositionId, positionId))
                    return position.Container;
            
            Debug.LogError($"Position {positionId} not found");
            return _positions[0].Container;
        }
    }
}
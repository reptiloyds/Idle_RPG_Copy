using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Resource.Defenition;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.View
{
    public class ResourceViewService
    {
        private readonly ResourceViewSettings[] _viewSettings = new ResourceViewSettings[]
        {
            new() {Type = ResourceType.Hard, FormatBigValue = false}
        };

        private readonly List<ResourceView> _views = new (2);
        private ResourcePopupRequest _defaultRequest;
        private readonly List<(Object source, ResourcePopupRequest request)> _requests = new();
        private RectTransform _container;
        private ResourceView _prefab;

        [Inject] private ResourceConfiguration _configuration;
        [Inject] private ResourceService _service;
        [Inject] private UIFactory _uiFactory;

        public void Setup(RectTransform container) =>
            _container = container;

        public async UniTask InitializeAsync()
        {
            var resourceGameObject = await _uiFactory.LoadAsync(Asset.UI.ResourceView, false);
            _prefab = resourceGameObject.GetComponent<ResourceView>();
            _defaultRequest = new ResourcePopupRequest(_configuration.PresentInGame, _container.parent);
            PresentRequest(_defaultRequest);
        }

        public void Popup(ResourcePopupRequest request, Object source)
        {
            _requests.Add((source, request));
            PresentRequest(request);
        }

        public void CancelPopup(Object source)
        {
            for (int i = 0; i < _requests.Count; i++)
            {
                if (_requests[i].source != source) continue;
                _requests.RemoveAt(i);
                break;
            }

            if (_requests.Count > 0) 
                PresentRequest(_requests[^1].request);
            else
                PresentRequest(_defaultRequest);
        }

        public ResourceView GetView(ResourceType type)
        {
            foreach (var resource in _views)
            {
                if (resource.Type != type) continue;
                return resource;
            }

            return null;
        }

        private void PresentRequest(ResourcePopupRequest request)
        {
            HideAll();
            
            foreach (var type in request.Types) 
                Show(type);
            
            _container.SetParent(request.Parent);
        }

        private void Show(ResourceType type)
        {
            var view = GetView(type);
            if (view == null)
            {
                var resource = _service.GetResource(type);
                view = CreateView();
                var personalSettings = GetPersonalSettings(type);
                view.Setup(resource, personalSettings);
            }
            view.Enable();
        }

        private void HideAll()
        {
            foreach (var view in _views) 
                view.Disable();
        }

        private ResourceView CreateView()
        {
            var item = Object.Instantiate(_prefab, _container);
            _views.Add(item);
            return item;
        }

        private ResourceViewSettings GetPersonalSettings(ResourceType type)
        {
            foreach (var personalSetting in _viewSettings)
                if (personalSetting.Type == type) return personalSetting;

            return null;
        }
    }
    
    public class ResourceViewSettings
    {
        public ResourceType Type;
        public bool FormatBigValue = true;
    }
    
    public struct ResourcePopupRequest
    {
        public readonly List<ResourceType> Types;
        public readonly Transform Parent;

        public ResourcePopupRequest(List<ResourceType> types, Transform parent)
        {
            Types = types;
            Parent = parent;
        }
    }
}

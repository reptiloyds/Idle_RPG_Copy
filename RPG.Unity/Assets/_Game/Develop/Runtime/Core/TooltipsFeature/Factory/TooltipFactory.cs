using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Keys;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Views;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using VContainer;
using VContainer.Unity;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;
using Object = UnityEngine.Object;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory
{
    public class TooltipFactory : IDisposable
    {
        [Inject] private ITranslator _translator;
        [Inject] private IAssetProvider _assetProvider;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private UIFactory _uiFactory;
        [Inject] private IWindowService _windowService;
        [Inject] private CompanionInventory _companionInventory;
        [Inject] private StuffInventory _stuffInventory;
        [Inject] private SkillInventory _skillInventory;
        [Inject] private ResourceService _resources;
        
        private readonly Dictionary<RectTransform, ResourceTooltipView> _actives = new();
        private ResourceTooltipView _prefab;

        public void Initialize()
        {
            Touch.onFingerDown += HandleClick;
        }

        public void Dispose()
        {
            Touch.onFingerDown -= HandleClick;
        }
        
        public async UniTask WarmUpAsync()
        {
            var result = await _uiFactory.LoadAsync(Asset.UI.ResourceTooltipView, false);
            
            if (result.TryGetComponent(out ResourceTooltipView view))
                _prefab = view;
            else
                Logger.LogError("No 'Tooltip View' found during warm up.");
        }

        public void ShowResourceTooltip(ResourceType resourceType, RectTransform target)
        {
            if (resourceType == ResourceType.None || target == null)
                return;
            
            if (_actives.Count > 0)
                ReleaseAll();
            
            ResourceTooltipKeys.ResourceTokens tokens = ResourceTooltipKeys.Resources[resourceType];
            Sprite icon = _resources.GetResource(resourceType).Sprite;
            
            ResourceTooltipView view = _objectResolver.Instantiate(_prefab);;
            _actives[target] = view;
            
            view.transform.SetParent(_windowService.GetOverUIRectTransform(), false);
            view.transform.position = Vector3.zero;
            view.transform.localScale = Vector3.one;
            view.SetupPosition(target);
            view.Setup(_translator.Translate(tokens.NameToken), _translator.Translate(tokens.DescriptionToken), icon);
        }

        public async void ShowItemTooltip(ItemType itemType)
        {
            if (_actives.Count > 0)
                ReleaseAll();

            switch (itemType)
            {
                case ItemType.None:
                    break;
                case ItemType.Stuff:
                    var stuffTooltipWindow = await _windowService.OpenAsync<StuffTooltipWindow>();
                    stuffTooltipWindow.Setup(_stuffInventory.GetItem(itemType));
                    break;
                case ItemType.Skill:
                    var skillTooltipWindow = await _windowService.OpenAsync<SkillTooltipWindow>();
                    skillTooltipWindow.Setup(_skillInventory.GetItem(itemType));
                    break;
                case ItemType.Companion:
                    var companionTooltipWindow = await _windowService.OpenAsync<CompanionTooltipWindow>();
                    companionTooltipWindow.Setup(_companionInventory.GetItem(itemType));
                    break;
            }
        }

        public void ReleaseAll()
        {
            if (_actives.Count == 0)
                return;
            
            foreach (var tooltip in _actives.ToList())
            {
                if (!_actives.TryGetValue(tooltip.Key, out var view))
                    continue;
                
                Release(view);
                _actives.Remove(tooltip.Key);
            }
        }
        
        private void Release(ResourceTooltipView view)
        {
            if (view == null)
                return;
            
            view.transform.SetParent(null);
            Object.Destroy(view.gameObject);
        }

        private void HandleClick(Finger finger)
        {
            if (_actives.Count > 0)
                ReleaseAll();
        }
    }
}
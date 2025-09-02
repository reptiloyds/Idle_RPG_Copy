using System;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Contracts;
using PleasantlyGames.RPG.Runtime.Core.UI.Backpack;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.Model
{
    public class ResourceFXFactory : IInitializable, IDisposable
    {
        [Inject] private ResourceBackpackView _backpack;
        [Inject] private ResourceViewService _resourceViewService;
        [Inject] private ResourceService _resourceService;
        [Inject] private IPopupResourceFactory _popupResourceFactory;
        [Inject] private IAudioService _audioService;
        
        private const int _maxViewReward = 7;
        private IDisposable _disposable;

        [Preserve]
        public ResourceFXFactory()
        {
        }

        public void Initialize()
        {
            _disposable = _resourceService.FXRequest
                .AsObservable()
                .Where(item => item.Type != ResourceType.None)
                .Subscribe(SpawnParticle);
        }

        public void Dispose() => 
            _disposable?.Dispose();

        private void SpawnParticle(ResourceFXRequest request)
        {
            var maxViewReward = request.MaxViewReward == 0 ? _maxViewReward : request.MaxViewReward;

            var spawnPosition = request.SpawnPosition;
            if (spawnPosition == default) 
                spawnPosition = GetCenterPosition();
            
            var target = _resourceViewService.GetView(request.Type);
            var resource = _resourceService.GetResource(request.Type);

            var viewAmount = Mathf.CeilToInt(Mathf.Min((float)request.Amount.ToDouble(), maxViewReward));
            
            if (target != null && (target.gameObject.activeSelf || request.ForceToTarget))
            {
                _popupResourceFactory.SpawnFromUI(spawnPosition, resource.Sprite, request.Amount, 
                    viewAmount, target.transform, target, true, context: request.Context);
            }
            else
            {
                _backpack.Open();
                _popupResourceFactory.SpawnFromUI(spawnPosition, resource.Sprite, request.Amount, 
                    viewAmount, _backpack.RectTransform, null, true,
                    context: request.Context, onComplete: _backpack.Close);
            }

            switch (request.Type)
            {
                case ResourceType.Soft:
                    _audioService.CreateLocalSound(UI_Earn.UI_Soft).Play();
                    break;
                case ResourceType.Hard:
                    _audioService.CreateLocalSound(UI_Earn.UI_Hard).Play();
                    break;
                default:
                    _audioService.CreateLocalSound(UI_Earn.UI_Other).Play();
                    break;
            }
        }

        private Vector2 GetCenterPosition()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            return new Vector2(screenWidth / 2, screenHeight / 2);
        }
    }
}
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Type;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Model
{
    [Serializable]
    public class PopupResourceSettings
    {
        [SerializeField] private PopupIconContext _context;
        [SerializeField, Required] private Transform _container;
            
        public PopupIconContext Context => _context;
        public Transform Container => _container;
    }
    
    public class PopupResourceFactory : IPopupResourceFactory
    {
        [Inject] private UIFactory _uiFactory;

        private readonly int _maxAmount = 10;
        private List<PopupResourceSettings> _contexts;
        private UnityEngine.Camera _camera;

        private PopupResource _prefab;
        private ObjectPool<PopupResource> _pool;
        
        [Preserve]
        public PopupResourceFactory()
        {
        }

        public async UniTask WarmUpAsync()
        {
            var popupPrefab = await _uiFactory.LoadAsync(Asset.UI.PopupResource, false);
            _prefab = popupPrefab.GetComponent<PopupResource>();
        }

        public void Setup(List<PopupResourceSettings> contexts, UnityEngine.Camera camera)
        {
            _contexts = contexts;
            _camera = camera;

            _pool = new ObjectPool<PopupResource>(() => Object.Instantiate(_prefab), Get, Release, Destroy);

            void Get(PopupResource resource) => 
                resource.gameObject.SetActive(true);

            void Release(PopupResource resource) => 
                resource.gameObject.SetActive(false);

            void Destroy(PopupResource resource) => 
                Object.Destroy(resource);
        }

        public void SpawnFromWorldPosition(Vector3 worldPosition, Sprite sprite, BigDouble.Runtime.BigDouble trueAmount, int viewAmount,
            Transform target, ResourceView resourceView = null, bool decreaseTotalValue = false, PopupIconEffect effect = PopupIconEffect.None,
            PopupIconContext context = PopupIconContext.Gameplay, Action onComplete = null)
        {
            var uiPosition = _camera.WorldToScreenPoint(worldPosition);

            SpawnFromUI(uiPosition, sprite, trueAmount, viewAmount, target, resourceView, decreaseTotalValue, effect, context, onComplete);
        }
    
        public void SpawnFromUI(Vector3 popupPosition, Sprite sprite, BigDouble.Runtime.BigDouble trueAmount, int viewAmount,
            Transform target, ResourceView resourceView = null, bool decreaseTotalValue = false, PopupIconEffect effect = PopupIconEffect.None,
            PopupIconContext context = PopupIconContext.Gameplay, Action onComplete = null)
        {
            var popupIcon = _pool.Get();
            Transform container = null;
            foreach (var contextSettings in _contexts)
            {
                if (contextSettings.Context != context) continue;
                container = contextSettings.Container;
                break;
            }
            popupIcon.transform.SetParent(container);
            popupIcon.transform.localScale = Vector3.one;
            popupIcon.transform.position = popupPosition;
            
            var particleImage = popupIcon.MainParticle;
            particleImage.sprite = sprite;
        
            particleImage.attractorEnabled = true;
            particleImage.attractorTarget = target;
        
            if (viewAmount > _maxAmount) 
                viewAmount = _maxAmount;
        
            particleImage.SetBurst(0, 0f, viewAmount);

            if (resourceView != null)
            {
                if(decreaseTotalValue)
                    resourceView.AddRedrawDelta(-trueAmount);
                particleImage.onAnyParticleFinished.AddListener(() =>
                {
                    if (decreaseTotalValue) 
                        resourceView.RemoveRedrawDelta(-trueAmount);
                    resourceView.Redraw();
                    particleImage.onAnyParticleFinished.RemoveAllListeners();
                });  
            } 
        
            particleImage.onLastParticleFinished.AddListener(() =>
            {
                onComplete?.Invoke();
                particleImage.onLastParticleFinished.RemoveAllListeners();
                _pool.Release(popupIcon);
            });
        
            particleImage.Play();
            popupIcon.PlaySecondParticles(effect);
        }
        
        Texture2D CreateTextureFromSprite(Sprite sprite)
        {
            Texture2D texture = new Texture2D(sprite.texture.width, sprite.texture.height);
            Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }   
}
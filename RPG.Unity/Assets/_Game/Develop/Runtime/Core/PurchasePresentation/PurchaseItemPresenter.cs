using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.PurchasePresentation
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PurchaseItemPresenter : MonoBehaviour
    {
        [SerializeField] private PurchaseItemView _itemPrefab;
        [SerializeField] private Transform _container;
        
        [Inject] private IObjectResolver _objectResolver;

        private readonly List<PurchaseItemView> _views = new();
        private int _counter;

        public void Append(Sprite sprite, Color color, string text, object certainProductType)
        {
            if (_views.Count <= _counter)
            {
                var rewardView = _objectResolver.Instantiate(_itemPrefab, _container);
                _views.Add(rewardView);
            }

            var view = _views[_counter];
            view.Setup(sprite, color, text, certainProductType);
            view.Enable();
            _counter++;
        }
        
        public void Clear()
        {
            _counter = 0;
            foreach (var view in _views) 
                view.Disable();
        }
    }
}
using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Products.Rewards;
using PleasantlyGames.RPG.Runtime.Core.PurchasePresentation;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ProductPurchaseWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform[] _animationTargets;
        
        [SerializeField] private RectTransform _rewardContainer;
        [SerializeField] private PurchaseItemView _rewardPrefab;

        [Inject] private IAudioService _audioService;
        [Inject] private IObjectResolver _objectResolver;

        private readonly List<PurchaseItemView> _views = new();
        
        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenScaleTween(_animationTargets, UseUnscaledTime, callback);

        protected override void CloseAnimation(Action callback) => 
            WindowTweens.CloseScaleTween(_animationTargets, UseUnscaledTime, callback);

        public override void Open()
        {
            base.Open();
            
            _audioService.CreateLocalSound(UI_Effect.UI_InAppReceive).Play();
        }

        public void Setup(IReadOnlyList<ProductReward> rewards)
        {
            foreach (var view in _views) 
                view.Disable();
            
            var id = 0;
            foreach (var item in rewards)
            {
                var view = GetView(id);
                view.Setup(item.Sprite, item.BackColor, item.Name, item.GetCertainProductType());
                view.PlayPresentAnimation();
                id++;
            }
        }

        private PurchaseItemView GetView(int id)
        {
            if (id >= _views.Count)
            {
                var item = _objectResolver.Instantiate(_rewardPrefab, _rewardContainer);
                _views.Add(item);
                item.Enable();
                return item;
            }

            var view = _views[id];
            view.Enable();
            return view;
        }
    }
}
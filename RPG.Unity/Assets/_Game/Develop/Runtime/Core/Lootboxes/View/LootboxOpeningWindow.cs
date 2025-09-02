using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Deal.Controller;
using PleasantlyGames.RPG.Runtime.Core.Deal.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Tween;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.View
{
    public class LootboxOpeningWindow : BaseWindow
    {
        [SerializeField, Required] private LootboxRewardView _rewardPrefab;
        [SerializeField, Required] private RectTransform _poolContainer;
        [SerializeField, Required] private RectTransform _container;
        [SerializeField] private List<ResourceDealView> _resourceDealViews;
        [SerializeField] private List<UIScale> _scaleAnimations;
        [SerializeField, MinValue(0)] private float _spawnItemDelay;
        [SerializeField, MinValue(0)] private float _rareItemDelayBefore = 0.5f;
        [SerializeField, MinValue(0)] private float _rareItemDelayAfter = 0.25f;
        [SerializeField, Required] private RectTransform _shakeTarget;
        [SerializeField] private ShakeSettings _shakeSettings;

        private ObjectPool<LootboxRewardView> _pool;
        private readonly List<LootboxRewardView> _views = new();

        private readonly List<ResourceDealController> _deals = new();
        private readonly Dictionary<ResourceDealController, int> _dealAmount = new();
        private Lootbox _lootbox;
        private Tween _scaleDelayTween;
        private Tween _shakeTween;
        private AudioEmitter _audioEmitter;
        
        [Inject] private ResourceService _resourceService;
        [Inject] private MessageBuffer _messageBuffer;
        [Inject] private ITranslator _translator;
        [Inject] private LootboxService _service;
        [Inject] private IAudioService _audioService;
        [Inject] private IObjectResolver _objectResolver;

        protected override void Awake()
        {
            base.Awake();
            
            _service.OnItemsApplied += OnItemsApplied;
            _pool = new ObjectPool<LootboxRewardView>(CreateView, OnGet, OnRelease);

            foreach (var dealView in _resourceDealViews)
            {
                dealView.gameObject.SetActive(true);
                var dealController = new ResourceDealController(dealView, _resourceService, _messageBuffer, _translator);
                
                _deals.Add(dealController);
                _dealAmount.Add(dealController, 0);
                dealController.OnSuccess += OnDealSuccess;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var deal in _deals) 
                deal.OnSuccess -= OnDealSuccess;
            
            _service.OnItemsApplied -= OnItemsApplied;
        }

        private void OnItemsApplied(Lootbox lootbox, IReadOnlyList<Item> items)
        {
            if (IsOpened)
                ClearItems();
            
            foreach (var item in items)
            {
                if (item is StuffItem stuffItem)
                    AppendItem(stuffItem, stuffItem.SlotType.EquippedSlotImage, stuffItem.SlotType.EquippedSlotColor);
                else
                    AppendItem(item);
            }

            if (!IsOpened)
            {
                SetupLootbox(lootbox);
                Open();
            }
            
            AnimateOpeningAsync().Forget();
        }

        private void OnDealSuccess(ResourceDealController dealController)
        {
            ClearItems();
            var amount = _dealAmount[dealController];
            _lootbox.Open(amount);
        }

        private void AppendItem(Item item, Sprite slotImage = null, Color backgroundColor = default)
        {
            var view = _pool.Get();
            view.Setup(item.Sprite, item.RarityColor, item.Rarity, item.Id, item.Type);
            if (slotImage != null) 
                view.ShowSlotImage(slotImage, backgroundColor);
            else
                view.HideSlotImage();
            _views.Add(view);
        }

        private void SetupLootbox(Lootbox lootbox)
        {
            _lootbox = lootbox;
            int index = 0;
            foreach (var deal in _deals)
            {
                if(index >= _lootbox.PurchaseDataList.Count)
                    deal.View.gameObject.SetActive(false);
                else
                {
                    var purchaseData = _lootbox.PurchaseDataList[index];
                    deal.View.gameObject.SetActive(true);
                    deal.View.SetLabelText($"{_translator.Translate(TranslationConst.Buy)} X{purchaseData.ItemAmount}");
                    
                    deal.ClearPrice();
                    deal
                        .AddPrice(purchaseData.ResourceType, purchaseData.Price)
                        .BuildPrice();
                    
                    _dealAmount[deal] = purchaseData.ItemAmount;
                }
                index++;
            }
        }
        
        private async UniTask AnimateOpeningAsync()
        {
            _audioEmitter = _audioService.CreateLocalSound(UI_Effect.UI_ItemPopup).DontRelease().Build();
            DisablePurchaseInteraction();

            foreach (var scale in _scaleAnimations)
                scale.Target.localScale = Vector3.zero;

            List<int> firstRareIndexes = FindAllMostValuableNewItemIndexes();

            for (int i = 0; i < _views.Count; i++)
            {
                var view = _views[i];

                if (firstRareIndexes.Contains(i))
                    await UniTask.Delay(TimeSpan.FromSeconds(_rareItemDelayBefore), ignoreTimeScale: true);
                
                view.Play(0, _audioEmitter);

                if (firstRareIndexes.Contains(i))
                {
                    if (_shakeTween.isAlive)
                        _shakeTween.Stop();

                    _shakeTarget.anchoredPosition = Vector3.zero;
                    _shakeTween = Tween.PunchLocalPosition(_shakeTarget, _shakeSettings);

                    await UniTask.Delay(TimeSpan.FromSeconds(_rareItemDelayAfter), ignoreTimeScale: true);
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_spawnItemDelay), ignoreTimeScale: true);
                }
            }
            
            await PlayScaleAnimations();
        }
        
        private async UniTask PlayScaleAnimations()
        {
            _audioEmitter.ReleaseOnEnd();

            foreach (var scale in _scaleAnimations)
                scale.Play();

            float maxDuration = _scaleAnimations.Max(s => s.Duration);
            await UniTask.Delay(TimeSpan.FromSeconds(maxDuration), ignoreTimeScale: true);

            EnablePurchaseInteraction();
        }
        
        private List<int> FindAllMostValuableNewItemIndexes()
        {
            var result = new List<int>();
            
            var idFilter = new HashSet<string>(_lootbox.NewItems.Select(x => x.Id));
            var addedFilter = new HashSet<string>();
            var rarityFilter = new HashSet<ItemRarityType>
            {
                ItemRarityType.Exotic,
                ItemRarityType.Mythic,
                ItemRarityType.Legendary,
                ItemRarityType.Epic
            };

            for (int i = 0; i < _views.Count; i++)
            {
                var view = _views[i];

                if (rarityFilter.Contains(view.Rarity)
                    && idFilter.Contains(view.ItemId) 
                    && !addedFilter.Contains(view.ItemId))
                {
                    result.Add(i);
                    addedFilter.Add(view.ItemId);
                }
            }

            return result;
        }

        private void DisablePurchaseInteraction()
        {
            foreach (var deal in _deals) 
                deal.DisableInternalInteraction();
        }

        private void EnablePurchaseInteraction()
        {
            foreach (var deal in _deals) 
                deal.EnableInternalInteraction();
        }

        public override void Close()
        {
            base.Close();

            _lootbox = null;
            ClearItems();
        }

        private void ClearItems()
        {
            foreach (var view in _views) 
                _pool.Release(view);
            _views.Clear();
        }

        private LootboxRewardView CreateView() => 
            _objectResolver.Instantiate(_rewardPrefab, _poolContainer);

        private void OnGet(LootboxRewardView view)
        {
            view.transform.SetParent(_container);
            view.gameObject.SetActive(true);
        }

        private void OnRelease(LootboxRewardView view)
        {
            view.transform.SetParent(_poolContainer);
            view.gameObject.SetActive(false);
        }
    }
}
using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Collections.Model;
using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CollectionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private CollectionItemView _itemView;
        [SerializeField] private RectTransform _container;
        [SerializeField] private GameObject _enhanceReadyObject;
        [SerializeField] private TextMeshProUGUI _bonusText;
        [SerializeField] private BaseButton _enhanceButton;
        [SerializeField] private GameObject _maxObject;
        [SerializeField] private GameObject _blockObject;
        [SerializeField] private Color _bonusDeltaColor;

        private readonly List<CollectionItemView> _views = new();
        private Collection _collection;
        private string _bonusDeltaColorHex;

        public Collection Collection => _collection;
        public BaseButton EnhanceButton => _enhanceButton;

        public event Action<CollectionView> OnEnhanceClicked;

        private void Awake()
        {
            _enhanceButton.OnClick += OnEnhanceClick;
            _bonusDeltaColorHex = _bonusDeltaColor.ToHexString();
        }

        private void OnDestroy()
        {
            _enhanceButton.OnClick -= OnEnhanceClick;
            if(_collection != null)
                _collection.OnUpdate -= OnCollectionUpdate;
        }

        public void Setup(Collection collection)
        {
            _collection = collection;
            _collection.OnUpdate += OnCollectionUpdate;
            CreateItems(collection.Items);
            
            Redraw();
        }

        private void OnCollectionUpdate(Collection collection) => 
            Redraw();

        private void CreateItems(IReadOnlyList<Item> items)
        {
            foreach (var item in items)
            {
                var view = Instantiate(_itemView, _container);
                view.Setup(item);
                _views.Add(view);
            }
        }

        private void OnEnhanceClick() => 
            OnEnhanceClicked?.Invoke(this);

        private void Redraw()
        {
            _blockObject.SetActive(!_collection.IsUnlocked);
            _maxObject.SetActive(_collection.IsLevelMax);
            _enhanceButton.gameObject.SetActive(!_collection.IsLevelMax);
            _enhanceButton.SetInteractable(_collection.CanEnhance);
            _enhanceReadyObject.SetActive(_collection.CanEnhance);
            _label.SetText(_collection.FormattedName);

            var requestedLevel = _collection.IsLevelMax ? _collection.Level : _collection.Level + 1;
            foreach (var view in _views)
                view.SetRequestedLevel(requestedLevel);

            if (!_collection.CanEnhance)
                _bonusText.SetText(_collection.FormattedBonus);
            else
                _bonusText.SetText($"{_collection.FormattedBonus} <color={_bonusDeltaColorHex}>{_collection.FormattedBonusDelta}</color>");
        }
    }
}
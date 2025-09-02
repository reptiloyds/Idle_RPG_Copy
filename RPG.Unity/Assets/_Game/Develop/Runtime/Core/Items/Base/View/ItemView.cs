using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ItemView : MonoBehaviour
    {
        [SerializeField, Required] private BaseButton _button;
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Image _rarityImage;
        [SerializeField, Required] private TextMeshProUGUI _levelText;
        [SerializeField, Required] private List<GameObject> _promotionObjects;
        [SerializeField, Required] private Slider _progressSlider;
        [SerializeField, Required] private TextMeshProUGUI _progressText;
        [SerializeField, Required] private GameObject _maxObject;
        [SerializeField, Required] private GameObject _equippedObject;
        
        [SerializeField, Required] private GameObject _equippedSignContainer;
        [SerializeField, Required] private GameObject _equippedSign; 
        [SerializeField, Required] private GameObject _unequippedSign;
        
        [SerializeField, Required] private GameObject _blockObject;
        [SerializeField, Required] private GameObject _selectedObject;

        private Item _model;

        public event Action<ItemView> OnClick;
        public Item Model => _model;

        private void Awake() => 
            _button.OnClick += OnButtonClick;

        private void OnDestroy() => 
            _button.OnClick -= OnButtonClick;

        private void OnButtonClick() => 
            OnClick?.Invoke(this);

        public void ShowEquippedObject() => 
            _equippedSignContainer.gameObject.SetActive(true);

        public void HideEquippedObject() => 
            _equippedSignContainer.gameObject.SetActive(false);

        public void SetButtonId(string id) => 
            _button.ChangeButtonId(id);

        public void SetModel(Item item)
        {
            _model = item;
            Redraw();

            _model.OnLevelUp += OnItemUpdate;
            _model.OnAmountChanged += OnItemUpdate;
            _model.OnUnlock += OnItemUpdate;
            _model.OnEquip += OnItemUpdate;
            _model.OnTakeOff += OnItemUpdate;
        }

        public void ClearModel()
        {
            _model.OnLevelUp -= OnItemUpdate;
            _model.OnAmountChanged -= OnItemUpdate;
            _model.OnUnlock -= OnItemUpdate;
            _model.OnEquip -= OnItemUpdate;
            _model.OnTakeOff -= OnItemUpdate;
            _model = null;
        }

        private void OnItemUpdate(Item item)
        {
            Redraw();
        }

        private void Redraw()
        {
            _image.sprite = _model.Sprite;
            _rarityImage.color = _model.RarityColor;
            _levelText.SetText($"{TranslationConst.LevelPrefixCaps} {_model.Level}");
            _blockObject.SetActive(!_model.IsUnlocked);
            
            _equippedSign.SetActive(_model.IsEquipped);
            _unequippedSign.SetActive(!_model.IsEquipped);
            _equippedObject.SetActive(_model.IsEquipped);

            if (_model.IsLevelMax)
            {
                _maxObject.gameObject.SetActive(true);
                _progressText.gameObject.SetActive(false);
                _progressSlider.value = 1;
            }
            else
            {
                _maxObject.gameObject.SetActive(false);
                _progressText.gameObject.SetActive(true);
                _progressText.SetText($"{_model.Amount}/{_model.TargetAmount}");
                _progressSlider.value = (float)_model.Amount / _model.TargetAmount;
            }
            
            foreach (var promotionObject in _promotionObjects) 
                promotionObject.SetActive(_model.CanEnhance);
        }

        public void Select() => _selectedObject.SetActive(true);

        public void Unselect() => _selectedObject.SetActive(false);

        public void Enable() => gameObject.SetActive(true);

        public void Disable() => gameObject.SetActive(false);
    }
}
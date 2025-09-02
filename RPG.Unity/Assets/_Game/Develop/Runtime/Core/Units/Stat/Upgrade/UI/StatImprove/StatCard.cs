using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Deal.Controller;
using PleasantlyGames.RPG.Runtime.Core.Deal.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.UI.StatImprove
{
    [DisallowMultipleComponent, HideMonoScript]
    public class StatCard : MonoBehaviour
    {
        [SerializeField, Required] private ResourceDealView _resourceDealView;
        [SerializeField, Required] private List<GameObject> _resourceDealObjects;
        [SerializeField, Required] private Image _backgroundImage;
        [SerializeField] private Color _firstBackColor;
        [SerializeField] private Color _secondBackColor;
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private TextMeshProUGUI _name;
        [SerializeField, Required] private TextMeshProUGUI _level;
        [SerializeField, Required] private TextMeshProUGUI _value;
        [SerializeField, Required] private GameObject _lockObject;
        [SerializeField, Required] private TextMeshProUGUI _lockText;
        [SerializeField, Required] private GameObject _levelUpObject;
        [SerializeField, Required] private GameObject _maxLevelObject;
        [SerializeField] private float _levelUpDuration = 0.2f;
        [SerializeField] private ShakeSettings _punchValueSettings;
        
        [ShowInInspector, HideInEditorMode, ReadOnly]
        private ImprovableUnitStat _stat;
        private Tween _levelUpTween;

        private ResourceDealController _dealController;
        private StatCardSetup _setup;

        [Inject] private ResourceService _resourceService;
        [Inject] private ITranslator _translator;
        [Inject] private MessageBuffer _messageBuffer;

        public BaseButton Button => _resourceDealView.Button;

        public bool IsLevelMax => _stat.IsLevelMax;
        public event Action<UnitStatType> Purchase;
        public UnitStatType Type => _stat?.Type ?? UnitStatType.None;

        public void Initialize()
        {
            _dealController = new ResourceDealController(_resourceDealView, _resourceService, _messageBuffer, _translator);
            _dealController.OnSuccess += OnSuccessDeal;
            _translator.OnChangeLanguage += OnChangeLanguage;
        }

        public void Setup(StatCardSetup setup, IUnlockable unlockable)
        {
            _setup = setup;
            if (unlockable is { IsUnlocked: false })
            {
                unlockable.OnUnlocked += OnUnlocked;
                Lock();
                UpdateLockText(unlockable.Condition);
            }
        }

        private void OnUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlocked;
            Unlock();
        }

        private void OnSuccessDeal(ResourceDealController dealController)
        {
            Purchase?.Invoke(_stat.Type);
        }

        private void OnChangeLanguage()
        {
            if(!gameObject.activeSelf) return;
            _name.SetText(_translator.Translate(_stat.TypeString));
        }

        public void SetStat(ImprovableUnitStat stat)
        {
            if(_stat != null)
                ClearStat();
            _stat = stat;
            
            _stat.OnLevelUp += OnLevelUp;
            _stat.OnValueChanged += RedrawDynamicData;
            
            RedrawStaticData();
            RedrawDynamicData();
            UpdatePrice();
        }

        public void ClearStat()
        {
            _stat.OnLevelUp -= OnLevelUp;
            _stat.OnValueChanged -= RedrawDynamicData;
            _stat = null;
        }

        public void Lock() => 
            _lockObject.gameObject.SetActive(true);

        public void Unlock() => 
            _lockObject.gameObject.SetActive(false);

        public void UpdateLockText(string text) => 
            _lockText.SetText(text);

        public void Enable()
        {
            if(gameObject.activeSelf) return;
            gameObject.SetActive(true);
            _value.transform.localScale = Vector3.one;
        }

        public void Disable()
        {
            if(!gameObject.activeSelf) return;
            gameObject.SetActive(false);
        }

        public void SetFirstColor() => 
            _backgroundImage.color = _firstBackColor;

        public void SetSecondColor() => 
            _backgroundImage.color = _secondBackColor;

        private void OnLevelUp()
        {
            RedrawDynamicData();
            UpdatePrice();
            PunchValue();

            EnableLevelUpObject();
            _levelUpTween.Stop();
            _levelUpTween = Tween.Delay(_levelUpDuration, DisableLevelUpObject);
        }

        private void EnableLevelUpObject() => 
            _levelUpObject.gameObject.SetActive(true);

        private void DisableLevelUpObject() => 
            _levelUpObject.gameObject.SetActive(false);

        private void RedrawStaticData()
        {
            _image.sprite = _stat.Sprite;
            _name.SetText(_translator.Translate(_stat.TypeString));
        }

        private void RedrawDynamicData()
        {
            _level.SetText($"{TranslationConst.LevelPrefixCaps} {_stat.Level}");
            _value.SetText($"{StringExtension.Instance.CutBigDouble(_stat.StableValue, _setup.RoundFirstGrade)}{_stat.ValuePostfix}");

            if (_stat.IsLevelMax && !_maxLevelObject.activeSelf)
            {
                foreach (var resourceDealObject in _resourceDealObjects) 
                    resourceDealObject.SetActive(false);
                _maxLevelObject.SetActive(true);   
            }
            else if (!_stat.IsLevelMax && _maxLevelObject.activeSelf)
            {
                foreach (var resourceDealObject in _resourceDealObjects) 
                    resourceDealObject.SetActive(true);
                _maxLevelObject.SetActive(false);
            }
        }

        private void UpdatePrice()
        {
            _dealController.ClearPrice();
            _dealController.AddPrice(ResourceType.Soft, _stat.GetPrice());
            _dealController.BuildPrice();
        }
        
        private void PunchValue() => 
            Tween.PunchScale(_value.transform, _punchValueSettings);
    }
}
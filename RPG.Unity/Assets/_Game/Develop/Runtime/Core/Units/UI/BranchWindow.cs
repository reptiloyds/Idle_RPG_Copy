using System;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.View;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BranchWindow : BaseWindow
    {
        [SerializeField, BoxGroup("Animation")] private RectTransform _animationTarget;

        [SerializeField, Required] private RawImage _rawImage;
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private StuffSlotPresenter _stuffSlotPresenter;
        [SerializeField, Required] private BaseButton _characterWindowButton;
        [SerializeField, Required] private TextMeshProUGUI _characterNameText;
        [SerializeField, Required] private CharacterBonusPresenter _characterBonusPresenter;
        [SerializeField, Required] private CharacterSkillView _characterSkillView;
        [SerializeField, Required] private BranchSwitchView _branchSwitchView;
        [SerializeField] private WindowTweens.VerticalAnimationType _animationType = WindowTweens.VerticalAnimationType.FromBottom;
        
        private Character _character;

        [Inject] private BranchService _branchService;
        [Inject] private CharacterService _characterService;
        [Inject] private ITranslator _translator;
        [Inject] private IWindowService _windowService;

        protected override void Awake()
        {
            base.Awake();
            
            _stuffSlotPresenter.OnSlotClick += OnSlotClick;
            _characterWindowButton.OnClick += OnCharacterWindowButtonClick;
            _characterService.OnAnySwitched += CharacterAnySwitched;
            _branchService.SwitchBranch += RedrawDynamicData;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _stuffSlotPresenter.OnSlotClick -= OnSlotClick;
            _characterWindowButton.OnClick -= OnCharacterWindowButtonClick;
            _characterService.OnAnySwitched -= CharacterAnySwitched;
            _branchService.SwitchBranch -= RedrawDynamicData;
        }

        protected override void OpenAnimation(Action callback) => 
            WindowTweens.OpenPositionTween(_animationTarget, UseUnscaledTime, callback, _animationType);

        private void CharacterAnySwitched() => 
            RedrawDynamicData();

        private async void OnCharacterWindowButtonClick() => 
            await _windowService.OpenAsync<CharacterWindow>();

        public override void Open()
        {            
            RedrawDynamicData();
            
            base.Open();
        }

        private void RedrawDynamicData()
        {
            var branch = _branchService.GetSelectedBranch();
            _character = _characterService.GetCharacter(branch.CharacterId);
            var mainImage = _character.MainImage;
            if (mainImage == null)
            {
                _rawImage.gameObject.SetActive(true);
                _image.gameObject.SetActive(false);
            }
            else
            {
                _image.sprite = mainImage;
                _rawImage.gameObject.SetActive(false);
                _image.gameObject.SetActive(true);
            }
            
            _characterNameText.SetText(_character.FormattedName);
            _characterBonusPresenter.Setup(_character.GetBonuses());
            _characterSkillView.Setup(_character);
            _stuffSlotPresenter.Present();
            _stuffSlotPresenter.Redraw();
        }

        private async void OnSlotClick(StuffSlot slot)
        {
            var window = await _windowService.OpenAsync<StuffInventoryWindow>();
            window.Setup(slot);
            window.SetEquippedItem(slot.Item);
        }

        public StuffSlotView GetViewByModel(StuffSlot slot) => 
            _stuffSlotPresenter.GetViewByModel(slot);
    }
}
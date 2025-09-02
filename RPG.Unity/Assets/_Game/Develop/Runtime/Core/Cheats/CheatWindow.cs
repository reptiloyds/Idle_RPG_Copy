using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Cheats
{
    public class CheatWindow : BaseWindow
    {
        [SerializeField] private List<TimeScaleButton> _timeScaleButtons;
        [SerializeField, Required] private BaseButton _saveButton;

        [SerializeField, Required] private TMP_Dropdown _resourceTypeDropdown;
        [SerializeField, Required] private TMP_InputField _resourceAmountInputField;
        [SerializeField, Required] private BaseButton _resourceSetButton;
        
        [SerializeField, Required] private TMP_InputField _periodInputField;
        [SerializeField, Required] private TMP_InputField _levelInputField;
        [SerializeField, Required] private BaseButton _changeLevelButton;
        
        [SerializeField, Required] private TMP_Dropdown _statTypeDropdown;
        [SerializeField, Required] private TMP_InputField _statLevelInputField;
        [SerializeField, Required] private BaseButton _statButton;
        
        [SerializeField, Required] private TMP_InputField _questImputField;
        [SerializeField, Required] private BaseButton _questButton;
        
        [SerializeField, Required] private TMP_InputField _tutorImputField;
        [SerializeField, Required] private BaseButton _tutorButton;
        
        [SerializeField, Required] private BaseButton _unlockBranchesButton;
        [SerializeField, Required] private BaseButton _unlockButton;
        [SerializeField, Required] private BaseButton _unlockItemsButton;
        
        [SerializeField, Required] private BaseButton _internalLogicTestButton;
        [SerializeField] private TextMeshProUGUI _balanceFileName;

        [Inject] private CheatService _cheatService;

        protected override void Awake()
        {
            base.Awake();
            
            _resourceTypeDropdown.options.Clear();

            var resourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>().ToList();
            List<string> options = new List<string>();
            foreach (var resourceType in resourceTypes) 
                options.Add(resourceType.ToString());
            _resourceTypeDropdown.AddOptions(options);
            
            _statTypeDropdown.options.Clear();
            var statTypes = _cheatService.GetAllStatTypes();
            options = new List<string>();
            foreach (var statType in statTypes) 
                options.Add(statType.ToString());
            _statTypeDropdown.AddOptions(options);
            
            foreach (var timeButton in _timeScaleButtons)
                timeButton.OnChangeTimeScale += OnChangeTimeScale;
            _saveButton.OnClick += OnSaveClick;
            _resourceSetButton.OnClick += OnResourceSetClick;
            _changeLevelButton.OnClick += OnChangeLevelClick;
            _statButton.OnClick += OnStatClick;
            _questButton.OnClick += OnQuestButtonClick;
            _tutorButton.OnClick += OnTutorButtonClick;
            _unlockButton.OnClick += OnUnlockContentClick;
            _internalLogicTestButton.OnClick += OnInternalLogicTestButtonClick;
            _unlockItemsButton.OnClick += OnUnlockItemsClick;
            _cheatService.OnBalanceFileChanged += RedrawBalanceFileName;
            _unlockBranchesButton.OnClick += OnUnlockBranches;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var timeButton in _timeScaleButtons) 
                timeButton.OnChangeTimeScale -= OnChangeTimeScale;
            _saveButton.OnClick -= OnSaveClick;
            _resourceSetButton.OnClick -= OnResourceSetClick;
            _changeLevelButton.OnClick -= OnChangeLevelClick;
            _statButton.OnClick -= OnStatClick;
            _questButton.OnClick -= OnQuestButtonClick;
            _tutorButton.OnClick -= OnTutorButtonClick;
            _unlockButton.OnClick -= OnUnlockContentClick;
            _internalLogicTestButton.OnClick -= OnInternalLogicTestButtonClick;
            _unlockItemsButton.OnClick -= OnUnlockItemsClick;
            _cheatService.OnBalanceFileChanged -= RedrawBalanceFileName;
            _unlockBranchesButton.OnClick -= OnUnlockBranches;
        }

        private void OnUnlockBranches()
        {
            _cheatService.UnlockBranches();
        }

        private void RedrawBalanceFileName() => 
            _balanceFileName.SetText(_cheatService.BalanceFileName);

        private void OnInternalLogicTestButtonClick()
        {
            _cheatService.TestInternalLogic();
        }

        private void OnUnlockContentClick()
        {
            _cheatService.UnlockContent();
        }

        private void OnStatClick()
        {
            UnitStatType statType = Enum.Parse<UnitStatType>(_statTypeDropdown.options[_statTypeDropdown.value].text);
            int level = int.Parse(_statLevelInputField.text);
            _cheatService.SetParamLevel(statType, level);
        }

        private void OnChangeLevelClick()
        {
            int period = int.Parse(_periodInputField.text);
            int level = int.Parse(_levelInputField.text);
            _cheatService.ChangeLevel(period, level);
        }

        private void OnResourceSetClick()
        {
            var resourceType = Enum.Parse<ResourceType>(_resourceTypeDropdown.options[_resourceTypeDropdown.value].text);
            var amount = BigDouble.Runtime.BigDouble.Parse(_resourceAmountInputField.text);
            _cheatService.SetResource(resourceType, amount);
        }

        private void OnQuestButtonClick()
        {
            var questId  = int.Parse(_questImputField.text);
            _cheatService.WarpQuest(questId);
        }
        
        private void OnTutorButtonClick()
        {
            var tutorialId  = _tutorImputField.text;
            _cheatService.TestTutorial(tutorialId);
        }

        private void OnSaveClick() => 
            _cheatService.Save();

        private void OnChangeTimeScale(float timeScale) => 
            _cheatService.SetTimeScale(timeScale);

        private void OnUnlockItemsClick() => 
            _cheatService.UnlockItems();

        public override void Open()
        {
            base.Open();
            _cheatService.DisableInput();
            RedrawBalanceFileName();
        }

        public override void Close()
        {
            base.Close();
            _cheatService.EnableInput();
        }
    }
}
using System;
using PleasantlyGames.RPG.Runtime.BonusAccess.View;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SubModeEnterButtons : MonoBehaviour
    {
        [SerializeField] private BaseButton _defaultButton;
        [SerializeField] private BonusAccessButton _bonusButton;

        [Inject] private ITranslator _translator;
        
        private SubMode _subMode;
        private Func<bool> _executeCondition;
        
        public BaseButton DefaultButton => _defaultButton;
        public BaseButton BonusButton => _bonusButton;

        public event Action OnBonusEnter;
        public event Action OnEnter;
        public event Action OnClicked;
        
        private void Awake()
        {
            _defaultButton.OnClick += OnEnterClick;
            _bonusButton.OnExecuted += OnBonusExecuted;
        }

        private void OnDestroy()
        {
            _defaultButton.OnClick -= OnEnterClick;
            _bonusButton.OnExecuted -= OnBonusExecuted;
        }

        public void Setup(SubMode subMode,
            bool setFakeMainAction,
            Func<bool> executeCondition = null)
        {
            _subMode = subMode;
            _executeCondition = executeCondition;
            if (setFakeMainAction)
                _bonusButton.SetMainAction(() => {});
            if (_executeCondition != null) 
                _bonusButton.SetExecuteCondition(() => _subMode.BonusEnterAmount > 0 && _executeCondition.Invoke());
            else
                _bonusButton.SetExecuteCondition(() => _subMode.BonusEnterAmount > 0);
            _bonusButton
                .SetLabelText(_translator.Translate(TranslationConst.DungeonEnter))
                .Build();
        }

        private void OnEnterClick()
        {
            if(_executeCondition != null && !_executeCondition.Invoke()) return;
            OnEnter?.Invoke();
            OnClicked?.Invoke();
        }

        private void OnBonusExecuted()
        {
            OnBonusEnter?.Invoke();
            OnClicked?.Invoke();
        }

        public void Redraw()
        {
            if (_subMode.IsEnterResourceEnough)
            {
                _defaultButton.gameObject.SetActive(true);
                _defaultButton.SetInteractable(true);
                _bonusButton.gameObject.SetActive(false);
            }
            else
            {
                _defaultButton.gameObject.SetActive(false);
                _bonusButton.gameObject.SetActive(true);
                _bonusButton.UpdateState();
            }
        }
    }
}
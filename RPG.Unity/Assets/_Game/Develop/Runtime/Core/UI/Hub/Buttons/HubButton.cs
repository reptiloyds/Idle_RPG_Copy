using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Hub.Buttons
{
    public abstract class HubButton : BaseButton
    {
        [SerializeField] private GameObject _closeVisual;
        [SerializeField] private UISoundType _closeSound = UISoundType.UI_CloseClick;

        private bool _isActive;
        [ShowInInspector, HideInEditorMode, ReadOnly]
        protected bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                SetClickSound(_isActive ? _closeSound : _originSound);
            }
        }

        private UISoundType _originSound;
        
        public event Action<HubButton> OnAutoActivate; 
        public event Action<HubButton> OnAutoDeactivate; 

        protected override void Awake()
        {
            base.Awake();
            DisableCloseVisual();
            _originSound = GetSoundType();
        }

        public abstract void Deactivate();

        protected void TriggerAutoActivate() => 
            OnAutoActivate?.Invoke(this);

        protected void TriggerAutoDeactivate() => 
            OnAutoDeactivate?.Invoke(this);

        protected void EnableCloseVisual() => 
            _closeVisual.SetActive(true);

        protected void DisableCloseVisual() => 
            _closeVisual.SetActive(false);
    }
}
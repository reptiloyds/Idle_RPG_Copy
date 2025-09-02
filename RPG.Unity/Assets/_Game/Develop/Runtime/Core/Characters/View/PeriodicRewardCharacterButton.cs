using System;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PeriodicRewardCharacterButton : MonoBehaviour
    {
        [SerializeField] private BaseButton _button;
        [SerializeField] private BaseButton _blockButton;
        
        [Inject] private MessageBuffer _messageBuffer;
        [Inject] private ContentService _contentService;

        private IUnlockable _unlockable;
        public event Action OnClick; 

        private void Awake()
        {
            _button.OnClick += OnButtonClick;
            _blockButton.OnClick += OnBlockClick;
            _unlockable = _contentService.GetPopupWindow(nameof(DailyLoginRewardWindow));
            if(!_unlockable.IsUnlocked)
                _unlockable.OnUnlocked += OnUnlocked;
            RedrawBlockButton();
        }

        private void OnDestroy()
        {
            _button.OnClick -= OnButtonClick;
            _blockButton.OnClick -= OnBlockClick;
        }

        private void OnUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlocked;
            RedrawBlockButton();
        }
        
        private void RedrawBlockButton()
        {
            var unlocked = _unlockable == null || _unlockable.IsUnlocked;
            _blockButton.gameObject.SetActive(!unlocked);
        }

        private void OnButtonClick() => 
            OnClick?.Invoke();

        private void OnBlockClick() => 
            _messageBuffer.Send(_unlockable.Condition);
    }
}
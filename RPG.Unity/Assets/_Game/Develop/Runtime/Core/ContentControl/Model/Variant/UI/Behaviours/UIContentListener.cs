using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PrimeTween;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.UI.Behaviours
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIContentListener : MonoBehaviour
    {
        [SerializeField] private string _contentId;
        [SerializeField] private List<GameObject> _enableWhenConditionFalse;
        [SerializeField] private List<GameObject> _disableWhenConditionFalse;
        [SerializeField] private bool _animateOnUnlock;
        [SerializeField, HideIf("@this._animateOnUnlock == false")] private TweenSettings<Vector3> _unlockSettings;
        [SerializeField] private bool _showCondition;
        [SerializeField, HideIf("@this._showCondition == false")]
        private BaseButton _showConditionButton;
        
        [Inject] private ContentService _contentService;
        [Inject] private MessageBuffer _messageBuffer;
        
        private Contract.IUnlockable _unlockable;

        private void Start()
        {
            if (!_contentService.IsInitialized)
            {
                _contentService.OnInitialized += OnServiceInitialized; 
                return;
            }
            Initialize();
        }

        private void OnServiceInitialized()
        {
            _contentService.OnInitialized -= OnServiceInitialized;
            Initialize();
        }

        private void Initialize()
        {
            if (_contentId.IsNullOrWhitespace())
            {
                foreach (var obj in _enableWhenConditionFalse) 
                    obj.SetActive(false);
                foreach (var obj in _disableWhenConditionFalse) 
                    obj.SetActive(true);
                return;
            }
            
            _unlockable = _contentService.GetById(_contentId);
            if (_unlockable == null)
            {
                Debug.LogError($"Can not find UIContent by Id {_contentId}");
                return;
            }

            if (_showCondition) 
                _showConditionButton.OnClick += OnShowConditionClick;
            
            if(!_unlockable.IsUnlocked)
                _unlockable.OnUnlocked += OnUnlock;
            Redraw(false);
        }

        private void OnUnlock(Contract.IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlock;
            Redraw(true);
        }

        private void Redraw(bool animate)
        {
            foreach (var obj in _enableWhenConditionFalse) 
                obj.SetActive(!_unlockable.IsUnlocked);
            foreach (var obj in _disableWhenConditionFalse)
            {
                obj.SetActive(_unlockable.IsUnlocked);
                if (animate && _animateOnUnlock)
                {
                    obj.transform.localScale = Vector3.zero;
                    Tween.Scale(obj.transform, _unlockSettings);
                }
            } 
        }

        private void OnDestroy()
        {
            if (_showCondition) 
                _showConditionButton.OnClick -= OnShowConditionClick;
            if (_unlockable is { IsUnlocked: false }) 
                _unlockable.OnUnlocked -= OnUnlock;
        }

        private void OnShowConditionClick() => 
            _messageBuffer.Send(_unlockable.Condition);
    }
}
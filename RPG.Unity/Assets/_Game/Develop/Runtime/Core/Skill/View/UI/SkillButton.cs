using System;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using IUnlockable = PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract.IUnlockable;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.View.UI
{
    [DisallowMultipleComponent]
    public class SkillButton : BaseButton
    {
        [SerializeField, Required] private GameObject _emptyBack;
        [SerializeField, Required] private Image _skillImage;
        [SerializeField, Required] private Image _readyToUseImage;
        [SerializeField, Required] private Image _useImage;
        [SerializeField, Required] private Image _useProgression;
        [SerializeField, Required] private Image _cooldownImage;
        [SerializeField, Required] private Image _cooldownProgression;
        [SerializeField, Required] private GameObject _blockVisual;
        [SerializeField] private string _contentId;
        [SerializeField] private Color _coolDownColor;

        private IUnlockable _windowContent;
        private SkillSlot _slot;
        private Model.Skill _skill;
        private SkillItem _item;
        private Tween _fillTween;

        [Inject] private ContentService _contentService;
        [Inject] private MessageBuffer _messageBuffer;

        public event Action<SkillButton> OnSkillClick;
        public event Action OnEmptyClick;
        
        public bool IsEmpty => _skill == null;

        protected override void Click()
        {
            base.Click();
            if (_windowContent is { IsUnlocked: false })
            {
                _messageBuffer.Send(_windowContent.Condition);
                return;
            }

            if (!_slot.IsUnlocked)
            {
                _messageBuffer.Send(_slot.Condition);
                return;
            }

            if (IsEmpty)
            {
                OnEmptyClick?.Invoke();
                return;
            }
            OnSkillClick?.Invoke(this);
        }

        public void Setup(Model.Skill skill, SkillItem item)
        {
            _skill = skill;
            _item = item;
            _skillImage.gameObject.SetActive(true);
            _emptyBack.gameObject.SetActive(false);
            _skill.OnChangeState += Redraw;
            Redraw(_skill, _skill.State);
        }

        public void Clear()
        {
            if (_skill != null) 
                _skill.OnChangeState -= Redraw;
            _skill = null;
            _emptyBack.gameObject.SetActive(true);
            _skillImage.gameObject.SetActive(false);
            _readyToUseImage.gameObject.SetActive(false);
            _useImage.gameObject.SetActive(false);
            _useProgression.gameObject.SetActive(false);
            _cooldownImage.gameObject.SetActive(false);
            _cooldownProgression.gameObject.SetActive(false);
            _fillTween.Stop();
        }

        public void Initialize(SkillSlot slot)
        {
            _slot = slot;
            _windowContent = _contentService.GetById(_contentId);
            if (_windowContent != null) 
                _windowContent.OnUnlocked += OnWindowUnlocked;
            
            RedrawBlock();
            if (!_slot.IsUnlocked) 
                _slot.OnUnlocked += OnSlotUnlocked;
        }

        private void OnWindowUnlocked(IUnlockable unlockable)
        {
            _windowContent.OnUnlocked -= OnWindowUnlocked;
            RedrawBlock();
        }

        private void OnSlotUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnSlotUnlocked;
            RedrawBlock();
        }

        private void RedrawBlock() => 
            _blockVisual.SetActive(!_slot.IsUnlocked || _windowContent is { IsUnlocked: false });

        private void Redraw(Model.Skill skill, SkillState state)
        {
            switch (state)
            {
                case SkillState.ReadyToExecute:
                    _readyToUseImage.gameObject.SetActive(true);
                    _useImage.gameObject.SetActive(false);
                    _useProgression.gameObject.SetActive(false);
                    _cooldownImage.gameObject.SetActive(false);
                    _cooldownProgression.gameObject.SetActive(false);
                    
                    _skillImage.sprite = _item.Sprite;
                    break;
                case SkillState.Cooldown:
                    _readyToUseImage.gameObject.SetActive(false);
                    _useImage.gameObject.SetActive(false);
                    _useProgression.gameObject.SetActive(false);
                    _cooldownImage.gameObject.SetActive(true);
                    _cooldownProgression.gameObject.SetActive(true);
                    _cooldownProgression.fillAmount = 1;
                    _fillTween.Stop();
                    _fillTween = Tween.UIFillAmount(_cooldownProgression, 1, 0, _skill.GetCooldown(), Ease.Linear);
                    
                    _skillImage.sprite = _item.InactiveSprite;
                    break;
                case SkillState.Execute:
                    _readyToUseImage.gameObject.SetActive(false);
                    _useImage.gameObject.SetActive(true);
                    _useProgression.gameObject.SetActive(true);
                    _cooldownImage.gameObject.SetActive(false);
                    _cooldownProgression.gameObject.SetActive(false);
                    _cooldownProgression.fillAmount = 1;
                    _fillTween.Stop();
                    _fillTween = Tween.UIFillAmount(_useProgression, 1, 0, _skill.GetDuration(), Ease.Linear);
                    
                    _skillImage.sprite = _item.Sprite;
                    break;
            }
        }
    }
}
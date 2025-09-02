using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;
using IUnlockable = PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract.IUnlockable;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.View.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SkillItemCasterView : MonoBehaviour, IInitializable
    {
        [SerializeField] private List<SkillButton> _skillButtons;
        [SerializeField] private BaseButton _autoCastButton;
        [SerializeField] private List<GameObject> _autoCastTexts;
        [SerializeField] private GameObject _blockAutoCastObject;
        [SerializeField] private string _contentId;
        [SerializeField, Required] private GameObject _offAutoCast;
        [SerializeField, Required] private GameObject _onAutoCast;

        [Inject] private ItemSkillService _service;
        [Inject] private SkillInventory _skillInventory;
        [Inject] private ContentService _contentService;
        [Inject] private MessageBuffer _messageBuffer;

        private IUnlockable _unlockable;

        [Button]
        private void GetComponents() => 
            _skillButtons = GetComponentsInChildren<SkillButton>().ToList();

        void IInitializable.Initialize()
        {
            _unlockable = _contentService.GetById(_contentId);
            if (_unlockable is { IsUnlocked: false }) 
                _unlockable.OnUnlocked += OnWindowUnlocked;
            
            RedrawAutoCastBlock();
            
            ClearAll();
            var slotCount = _skillInventory.Slots.Count;

            if (slotCount != _skillButtons.Count)
            {
                Debug.LogError("SkillSlots.Count != SkillButtons.Count");
                return;
            }

            for (var i = 0; i < slotCount; i++) 
                _skillButtons[i].Initialize(_skillInventory.Slots[i]);

            foreach (var kvp in _service.SlotDictionary)
            {
                if(kvp.Key.Item == null) continue;
                Setup(kvp.Value, kvp.Key.Item, kvp.Key.Id);
            }
            
            _service.OnSkillEquippedToSlot += Setup;
            _service.OnSkillRemovedFromSlot += Clear;
            _autoCastButton.OnClick += OnAutoCastClick;
            foreach (var skillButton in _skillButtons)
            {
                skillButton.OnEmptyClick += OnEmptyClick;
                skillButton.OnSkillClick += OnSkillClick;  
            } 
            RedrawAutoCast();
        }

        private void OnDestroy()
        {
            _service.OnSkillEquippedToSlot -= Setup;
            _service.OnSkillRemovedFromSlot -= Clear;
            _autoCastButton.OnClick -= OnAutoCastClick;
            foreach (var skillButton in _skillButtons)
            {
                skillButton.OnEmptyClick -= OnEmptyClick;
                skillButton.OnSkillClick -= OnSkillClick;  
            } 
        }

        private void OnWindowUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnWindowUnlocked;
            RedrawAutoCastBlock();
        }
        
        private void RedrawAutoCastBlock()
        {
            var isAutoCastBlocked = _unlockable is { IsUnlocked: false };
            _blockAutoCastObject.SetActive(isAutoCastBlocked);
            foreach (var autoCastText in _autoCastTexts) 
                autoCastText.SetActive(!isAutoCastBlocked);
        }

        private void OnSkillClick(SkillButton button) => 
            _service.LaunchSkill(_skillButtons.IndexOf(button));

        private void OnEmptyClick() => 
            _service.InteractEmptySlot();

        private void OnAutoCastClick()
        {
            if (_unlockable is { IsUnlocked: false })
            {
                _messageBuffer.Send(_unlockable.Condition);
                return;
            }
            _service.ToggleAutoCast();
            RedrawAutoCast();
        }

        private void ClearAll()
        {
            foreach (var skillButton in _skillButtons) 
                skillButton.Clear();
        }

        private void Setup(Model.Skill skill, SkillItem item, int id)
        {
            var button = GetButton(id);
            button.Setup(skill, item);
        }

        private void Clear(int id)
        {
            var button = GetButton(id);
            button.Clear();
        }

        private void RedrawAutoCast()
        {
            if (_service.IsAutoCastActive) 
                EnableAutoCast();
            else
                DisableAutoCast();
        }
        
        private void EnableAutoCast()
        {
            _onAutoCast.gameObject.SetActive(true);
            _offAutoCast.gameObject.SetActive(false);
        }

        private void DisableAutoCast()
        {
            _onAutoCast.gameObject.SetActive(false);
            _offAutoCast.gameObject.SetActive(true);
        }

        private SkillButton GetButton(int id)
        {
            if (id >= _skillButtons.Count)
            {
                Debug.LogError("Id out of range");
                return null;
            }

            return _skillButtons[id];
        }
    }
}
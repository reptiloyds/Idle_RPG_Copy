using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Save;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.Skill.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model
{
    public class ItemSkillService : BaseSkillService
    {
        [Inject] private SkillInventory _inventory;
        [Inject] private SkillDataProvider _dataProvider;
        
        private readonly Dictionary<SkillItem, Skill> _itemDictionary = new ();
        private readonly Dictionary<SkillSlot, Skill> _slotDictionary = new ();
        private SkillDataContainer _data;

        public IReadOnlyDictionary<SkillSlot, Skill> SlotDictionary => _slotDictionary;
        public bool IsAutoCastActive => _data.IsAutoCastEnabled;
        public event Action<Skill, SkillItem, int> OnSkillEquippedToSlot;
        public event Action<int> OnSkillRemovedFromSlot;
        public event Action OnEmptySlotInteracted;
        
        [Preserve]
        public ItemSkillService()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            _data = _dataProvider.GetData();
            
            foreach (var item in _inventory.Items)
            {
                var skill = SkillFactory.Create(Sheet[item.SkillId], () => item.Level);
                _itemDictionary.Add(item, skill);
            }
            
            foreach (var slot in _inventory.Slots)
            {
                var item = slot.Item;
                if (item != null)
                    Enable(slot, item);
            }
            
            _inventory.OnEquipped += Enable;
            _inventory.OnRemoved += Disable;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _inventory.OnEquipped -= Enable;
            _inventory.OnRemoved -= Disable;
        }

        public void LaunchSkill(int index)
        {
            foreach (var slot in _inventory.Slots)
            {
                if(slot.Id != index) continue;
                if (slot.Item == null) return;
                var skill = _slotDictionary[slot];
                if(skill.State == SkillState.ReadyToExecute)
                    skill.Execute();
                return;
            }
        }

        public void InteractEmptySlot() => 
            OnEmptySlotInteracted?.Invoke();

        public Skill GetSkillByItem(SkillItem skillItem) => 
            _itemDictionary.GetValueOrDefault(skillItem);

        public override void StopSkills()
        {
            foreach (var pair in _slotDictionary)
                if (pair.Key.Item != null) 
                    pair.Value.Stop();
        }

        public void ToggleAutoCast() => 
            _data.IsAutoCastEnabled = !_data.IsAutoCastEnabled;

        public override void ResetAllSkill()
        {
            //StopSkills();
            foreach (var pair in _slotDictionary)
                if(pair.Key.Item != null)
                    pair.Value.ReadyToExecute();
        }

        private void Enable(SkillSlot slot, SkillItem item)
        {
            var model = _itemDictionary[item];
            _slotDictionary[slot] = model;
            
            OnSkillEquippedToSlot?.Invoke(model, item, slot.Id);
            
            model.Cooldown();
            ActiveSkills.Add(model);
        }

        private void Disable(SkillSlot slot, SkillItem item)
        {
            if (!_slotDictionary.TryGetValue(slot, out var skill)) return;
            
            skill.Stop();
            ActiveSkills.Remove(skill);
 
            _slotDictionary[slot] = null;
            OnSkillRemovedFromSlot?.Invoke(slot.Id);
        }

        protected override void AutoCast()
        {
            if(!IsAutoCastActive) return;
            foreach (var pair in _slotDictionary)
            {
                if (pair.Key.Item == null || pair.Value.State != SkillState.ReadyToExecute) continue;
                pair.Value.Execute();
                return;
            }
        }
    }
}
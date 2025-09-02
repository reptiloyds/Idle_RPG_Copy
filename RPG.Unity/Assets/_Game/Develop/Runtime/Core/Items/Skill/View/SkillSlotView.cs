using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SkillSlotView : SlotView<SkillSlot>
    {
        [SerializeField, Required, FoldoutGroup("Accent")] private List<GameObject> _accentObjects;

        public void EnableAccent()
        {
            if(!Slot.IsUnlocked) return;

            foreach (var accentObject in _accentObjects) 
                accentObject.SetActive(true);
        }

        public void DisableAccent()
        {
            foreach (var accentObject in _accentObjects) 
                accentObject.SetActive(false);
        }
    }
}
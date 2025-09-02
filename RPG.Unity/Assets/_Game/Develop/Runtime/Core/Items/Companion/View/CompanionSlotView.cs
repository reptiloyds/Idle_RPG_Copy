using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Companion.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CompanionSlotView : SlotView<CompanionSlot>
    {
        [SerializeField, Required, FoldoutGroup("Accent")] private Color _lightColor;
        [SerializeField, Required, FoldoutGroup("Accent")] private List<Image> _lightImages;
        [SerializeField, Required, FoldoutGroup("Accent")] private List<GameObject> _accentObjects;

        private readonly List<(Image image, Color originalColor)> _colorList = new();

        public void EnableAccent()
        {
            if(!Slot.IsUnlocked) return;
            _colorList.Clear();
            foreach (var image in _lightImages)
            {
                _colorList.Add((image, image.color));
                image.color = new Color(_lightColor.r, _lightColor.g, _lightColor.b, image.color.a);
            }

            foreach (var accentObject in _accentObjects) 
                accentObject.SetActive(true);
        }

        public void DisableAccent()
        {
            foreach (var tuple in _colorList) 
                tuple.image.color = tuple.originalColor;
            
            foreach (var accentObject in _accentObjects) 
                accentObject.SetActive(false);
        }
    }
}
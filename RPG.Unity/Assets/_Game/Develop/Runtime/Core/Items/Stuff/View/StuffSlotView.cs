using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class StuffSlotView : SlotView<StuffSlot>
    {
        [SerializeField, Required, BoxGroup("Main")] private Image _emptySlotImage;
        [SerializeField, Required, BoxGroup("Main")] private SlotEquippedImage _slotEquippedImage;

        protected override void ShowItem()
        {
            base.ShowItem();
            _slotEquippedImage.Redraw(Slot.Type.EquippedSlotImage, Slot.Type.EquippedSlotColor);
            _slotEquippedImage.Enable();
        }

        protected override void HideItem()
        {
            base.HideItem();
            _slotEquippedImage.Disable();
            _emptySlotImage.sprite = Slot.Type.EmptySlotImage;
        }
    }
}
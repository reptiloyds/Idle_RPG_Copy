using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.InfinityScrolling
{
    public class ExtendedScrollRect : ScrollRect
    {
        public bool IsDragging { get; private set; }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            IsDragging = true;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            IsDragging = false;
        }
    }
}
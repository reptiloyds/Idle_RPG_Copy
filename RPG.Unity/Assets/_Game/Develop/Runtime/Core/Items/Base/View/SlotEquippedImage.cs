using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SlotEquippedImage : MonoBehaviour
    {
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Image _background;

        public void Redraw(Sprite sprite, Color color)
        {
            _image.sprite = sprite;
            _background.color = color;
        }
        
        public void Enable() => 
            gameObject.SetActive(true);

        public void Disable() => 
            gameObject.SetActive(false);
    }
}
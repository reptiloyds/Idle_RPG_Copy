using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View.Enhance
{
    [DisallowMultipleComponent, HideMonoScript]
    public class EnhanceItemView : MonoBehaviour
    {
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Image _rarityImage;
        [SerializeField, Required] private TextMeshProUGUI _previousLevelText;
        [SerializeField, Required] private TextMeshProUGUI _currentLevelText;
        
        private Item _item;

        public void SetModel(Item item)
        {
            _item = item;
            _image.sprite = _item.Sprite;
            _rarityImage.color = _item.RarityColor;
            _previousLevelText.SetText(_item.Level.ToString());
        }

        public void DrawCurrentLevel() => 
            _currentLevelText.SetText(_item.Level.ToString());

        public void Clear() => 
            _item = null;

        public void Enable() => 
            gameObject.SetActive(true);

        public void Disable() => 
            gameObject.SetActive(false);
    }
}
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CollectionItemView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _backImage;
        [SerializeField] private GameObject _blockObject;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Color _defaltTextColor;
        [SerializeField] private Color _notEnoughTextColor;

        private int _requestedLevel;
        private Item _item;

        private void OnDestroy()
        {
            if (_item != null)
            {
                _item.OnLevelUp -= OnItemLevelUp;
                _item.OnUnlock -= OnItemUnlock;
            }
        }

        public void Setup(Item item)
        {
            _item = item;
            _item.OnLevelUp += OnItemLevelUp;
            _item.OnUnlock += OnItemUnlock;
            
            _image.sprite = item.Sprite;
            _backImage.color = item.RarityColor;
        }

        public void SetRequestedLevel(int requestedLevel)
        {
            _requestedLevel = requestedLevel;
            Redraw();
        }

        private void Redraw()
        {
            var itemLevel = _item.IsUnlocked ? _item.Level : 0;
            var isLevelEnough = itemLevel >= _requestedLevel;
            _levelText.color = isLevelEnough ? _defaltTextColor : _notEnoughTextColor;
            _blockObject.SetActive(!isLevelEnough);
            _levelText.SetText($"{TranslationConst.LevelPrefixCaps} {itemLevel}/{_requestedLevel}");
        }

        private void OnItemUnlock(Item item) => 
            Redraw();

        private void OnItemLevelUp(Item item) => 
            Redraw();
    }
}
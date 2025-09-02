using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ItemInfoView : MonoBehaviour
    {
        [SerializeField, Required] private Image _itemImage;
        [SerializeField, Required] private Image _rarityImage;
        [SerializeField, Required] private GameObject _block;
        [SerializeField, Required] private TextMeshProUGUI _itemName;
        [SerializeField, Required] private TextMeshProUGUI _rarityText;
        [SerializeField, Required] private LevelProgressionView _levelProgressionView;
        
        [Inject] private ITranslator _translator;

        public void Draw(Item item)
        {
            _block.SetActive(!item.IsUnlocked);
            _itemImage.sprite = item.Sprite;
            _rarityImage.color = item.RarityColor;
            _rarityText.color = item.RarityColor;
            _rarityText.SetText(item.LocalizedRarity);
            _itemName.SetText(item.LocalizedName);
            _levelProgressionView.Redraw(item.Level, item.IsLevelMax, item.Amount, item.TargetAmount);
        }
    }
}
using PleasantlyGames.RPG.Runtime.Core.Tag;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Stuff.Tag
{
    [CreateAssetMenu(fileName = nameof(StuffSlotType), menuName = "SO/Tag/" + nameof(StuffSlotType))]
    public class StuffSlotType : UniqueTag
    {
        [SerializeField] private Sprite _emptySlotImage;
        [SerializeField] private Sprite _equippedSlotImage;
        [SerializeField] private Color _equippedSlotColor;
        [SerializeField] private string _localizationToken;
        
        public Sprite EmptySlotImage => _emptySlotImage;
        public Sprite EquippedSlotImage => _equippedSlotImage;
        public Color EquippedSlotColor => _equippedSlotColor;
        public string LocalizationToken => _localizationToken;
    }
}
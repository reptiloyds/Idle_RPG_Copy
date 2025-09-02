using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Views
{
    public class PiggyBankUIElementsContainer : MonoBehaviour
    {
        [SerializeField, Required] private TMP_Text _progressHardLimitText;
        [SerializeField, Required] private TMP_Text _progressHalfHardLimitText;
        [SerializeField, Required] private TMP_Text _levelText;
        [SerializeField, Required] private TMP_Text _nextLevelText;
        [SerializeField, Required] private TMP_Text _levelHintText;
        [SerializeField, Required] private TMP_Text _nextHardLimitText;
        [SerializeField, Required] private TMP_Text _hardLimitHintText;
        [SerializeField, Required] private TMP_Text _currentHardText;
        [SerializeField, Required] private Slider _progressSlider;
        [SerializeField, Required] private BaseButton _buyButton;
        [SerializeField, Required] private BaseButton _guideButton;
        [SerializeField, Required] private Image _icon;
        [SerializeField, Required] private RectTransform _bonusesParent;
        [SerializeField, Required] private Slider _bonusSlider;
        [SerializeField, Required] private NextStatUIContainer _nextLevelImage;
        [SerializeField, Required] private NextStatUIContainer _nextLimitImage;
        [SerializeField, Required] private Transform _levelArrow;
        [SerializeField, Required] private Transform _limitArrow;
        [SerializeField, Required] private TMP_Text _priceText;

        public TMP_Text ProgressHardLimitText => _progressHardLimitText;
        public TMP_Text ProgressHalfHardLimitText => _progressHalfHardLimitText;
        public TMP_Text LevelText => _levelText;
        public TMP_Text NextLevelText => _nextLevelText;
        public TMP_Text LevelHintText => _levelHintText;
        public TMP_Text NextHardLimitText => _nextHardLimitText;
        public TMP_Text HardLimitHintText => _hardLimitHintText;
        public TMP_Text CurrentHardText => _currentHardText;
        public Slider ProgressSlider => _progressSlider;
        public BaseButton BuyButton => _buyButton;
        public BaseButton GuideButton => _guideButton;
        public Image Icon => _icon;
        public RectTransform BonusesParent => _bonusesParent;
        public Slider BonusSlider => _bonusSlider;
        public NextStatUIContainer NextLevelImage => _nextLevelImage;
        public NextStatUIContainer NextLimitImage => _nextLimitImage;
        public Transform LevelArrow => _levelArrow;
        public TMP_Text PriceText => _priceText;
    }
}
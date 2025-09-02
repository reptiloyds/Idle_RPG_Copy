using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Views
{
    public class MaxLevelView : MonoBehaviour
    {
        [SerializeField, Required] private TMP_Text _levelHintText;
        [SerializeField, Required] private TMP_Text _hardLimitHintText;
        
        public TMP_Text LevelHintText => _levelHintText;
        public TMP_Text HardLimitHintText => _hardLimitHintText;
    }
}
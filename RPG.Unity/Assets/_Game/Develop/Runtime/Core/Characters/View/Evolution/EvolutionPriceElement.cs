using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View.Evolution
{
    [DisallowMultipleComponent, HideMonoScript]
    public class EvolutionPriceElement : MonoBehaviour
    {
        [SerializeField] private Image _resourceImage;
        [SerializeField] private TextMeshProUGUI _amountText;

        public void Setup(Sprite sprite, BigDouble.Runtime.BigDouble amount)
        {
            _resourceImage.sprite = sprite;
            _amountText.SetText(StringExtension.Instance.CutBigDouble(amount, true));
        }

        public void Enable() => 
            gameObject.SetActive(true);

        public void Disable() => 
            gameObject.SetActive(false);
    }
}
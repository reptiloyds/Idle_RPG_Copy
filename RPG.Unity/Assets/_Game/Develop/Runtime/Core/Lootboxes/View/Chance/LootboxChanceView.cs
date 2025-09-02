using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.View.Chance
{
    [DisallowMultipleComponent, HideMonoScript]
    public class LootboxChanceView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rarityText;
        [SerializeField] private TextMeshProUGUI _chanceText;
        [SerializeField] private List<Graphic> _colorfulGraphics;

        public void Setup(string rarityText, string chanceText, Color color)
        {
            _rarityText.SetText(rarityText);
            _chanceText.SetText($"{chanceText}%");
            
            foreach (var graphic in _colorfulGraphics) 
                graphic.color = color;
        }

        public void Enable() => 
            gameObject.SetActive(true);

        public void Disable() => 
            gameObject.SetActive(false);
    }
}
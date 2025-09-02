using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ItemModifierView : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI _modifierText;
        [SerializeField, Required] private GameObject _deltaObject;
        [SerializeField, Required] private TextMeshProUGUI _deltaText;
        [SerializeField, Required] private Image _deltaImage;
        [SerializeField] private Vector3 _positiveDeltaRotation;
        [SerializeField] private Vector3 _negativeDeltaRotation;
        [SerializeField] private Color _bonusColor;
        [SerializeField] private Color _positiveDeltaColor;
        [SerializeField] private Color _negativeDeltaColor;
        
        [Inject] private ITranslator _translator;

        private void Awake() => 
            HideDelta();

        public void Draw(UnitStatType statType, StatModifier statModifier)
        {
            _modifierText.SetText($"{_translator.Translate(statType.ToString())} <color=#{ColorUtility.ToHtmlStringRGB(_bonusColor)}>{statModifier.Type.GetFormattedModifier(statModifier.Value)}</color>");
        }

        public void DrawDelta(StatModifier origin, StatModifier other)
        {
            var delta = origin.Value - (other?.Value ?? 0);
            if (delta == 0) return;
            _deltaObject.SetActive(true);

            if (delta > 0)
            {
                _deltaImage.transform.localRotation = Quaternion.Euler(_positiveDeltaRotation);
                _deltaImage.color = _positiveDeltaColor;
            }
            else
            {
                _deltaImage.transform.localRotation = Quaternion.Euler(_negativeDeltaRotation);
                _deltaImage.color = _negativeDeltaColor;
            }
            
            var sign = delta > 0 ? "+" : "";
            _deltaText.SetText($"{sign}{StringExtension.Instance.CutBigDouble(delta * 100)}%");
            _deltaText.color = delta > 0 ? _positiveDeltaColor : _negativeDeltaColor;
        }

        public void HideDelta() => 
            _deltaObject.SetActive(false);
    }
}
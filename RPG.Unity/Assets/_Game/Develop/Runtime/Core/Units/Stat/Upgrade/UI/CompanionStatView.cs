using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CompanionStatView : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI _name;
        [SerializeField, Required] private TextMeshProUGUI _value;
        [SerializeField, Required] private Image _image;

        [Inject] private ITranslator _translator;
        [Inject] private UnitStatService _statService;
        
        private UnitStat _stat;

        private string _damageStatString;
        
        private void Awake() => 
            _damageStatString = UnitStatType.Damage.ToString();

        public void Setup(UnitStat stat)
        {
            _stat = stat;
            _stat.OnValueChanged += OnValueChanged;
            _image.sprite = stat.Sprite;
            RedrawName();
            RedrawValue();
        }

        private void OnValueChanged() =>
            RedrawValue();

        public void Clear()
        {
            if (_stat == null) return;
            _stat.OnValueChanged -= OnValueChanged;
            _stat = null;
        }

        private void RedrawName()
        {
            if (_stat.Type == UnitStatType.DamagePercent) 
                _name.SetText(_translator.Translate(_damageStatString));
            else
                _name.SetText(_translator.Translate(_stat.TypeString));
        }

        private void RedrawValue()
        {
            if (_stat.Type == UnitStatType.DamagePercent)
            {
                var playerDamage = _statService.GetPlayerStat(UnitStatType.Damage);
                _value.SetText(StringExtension.Instance.CutBigDouble(playerDamage.Value * (1 + _stat.Value)));
            }
            else
                _value.SetText(StringExtension.Instance.CutBigDouble(_stat.Value));
        }

        private void OnDestroy() => 
            Clear();
    }
}
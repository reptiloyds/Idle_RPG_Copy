using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PrimeTween;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Health
{
    public class OwnHealth : UnitHealth
    {
        private readonly UnitView _unitView;

        private UnitStat _healthStat;
        private UnitStat _healthRegenStat;
        private UnitStat _healthSpeedStat;
        private Sequence _regenSequence;

        public OwnHealth(HealthBarFactory healthBarFactory, UnitView unitView) : base(healthBarFactory) => 
            _unitView = unitView;

        public override void Initialize()
        {
            base.Initialize();

            _healthStat = _unitView.GetStat(UnitStatType.Health);
            _healthStat.OnValueChanged += UpdateMaxHealth;
            UpdateMaxHealth();
            ResetHealth();

            _healthRegenStat = _unitView.GetStat(UnitStatType.HealthRegenAmount);
            _healthSpeedStat = _unitView.GetStat(UnitStatType.HealthRegenSpeed);

            if (_healthRegenStat != null && _healthSpeedStat != null)
            {
                _healthRegenStat.SetModifierReference(_healthStat);
                _regenSequence = Sequence.Create(-1);
                _regenSequence.Chain(Tween.Delay((float)_healthSpeedStat.Value.ToDouble(), Regen));
            }

            Bar = HealthBarFactory.Create(_unitView.transform, _unitView.HealthBarPoint.localPosition, this);
        }
        
        private void Regen() => 
            ApplyHeal(_healthRegenStat.Value);

        private void UpdateMaxHealth() => 
            SetNewMaxValue(_healthStat.Value);

        public override void Dispose()
        {
            base.Dispose();
            
            HealthBarFactory.Dispose(Bar);
            
            _regenSequence.Stop();
            _healthStat.OnValueChanged -= UpdateMaxHealth;
            _healthStat = null;
            _healthRegenStat = null;
            _healthSpeedStat = null;
        }
    }
}
using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model.Effects;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Save;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using R3;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model
{
    public class Blessing : BaseBlessing
    {
        private readonly BlessingSheet.Row _config;
        private readonly SerialDisposable _durationDisposable = new();
        private readonly SwitchableBlessingData _switchableData;
        private readonly ReactiveProperty<float> _duration = new();
        private readonly ReactiveProperty<int> _freeActivation = new();
        private readonly CompositeDisposable _compositeDisposable = new();

        public event Action<Blessing> OnEnabled;
        public event Action<Blessing> OnDisabled;
        
        public bool IsIncreaseEffect => _config.Increase;
        public ReadOnlyReactiveProperty<float> Duration => _duration;
        public ReadOnlyReactiveProperty<int> FreeActivation => _freeActivation;

        public Blessing(BlessingSheet.Row config, SwitchableBlessingData data, List<BlessingEffect> effects) :
            base(config, data, effects, config.MaxLevel)
        {
            _switchableData = data;
            _config = config;
            if (_switchableData.ActivationAmount == 0) 
                _switchableData.FreeActivations = _config.FreeActivation;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            _freeActivation.Value = _switchableData.FreeActivations;
            _freeActivation
                .Subscribe(value => _switchableData.FreeActivations = value)
                .AddTo(_compositeDisposable);

            _isActive.Value = (Time.Now() - Data.LastActivationTime).TotalSeconds < _config.DurationInMinute * 60;
            if (IsActive.CurrentValue) 
                Enable();
        }

        protected override BaseValueFormula GetLevelProgression() => 
            _config.LevelFormula.CreateFormula(_config.LevelFormulaJSON);

        public override void Enable()
        {
            if (!IsActive.CurrentValue) 
                _switchableData.ActivationAmount++;
            base.Enable();
            
            if (_freeActivation.Value > 0)
                _freeActivation.Value--;

            var endTime = Data.LastActivationTime + TimeSpan.FromMinutes(_config.DurationInMinute);
            _duration.Value = (int)(endTime - Time.Now()).TotalSeconds;
            
            Time.LaunchGlobalTimer(_durationDisposable, _duration, TimeSpan.FromSeconds(1), completeCallback: Disable);
            
            OnEnabled?.Invoke(this);
        }

        public override void Disable()
        {
            base.Disable();
            _durationDisposable.Disposable?.Dispose();
            OnDisabled?.Invoke(this);
        }

        public override void Dispose()
        {
            base.Dispose();
            _compositeDisposable.Dispose();
            _durationDisposable.Dispose();
        }

        public string GetBonusValue()
        {
            if (_effects.Count == 0)
                return string.Empty;

            var statModifier = _effects[0].StatModifier;
            return statModifier.Type.GetFormattedModifier(statModifier.Value, false);
        }

        public string GetDuration() => 
            $"{_config.DurationInMinute} {Translator.Translate(TranslationConst.ShortMinute)}";
    }
}
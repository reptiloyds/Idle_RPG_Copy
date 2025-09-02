using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model.Effects;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Save;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using R3;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model
{
    public abstract class BaseBlessing : IDisposable
    {
        private readonly BlessingRow _baseConfig;
        protected readonly List<BlessingEffect> _effects;
        private readonly int _maxLevel;
        private BaseValueFormula _levelProgression;

        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] protected ITranslator Translator;
        [Inject] protected readonly TimeService Time;

        protected readonly BlessingData Data;
        protected readonly ReactiveProperty<bool> _isActive = new();
        
        public IReadOnlyList<BlessingEffect> Effects => _effects;
        public int Level => Data.Level;
        public int Progression => Data.Progression;
        public int TargetProgression { get; private set; }

        public ReadOnlyReactiveProperty<bool> IsActive => _isActive;
        public bool IsLevelMax => Data.Level == _maxLevel;
        public string Name { get; private set; }
        public string BonusDescription { get; private set; }
        public Sprite Sprite { get; private set; }

        public event Action OnProgressionChanged;
        public event Action<BaseBlessing> OnLevelUpped;

        protected BaseBlessing(BlessingRow baseConfig, BlessingData data, List<BlessingEffect> effects, int maxLevel)
        {
            Data = data;
            _effects = effects;
            _maxLevel = maxLevel;
            _baseConfig = baseConfig;
        }

        public virtual void Initialize()
        {
            Name = Translator.Translate(_baseConfig.LocalizationToken);
            BonusDescription = Translator.Translate($"{_baseConfig.LocalizationToken}_des");
            Sprite = _spriteProvider.GetSprite(Asset.MainAtlas, _baseConfig.ImageName);

            _levelProgression = GetLevelProgression();
            UpdateProgressionTarget();
            UpdateLevelForEffects();
        }

        public virtual void Enable()
        {
            foreach (var effect in _effects)
                effect.Enable();
            if (_isActive.Value) return;
            
            Data.LastActivationTime = Time.Now();
            _isActive.Value = true;
        }

        public virtual void Disable()
        {
            foreach (var effect in _effects)
                effect.Disable();
            _isActive.Value = false;
        }

        public void AppendProgression()
        {
            Data.Progression++;
            if (Progression >= TargetProgression)
            {
                Data.Progression = Progression - TargetProgression;
                LevelUp();
            }
            
            OnProgressionChanged?.Invoke();
        }
        
        protected virtual void LevelUp()
        {
            if (IsLevelMax) return;
            Data.Level++;
            UpdateProgressionTarget();
            UpdateLevelForEffects();
            OnLevelUpped?.Invoke(this);
        }

        private void UpdateProgressionTarget() =>
            TargetProgression = Mathf.CeilToInt((float)_levelProgression.CalculateBigDouble(Level).ToDouble());

        private void UpdateLevelForEffects()
        {
            foreach (var effect in _effects)
                effect.SetLevel(Data.Level);
        }

        protected abstract BaseValueFormula GetLevelProgression();

        public virtual void Dispose()
        {
        }
    }
}
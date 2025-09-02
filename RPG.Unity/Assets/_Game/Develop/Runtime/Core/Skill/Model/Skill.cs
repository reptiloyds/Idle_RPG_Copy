using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Skill.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PrimeTween;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model
{
    public class Skill
    {
        private readonly SkillRow _config;
        private readonly Dictionary<string, Func<string>> _descriptionDictionary = new();
        private readonly List<BaseSkillEffect> _effects = new();
        private readonly Func<int> _levelSource;
        private Sequence _executionSequence;
        private Tween _useTween, _cooldownTween;
        private SkillState _state;
        private GlobalStat _decreaseCooldownStat;
        
        private const string DescriptionPostfix = "des";
        private const char LocalizationSeparator = '@';
        
        [Inject] private ITranslator _translator;
        [Inject] private GlobalStatProvider _globalStatProvider;

        public SkillState State => _state;
        public string NameId => _config.Id;
        public event Action<Skill, SkillState> OnChangeState;

        public Skill(SkillRow config, Func<int> levelSource = null)
        {
            _config = config;
            _levelSource = levelSource ?? (() => 1);
        }

        public void Initialize()
        {
            _decreaseCooldownStat = _globalStatProvider.GetStat(GlobalStatType.DecreaseSkillCooldown);

            foreach (var effect in _effects)
            {
                effect.Initialize(_levelSource);
                var descriptionTuples = effect.GetDescriptions();
                foreach (var descriptionTuple in descriptionTuples)
                    _descriptionDictionary.Add($"{descriptionTuple.valueType}", descriptionTuple.variableGetter);
                
                effect.OnComplete += OnEffectComplete;
            }
        }

        private void OnEffectComplete()
        {
            foreach (var effect in _effects)
                if(!effect.IsCompleted) return;
            
            _useTween.Stop();
            Stop();
            Cooldown();
        }

        public void Execute()
        {
            _executionSequence = Sequence.Create();
            foreach (var effect in _effects)
            {
                var delay = effect.GetDelay();
                if (delay <= 0) 
                    effect.Execute();
                else
                    _executionSequence.Chain(Tween.Delay(delay, effect.Execute));
            }

            SetState(SkillState.Execute);
            var duration = GetDuration();
            if (duration > 0)
                _useTween = Tween.Delay(duration, Cooldown);
            else
                SetState(SkillState.Cooldown);
        }

        public void Cooldown()
        {
            var cooldown = GetCooldown();
            SetState(SkillState.Cooldown);
            _cooldownTween.Stop();
            if(cooldown > 0)
                _cooldownTween = Tween.Delay(cooldown, ReadyToExecute);
            else
                ReadyToExecute();
        }

        public void ReadyToExecute()
        {
            _useTween.Stop();
            _cooldownTween.Stop();
            SetState(SkillState.ReadyToExecute);
        }

        public void Stop()
        {
            _useTween.Stop();
            _cooldownTween.Stop();
            _executionSequence.Stop();
            foreach (var effect in _effects)
            {
                if(effect.IsActive)
                    effect.Stop();  
            } 
            SetState(SkillState.Stop);
        }

        public void AddEffect(BaseSkillEffect effect) => 
            _effects.Add(effect);

        public float GetCooldown()
        {
            var decreaseK = Mathf.Clamp01(2 - (float)_decreaseCooldownStat.Value.ToDouble());
            return _config.Cooldown * decreaseK;
        }

        public float GetDuration()
        {
            float maxDuration = 0;

            foreach (var effect in _effects)
            {
                var duration = effect.GetTotalDuration();
                if (duration > maxDuration)
                    maxDuration = duration;
            }

            return maxDuration;
        }

        public string GetDescription()
        {
            var localizedDescription = _translator.Translate($"{_config.Id}_{DescriptionPostfix}");
            return FillDescription(localizedDescription, LocalizationSeparator);
        }

        private void SetState(SkillState state)
        {
            _state = state;
            OnChangeState?.Invoke(this, state);
        }

        private string FillDescription(string description, char separator)
        {
            string pattern = $@"{separator}(\w+){separator}";

            string result = Regex.Replace(description, pattern, match =>
            {
                string key = match.Groups[1].Value;
                return _descriptionDictionary.TryGetValue(key, out var value) ? value.Invoke() : key;
            });

            return result;
        }

        public void Tick()
        {
            foreach (var effect in _effects)
            {
                if(effect.IsActive)
                    effect.Tick();  
            } 
        }

        public void FixedTick()
        {
            foreach (var effect in _effects)
            {
                if(effect.IsActive)
                    effect.FixedTick();
            }
        }
    }
}
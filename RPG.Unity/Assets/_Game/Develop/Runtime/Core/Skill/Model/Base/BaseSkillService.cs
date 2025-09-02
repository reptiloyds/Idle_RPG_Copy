using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Definition;
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Base
{
    public abstract class BaseSkillService : ITickable, IFixedTickable, IDisposable
    {
        [Inject] protected SkillFactory SkillFactory;
        [Inject] private LocationUnitCollection _locationUnits;
        [Inject] private BalanceContainer _balance;
        [Inject] private SkillConfiguration _configuration;

        protected SkillSheet Sheet;
        
        private float _autoCastTimer;
        private bool _hasEnemies = false;
        
        private bool _isAutoCastBlocked = true;

        protected readonly List<Skill> ActiveSkills = new();
        
        [Inject]
        private void Construct() => 
            Sheet = _balance.Get<SkillSheet>();

        public virtual void Initialize() => 
            _locationUnits.OnChanged += OnLocationUnitsChanged;

        public virtual void Dispose() => 
            _locationUnits.OnChanged -= OnLocationUnitsChanged;

        private void OnLocationUnitsChanged()
        {
            foreach (var unit in _locationUnits.Units)
            {
                if (unit.TeamType != TeamType.Enemy) continue;
                _hasEnemies = true;
                return;
            }

            _hasEnemies = false;
        }
        
        public void BlockAutoCast() => 
            _isAutoCastBlocked = true;

        public void UnlockAutoCast() => 
            _isAutoCastBlocked = false;

        public abstract void ResetAllSkill();

        public abstract void StopSkills();

        public virtual void FixedTick()
        {
            for (var i = 0; i < ActiveSkills.Count; i++) 
                ActiveSkills[i].FixedTick();
        }

        public virtual void Tick()
        {
            for (var i = 0; i < ActiveSkills.Count; i++) 
                ActiveSkills[i].Tick();
            
            if(!_hasEnemies) return;
            _autoCastTimer -= Time.unscaledDeltaTime;
            if (_autoCastTimer > 0) return;
            
            _autoCastTimer = _configuration.AutoCastDelay;
            if (!_isAutoCastBlocked)
                AutoCast();
        }

        protected abstract void AutoCast();
    }
}
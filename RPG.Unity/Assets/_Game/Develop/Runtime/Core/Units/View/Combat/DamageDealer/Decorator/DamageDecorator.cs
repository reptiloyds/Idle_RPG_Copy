using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer.Decorator
{
    public abstract class DamageDecorator : IDamageProvider, IDisposable
    {
        protected readonly IDamageProvider WrappedEntity;
        protected readonly UnitView UnitView;
        private readonly List<UnitStat> _stats = new();

        protected bool IsReady { get; private set; }

        protected DamageDecorator(UnitView unitView, IDamageProvider wrappedEntity)
        {
            WrappedEntity = wrappedEntity;
            UnitView = unitView;
        }

        public void Enable()
        {
            WrappedEntity.Enable();
        }

        public void Disable()
        {
            WrappedEntity.Disable();
        }

        public BigDouble.Runtime.BigDouble GetDamage(UnitView targetUnitView) => 
            GetDamageInternal(targetUnitView);

        protected abstract BigDouble.Runtime.BigDouble GetDamageInternal(UnitView targetUnitView);

        public (Color color, int priority) GetColor() => 
            GetColorInternal();

        protected virtual (Color color, int priority) GetColorInternal() => 
            WrappedEntity.GetColor();

        protected void AppendStat(UnitStat stat)
        {
            if(stat is { IsUnlocked: true }) return;
            
            if(stat != null) 
                stat.OnLevelUp += UpdateReadyStatus;
            
            _stats.Add(stat);
        }

        protected void UpdateReadyStatus()
        {
            var result = true;

            foreach (var stat in _stats)
            {
                if (stat == null)
                {
                    result = false;
                    break;
                }
                if (stat.IsUnlocked) continue;
                
                result = false;
                break;
            }

            IsReady = result;
        }

        protected void ClearStats()
        {
            foreach (var stat in _stats)
            {
                if(stat != null)
                    stat.OnLevelUp -= UpdateReadyStatus;
            }
            
            _stats.Clear();
        }

        public virtual void Dispose() { }
    }
}
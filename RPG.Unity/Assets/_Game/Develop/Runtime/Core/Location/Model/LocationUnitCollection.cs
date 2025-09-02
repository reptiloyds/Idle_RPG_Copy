using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Location.Model
{
    public class LocationUnitCollection
    {
        private readonly List<UnitView> _units = new();

        public IReadOnlyList<UnitView> Units => _units;

        public event Action<UnitView> OnUnitEnter;
        public event Action<UnitView> OnUnitExit;

        public event Action OnChanged;

        [Preserve]
        public LocationUnitCollection()
        {
            
        }

        public void AppendUnit(UnitView unitView)
        {
            if(_units.Contains(unitView)) return;
            
            _units.Add(unitView);
            unitView.OnDie += RemoveUnit; 
            unitView.DisposeEvent += RemoveUnit;
            
            OnUnitEnter?.Invoke(unitView);
            OnChanged?.Invoke();
        }

        public void RemoveUnit(UnitView unitView)
        {
            if (!_units.Remove(unitView)) return;
            
            unitView.OnDie -= RemoveUnit; 
            unitView.DisposeEvent -= RemoveUnit;
            OnUnitExit?.Invoke(unitView);
            OnChanged?.Invoke();
        }

        public void Clear()
        {
            foreach (var unit in _units)
            {
                unit.OnDie -= RemoveUnit; 
                unit.DisposeEvent -= RemoveUnit;
            }
            _units.Clear();
            OnChanged?.Invoke();
        }
    }
}
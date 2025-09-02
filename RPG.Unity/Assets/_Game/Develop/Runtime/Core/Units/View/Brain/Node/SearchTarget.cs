using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Units.Team;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.Weapons;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Movement;
using PleasantlyGames.RPG.Runtime.NodeMachine.Model;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node
{
    internal enum SearchType
    {
        Closest = 0,
        Random = 1,
    }
    
    internal class SearchTarget : UnitNode
    {
        private readonly SearchType _searchType;
        private readonly VariableContainer<UnitView> _target;
        private readonly MortalUnitContainer _mortalUnitContainer;
        private readonly List<int> _validRandomIndexes = new();

        public SearchTarget(UnitView unitView, MortalUnitContainer mortalUnitContainer, SearchType searchType, VariableContainer<UnitView> target) : base(unitView)
        {
            _searchType = searchType;
            _target = target;
            _mortalUnitContainer = mortalUnitContainer;
        }

        public override void Enter()
        {
            base.Enter();

            var enemies = _mortalUnitContainer.GetEnemies(UnitView.TeamType);
            UnitView target = null;
            switch (_searchType)
            {
                case SearchType.Closest:
                    target = GetClosest(enemies);
                    break;
                case SearchType.Random:
                    target = GetRandom(enemies);
                    break;
                default:
                    Debug.LogError($"Unknown {_searchType}");
                    break;
            }

            if (target == null)
            {
                Fail();
                return;
            }

            _target.Set(target);
            Complete();
        }

        private UnitView GetClosest(List<UnitView> units)
        {
            var minDistance = float.MaxValue;
            UnitView closestUnitView = null;

            var canAttackAirTarget = UnitView.BaseMovement.Type == MovementType.Air || UnitView.Equipment.Weapon.Type == WeaponType.Range;
            foreach (var unit in units)
            {
                if (!canAttackAirTarget)
                    if(unit.BaseMovement.Type == MovementType.Air) continue;
                
                var fakeDistance = Vector3.SqrMagnitude(unit.transform.position - UnitView.transform.position);
                if (fakeDistance >= minDistance) continue;
                
                minDistance = fakeDistance;
                closestUnitView = unit;
            }

            return closestUnitView;
        }
        
        private UnitView GetRandom(List<UnitView> units)
        {
            var canAttackAirTarget = UnitView.BaseMovement.Type == MovementType.Air || UnitView.Equipment.Weapon.Type == WeaponType.Range;
            if(canAttackAirTarget) return units.GetRandomElement();

            for (int i = 0; i < units.Count; i++)
            {
                if(units[i].BaseMovement.Type == MovementType.Air) continue;
                _validRandomIndexes.Add(i);
            }

            var randomElement = _validRandomIndexes.GetRandomElement();
            var result = units[randomElement];
            _validRandomIndexes.Clear();
            return result;
        }
    }
}
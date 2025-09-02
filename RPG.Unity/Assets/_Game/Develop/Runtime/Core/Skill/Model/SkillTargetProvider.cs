using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Contract;
using PleasantlyGames.RPG.Runtime.Core.Units.Factory;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model
{
    public enum SkillSearchTargetType
    {
        InArea = 0,
        All = 1,
    }
    
    public struct SkillTargetRequest
    {
        public SkillSearchTargetType SearchTargetType;
        public HashSet<TeamType> TeamFilter;
        public HashSet<UnitSubType> IncludeTypeFilter;
        public HashSet<UnitSubType> ExcludeTypeFilter;
    }

    public struct SkillTargetResponse
    {
        public bool Success;
        public Vector3 AlternativePosition;
    }
    
    public class SkillTargetProvider
    {
        [Inject] private UnitFactory _unitFactory;
        [Inject] private LocationUnitCollection _locationUnitCollection;
        
        private readonly List<UnitView> _validUnits = new();
        private ISkillLocationDataProvider _dataProvider;

        [Preserve]
        public SkillTargetProvider() { }

        public Vector3 GetRandomPosition() => 
            _dataProvider.GetRandomPosition();

        public Vector3 GetCenterXZPosition() =>
            _dataProvider.GetCenterXZPosition();

        public void SetSkillData(ISkillLocationDataProvider dataProvider) => 
            _dataProvider = dataProvider;

        public SkillTargetResponse GetTarget(SkillTargetRequest request, List<UnitView> targets, List<UnitView> exclusion = null)
        {
            IReadOnlyList<UnitView> units;
            switch (request.SearchTargetType)
            {
                case SkillSearchTargetType.InArea:
                    units = _locationUnitCollection.Units;
                    break;
                case SkillSearchTargetType.All:
                    units = _unitFactory.Units;
                    break;
                default:
                    units = _unitFactory.Units;
                    break;
            }
            SearchValidUnits(units, request);
            var success = false;
            if(_validUnits.Count > 0)
            {
                if (exclusion != null)
                {
                    foreach (var unit in _validUnits)
                    {
                        if(exclusion.Contains(unit))
                            continue;
                        targets.Add(unit);
                        success = true;
                    }

                    if (!success)
                    {
                        targets.AddRange(_validUnits);
                        success = true;
                    }
                }
                else
                {
                    targets.AddRange(_validUnits);
                    success = true;
                }
            }
            var result = new SkillTargetResponse { Success = success, AlternativePosition = success ? Vector3.zero : _dataProvider.GetRandomEmptyPosition()};
            
            _validUnits.Clear();
            return result;
        }

        private Vector3 GetClosestBoundedPosition()
        {
            UnitView closestUnitView = null;
            var fakeMinDistance = float.MaxValue;
            var areaCenter = _dataProvider.GetCenterPosition();
            foreach (var validUnit in _validUnits)
            {
                var fakeDistance = Vector3.SqrMagnitude(validUnit.transform.position - areaCenter); 
                if (fakeDistance < fakeMinDistance)
                {
                    fakeMinDistance = fakeDistance;
                    closestUnitView = validUnit;
                }
            }

            if (closestUnitView == null) return Vector3.zero;

            return _dataProvider.FindClosestPoint(closestUnitView.transform.position);
        }

        private void SearchValidUnits(IReadOnlyList<UnitView> units, SkillTargetRequest request)
        {
            foreach (var unit in units)
            {
                if (!IsTeamValid(unit, request.TeamFilter)) continue;
                if (!IsTypeValid(unit, request.IncludeTypeFilter, request.ExcludeTypeFilter)) continue;
                _validUnits.Add(unit);
            }
        }

        private bool IsTeamValid(UnitView unitView, HashSet<TeamType> teamFilter)
        {
            if (teamFilter == null)
                return true;
            return teamFilter.Count <= 0 || teamFilter.Contains(unitView.TeamType);
        }

        private bool IsTypeValid(UnitView unitView, HashSet<UnitSubType> includeTypeFilter, HashSet<UnitSubType> excludeTypeFilter)
        {
            var includeFilterIsNullOrEmpty = includeTypeFilter == null || includeTypeFilter.Count == 0;
            var excludeFilterIsNullOrEmpty = excludeTypeFilter == null || excludeTypeFilter.Count == 0;
            if (includeFilterIsNullOrEmpty && excludeFilterIsNullOrEmpty)
                return true;
            
            var hasIncludeTypes = false;
            var types = unitView.SubTypes;
            foreach (var type in types)
            {
                if(!hasIncludeTypes && (includeFilterIsNullOrEmpty || includeTypeFilter.Contains(type)))
                    hasIncludeTypes = true;

                if(!excludeFilterIsNullOrEmpty && excludeTypeFilter.Contains(type))
                    return false;
            }
            return hasIncludeTypes;
        }
    }
}
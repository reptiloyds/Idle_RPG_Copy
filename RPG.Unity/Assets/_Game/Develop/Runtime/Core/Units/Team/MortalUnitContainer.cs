using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Team
{
    public class MortalUnitContainer
    {
        [Inject] private LocationUnitCollection _locationUnitCollection;
        private readonly Dictionary<TeamType, List<UnitView>> _teams = new();
        private readonly List<UnitView> _enemyBuffer = new();

        [UnityEngine.Scripting.Preserve]
        public MortalUnitContainer()
        {
        }

        public void Add(UnitView unitView)
        {
            if (_teams.TryGetValue(unitView.TeamType, out var team))
                team.Add(unitView);
            else
            {
                var newTeam = new List<UnitView> { unitView };
                _teams.Add(unitView.TeamType, newTeam);
            }
        }

        public void Remove(UnitView unitView)
        {
            if (_teams.TryGetValue(unitView.TeamType, out var team))
                team.Remove(unitView);
        }

        public List<UnitView> GetEnemies(TeamType teamType)
        {
            _enemyBuffer.Clear();
            foreach (var team in _teams)
            {
                if (team.Key == teamType) continue;
                foreach (var enemy in team.Value)
                {
                    if(!_locationUnitCollection.Units.Contains(enemy)) continue;
                    _enemyBuffer.Add(enemy);
                }
            }


            return _enemyBuffer;
        }
    }
}
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Sheets;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model
{
    public class GlobalStatProvider
    {
        [Inject] private BalanceContainer _balance;

        private readonly List<GlobalStat> _stats = new();

        [Preserve]
        public GlobalStatProvider() { }

        public void Initialize() => 
            CreateStats();

        private void CreateStats()
        {
            var globalStatSheet = _balance.Get<GlobalStatSheet>();
            foreach (var statConfig in globalStatSheet) 
                _stats.Add(new GlobalStat(statConfig));
        }

        public GlobalStat GetStat(GlobalStatType type)
        {
            foreach (var stat in _stats)
                if (stat.Type == type) return stat;

            return null;
        }
    }
}
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Definition;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.Extension
{
    public static class RouletteExtension
    {
         public static void TryDeserialize(this RouletteType type, string json)
        {
            switch (type)
            {
                case RouletteType.None:
                    break;
                case RouletteType.DailyWheel:
                    var dailyData = JsonConvertLog.DeserializeObject<DailyRouletteDefinition>(json);
                    break;
                case RouletteType.SlotRush:
                    break;
            }
        }
    }
}
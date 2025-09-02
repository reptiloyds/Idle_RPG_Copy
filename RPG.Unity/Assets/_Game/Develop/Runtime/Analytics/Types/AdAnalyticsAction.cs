using System;

namespace PleasantlyGames.RPG.Runtime.Analytics.Types
{
    [Flags]
    public enum AdAnalyticsAction
    {
        Start = 1 << 0,
        Success = 1 << 1,
        Fail = 1 << 2,
    }
}
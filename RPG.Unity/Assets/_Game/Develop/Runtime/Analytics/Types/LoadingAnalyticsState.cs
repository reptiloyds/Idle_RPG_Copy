using System;

namespace PleasantlyGames.RPG.Runtime.Analytics.Types
{
    [Flags]
    public enum LoadingAnalyticsState
    {
        Start = 1 << 0,
        Complete = 1 << 1,
    }
}
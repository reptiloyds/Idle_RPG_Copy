using System;

namespace PleasantlyGames.RPG.Runtime.Analytics.Types
{
    [Flags]
    public enum AdAnalyticsType
    {
        Interstitial = 1 << 0,
        Rewarded = 1 << 1,
        Banner = 1 << 2,
    }
}
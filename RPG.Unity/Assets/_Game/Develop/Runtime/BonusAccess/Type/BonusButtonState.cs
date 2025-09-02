using System;

namespace PleasantlyGames.RPG.Runtime.BonusAccess.Type
{
    [Flags]
    public enum BonusButtonState
    {
        None = 0,
        Inactive = 1 << 0,
        FreeAccess = 1 << 1,
        Cooldown = 1 << 2,
        Ad = 1 << 3,
        Vip = 1 << 4,

        Main = Ad | Vip,
    }
}
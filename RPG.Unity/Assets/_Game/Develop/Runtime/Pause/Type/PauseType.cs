using System;

namespace PleasantlyGames.RPG.Runtime.Pause.Type
{
    [Flags]
    public enum PauseType
    {
        Time = 1 << 0,
        Audio = 1 << 1,
        CursorVision = 1 << 2,
        CursorLock = 1 << 3,
        Input = 1 << 4,
        Ad = 1 << 5,
        UIInput = 1 << 6,
        Save = 1 << 7,
    }
}
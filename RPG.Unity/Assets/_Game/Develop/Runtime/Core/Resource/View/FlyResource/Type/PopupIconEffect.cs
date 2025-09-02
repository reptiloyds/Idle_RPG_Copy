using System;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Type
{
    [Flags]
    public enum PopupIconEffect
    {
        None = 1 << 0,
        StarsEffect = 1 << 1,
    }
}
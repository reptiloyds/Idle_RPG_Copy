using System;
using Sirenix.OdinInspector;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons
{
    [Serializable]
    public class DungeonConfiguration
    {
        [MinValue(0)] public float AutoSweepDelay = 5f;
    }
}
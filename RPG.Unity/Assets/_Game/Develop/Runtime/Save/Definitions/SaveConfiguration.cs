using System;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Save.Definitions
{
    [Serializable]
    public class SaveConfiguration
    {
        [Min(3)] public int AutoSaveDelay;
        [Min(30)] public int AutoSyncDelay;
    }
}
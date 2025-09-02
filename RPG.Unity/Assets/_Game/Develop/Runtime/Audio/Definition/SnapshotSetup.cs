using System;
using PleasantlyGames.RPG.Runtime.Audio.Type;
using Sirenix.OdinInspector;

namespace PleasantlyGames.RPG.Runtime.Audio.Definition
{
    [Serializable]
    public class SnapshotSetup
    {
        public AudioSnapshot Type;
        [MinValue(0)]
        public float TransitionTime;
    }
}
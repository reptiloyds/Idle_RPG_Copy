using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Music.Type;
using Sirenix.OdinInspector;

namespace PleasantlyGames.RPG.Runtime.Core.Music.Definition
{
    [Serializable]
    public class MusicConfiguration
    {
        public MusicMode Mode;
        [HideIf("@this.Mode == MusicMode.None || this.Mode == MusicMode.Single")]
        public List<MusicType> MusicList;
        [HideIf("@this.Mode == MusicMode.None || this.Mode == MusicMode.Sequence || this.Mode == MusicMode.Random")]
        public MusicType MusicType;
    }
}
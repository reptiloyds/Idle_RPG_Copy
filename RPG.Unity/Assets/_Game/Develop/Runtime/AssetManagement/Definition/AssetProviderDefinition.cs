using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.Definition
{
    [Serializable]
    public class AssetProviderDefinition
    {
#if RPG_DEV
        [MinValue(0)]
        public float UploadingDurationOffset;  
#endif
        [MinValue(0)]
        public int FrameDelayBeforeUploadWindow = 10;
    }
}
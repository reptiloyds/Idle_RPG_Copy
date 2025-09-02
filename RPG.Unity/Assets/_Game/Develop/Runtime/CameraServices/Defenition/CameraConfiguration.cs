using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Defenition
{
    [Serializable]
    public class CameraConfiguration
    {
        public float DemonstrationTime = 2;
        public float BlendTime = 1;
        [ProgressBar(0, 1)]
        public float StartZoomProgress;
        public float ZoomSensentivity = 10f;
        [MinValue(0)]
        public float ZoomSpeed = 10f;
        public Vector2 ZoomRange = new(-1, 1);
    }
}
using System;
using PleasantlyGames.RPG.Runtime.CameraServices.Model.Mediator;
using UnityEngine;
using CameraType = PleasantlyGames.RPG.Runtime.CameraServices.Type.CameraType;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Defenition
{
    [Serializable]
    public class VirtualCameraConfig
    {
        public CameraType Type;
        public CinemachineInterlayer Interlayer;
        [HideInInspector] public int DefaultPriority;

        public void ResetPriority() => 
            Interlayer.Instance.m_Priority = DefaultPriority;
    }
}
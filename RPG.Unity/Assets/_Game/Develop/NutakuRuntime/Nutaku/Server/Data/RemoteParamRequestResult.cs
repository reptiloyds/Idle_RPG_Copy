using System;
using UnityEngine.Serialization;

namespace _Game.Scripts.Systems.Server.Data
{
    [Serializable]
    public class RemoteParamRequestResult
    {
        public ServerRequestResult Result;
        public string ParamName;
        [FormerlySerializedAs("UpdatedValue")] public double Value;
    }
}
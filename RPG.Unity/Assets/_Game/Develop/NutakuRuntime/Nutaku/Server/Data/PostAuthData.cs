using System;

namespace _Game.Scripts.Systems.Server.Data
{
    [Serializable]
    public class PostAuthData
    {
        public string id;
        public string platform_id;
        public string platform;
        public string device;
    }
}
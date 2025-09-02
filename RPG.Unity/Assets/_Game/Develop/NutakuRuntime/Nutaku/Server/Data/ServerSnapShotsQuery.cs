using System;
using System.Collections.Generic;

namespace _Game.Scripts.Systems.Server.Data
{
    [Serializable]
    public class ServerSnapShotsQuery
    {
        public List<ServerSnapShotData> data;
        public int total;
        public string page;
        public string limit;
    }
}
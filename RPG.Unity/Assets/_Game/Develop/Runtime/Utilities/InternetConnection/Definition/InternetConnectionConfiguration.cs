using System;
using System.Collections.Generic;

namespace PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Definition
{
    [Serializable]
    public class InternetConnectionConfiguration
    {
        public float FrequentPingDelay = 1;
        public float RarePingDelay = 3;
        public List<string> Uris;
    }
}
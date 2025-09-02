using PleasantlyGames.RPG.Runtime.Utilities.ApplicationCloser.Contract;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Utilities.ApplicationCloser.Model
{
    public class DefaultApplicationCloser : IApplicationCloser
    {
        public void Close() => 
            Application.Quit();
    }
}
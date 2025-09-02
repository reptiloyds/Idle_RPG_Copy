using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Building
{
    public interface IBuildElement
    {
        void LogIfWrong(ref int errorCount);
    }
}
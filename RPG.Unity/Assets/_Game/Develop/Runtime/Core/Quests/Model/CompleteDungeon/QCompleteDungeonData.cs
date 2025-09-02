using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.CompleteDungeon
{
    [Serializable]
    public class QCompleteDungeonData
    {
        public GameModeType Type; 
        public int Level;
        
        [Preserve]
        public QCompleteDungeonData()
        {
        }
    }
}
namespace PleasantlyGames.RPG.Runtime.Core.Enemy
{
    public class EnemyData
    {
        public string UnitId;
        public int Count;

        public EnemyData(string unitId, int count)
        {
            UnitId = unitId;
            Count = count;
        }
    }
}
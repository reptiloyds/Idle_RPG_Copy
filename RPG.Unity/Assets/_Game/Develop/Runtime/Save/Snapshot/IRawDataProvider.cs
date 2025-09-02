namespace PleasantlyGames.RPG.Runtime.Save.Snapshot
{
    public interface IRawDataProvider
    {
        void Set(string rawData);
        string Get();
    }
}
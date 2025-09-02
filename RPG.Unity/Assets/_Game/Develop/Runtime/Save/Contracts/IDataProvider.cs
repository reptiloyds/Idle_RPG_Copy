namespace PleasantlyGames.RPG.Runtime.Save.Contracts
{
    public interface IDataProvider
    {
        void UpdateData();
        void SaveData();
        void LoadData();
    }
}
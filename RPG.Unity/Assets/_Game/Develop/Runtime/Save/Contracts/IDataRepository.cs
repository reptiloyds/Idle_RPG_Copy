using Cysharp.Threading.Tasks;

namespace PleasantlyGames.RPG.Runtime.Save.Contracts
{
    public interface IDataRepository
    {
        void Save();
        UniTask<bool> SaveToCloudAsync();
        UniTask LoadAsync();

        void SetData<T>(T value);
        bool TryGetData<T>(out T value);
    }
}
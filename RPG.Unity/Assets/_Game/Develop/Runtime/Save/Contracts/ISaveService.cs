using Cysharp.Threading.Tasks;

namespace PleasantlyGames.RPG.Runtime.Save.Contracts
{
    public interface ISaveService
    {
        void Initialize();
        void Load();
        UniTask SaveAndLoadToCloudAsync();
        void RegisterProvider(IDataProvider provider);
        void UnregisterProvider(IDataProvider provider);
    }
}
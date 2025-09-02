using Cysharp.Threading.Tasks;

namespace PleasantlyGames.RPG.Runtime.Bootstrap
{
    public interface IBootstrapStarter
    {
        UniTask WaitForReady();
    }
}
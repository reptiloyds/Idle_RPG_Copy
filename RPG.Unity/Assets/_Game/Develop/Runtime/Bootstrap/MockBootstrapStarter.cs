using Cathei.BakingSheet.Internal;
using Cysharp.Threading.Tasks;

namespace PleasantlyGames.RPG.Runtime.Bootstrap
{
    public class MockBootstrapStarter : IBootstrapStarter
    {
        [Preserve]
        public MockBootstrapStarter()
        {
        }
        
        public UniTask WaitForReady() => 
            UniTask.CompletedTask;
    }
}
using Cysharp.Threading.Tasks;

namespace PleasantlyGames.RPG.Runtime.Balance.Contract
{
    public interface IBalanceLoader
    {
        public UniTask Load(string jsonBalance = null);
    }
}
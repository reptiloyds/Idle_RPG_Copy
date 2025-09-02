using System;
using Cysharp.Threading.Tasks;

namespace PleasantlyGames.RPG.Runtime.Balance.Contract
{
    public interface IBalanceProvider
    {
        string FileName { get; }
        event Action OnFileNameChanged;
        UniTask<bool> LoadAsync();
    }
}
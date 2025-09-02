using System;
using Cysharp.Threading.Tasks;
using R3;

namespace PleasantlyGames.RPG.Runtime.Ad.Contracts
{
    public interface IAdService : IDisposable
    {
        public event Action<string, bool> OnRewardClosed;
        public event Action OnRewardRefreshed;
        public ReadOnlyReactiveProperty<bool> IsDisabled { get; }

        public void Initialize();
        public void Disable();

        public bool CanShowReward();
        public void ShowReward(string id);
    }
}

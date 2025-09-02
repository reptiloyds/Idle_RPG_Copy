using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using R3;

namespace PleasantlyGames.RPG.Runtime.VIP.Contract
{
    public interface IVipService
    {
        ReadOnlyReactiveProperty<bool> IsActive { get; }
        ReadOnlyReactiveProperty<float> Duration { get; }
        IReadOnlyCollection<VipBonusData> BonusData { get; }
        IReadOnlyCollection<VipPurchaseReward> PurchaseRewards { get; }
        void Initialize();
        UniTask<bool> TryActivate();
    }
}
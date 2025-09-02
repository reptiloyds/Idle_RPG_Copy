using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Contract
{
    public interface IResourcefulDungeon : ILeveledDungeon
    {
        void ApplyRewardFor(int level);
        BigDouble.Runtime.BigDouble GetRewardFor(int level);
        ResourceType RewardType { get; }
        Color RewardColor { get; }
        public ResourceModel EnterResource { get; }
        public Sprite RewardImage { get; }
        void Sweep(int level, int amount);
    }
}
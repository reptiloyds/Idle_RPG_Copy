using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Type;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.Model
{
    public struct ResourceFXRequest : IEquatable<ResourceFXRequest>
    {
        public ResourceType Type;
        public BigDouble.Runtime.BigDouble Amount;
        public Vector3 SpawnPosition;
        public PopupIconContext Context;
        public int MaxViewReward;
        public bool ForceToTarget;
        [Preserve] public bool IsCreated;
        
        public static ResourceFXRequest Create(
            ResourceType type = ResourceType.None,
            BigDouble.Runtime.BigDouble amount = default,
            Vector3 spawnPosition = default,
            PopupIconContext context = PopupIconContext.OverUI,
            int maxViewReward = 0,
            bool forceToTarget = false,
            bool isCreated = true
        ) => new(type, amount, spawnPosition, context, maxViewReward, forceToTarget, isCreated);

        private ResourceFXRequest(
            ResourceType type,
            BigDouble.Runtime.BigDouble amount,
            Vector3 spawnPosition,
            PopupIconContext context = PopupIconContext.OverUI,
            int maxViewReward = 0,
            bool forceToTarget = false,
            bool isCreated = true)
        {
            Type = type;
            Amount = amount;
            SpawnPosition = spawnPosition;
            Context = context;
            MaxViewReward = maxViewReward;
            ForceToTarget = forceToTarget;
            IsCreated = isCreated;
        }

        public override string ToString() =>
            $"ResourceFXRequest(Type={Type}, Amount={Amount}, Pos={SpawnPosition}, Context={Context}, MaxViewReward={MaxViewReward}, ForceToTarget={ForceToTarget})";

        public override int GetHashCode() =>
            HashCode.Combine(Type, Amount, SpawnPosition, Context, MaxViewReward, ForceToTarget);

        public override bool Equals(object obj) => obj is ResourceFXRequest other && Equals(other);

        public bool Equals(ResourceFXRequest other) =>
            Type == other.Type &&
            Amount == other.Amount &&
            SpawnPosition == other.SpawnPosition &&
            Context == other.Context &&
            MaxViewReward == other.MaxViewReward &&
            ForceToTarget == other.ForceToTarget;

        public static bool operator ==(ResourceFXRequest left, ResourceFXRequest right) => left.Equals(right);
        public static bool operator !=(ResourceFXRequest left, ResourceFXRequest right) => !(left == right);
    }
}
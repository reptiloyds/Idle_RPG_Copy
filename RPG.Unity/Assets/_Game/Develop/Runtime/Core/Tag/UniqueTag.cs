using System;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tag
{
    public abstract class UniqueTag : ScriptableObject
    {
        [SerializeField, ReadOnly] private string _hashId;
        public string HashId => _hashId;

        private void Reset() =>
            GenerateHashId();

        [ContextMenu("!RegenerateHashId!")]
        public void GenerateHashId() => 
            _hashId = RandomHash.New();
        
        public override bool Equals(object obj)
        {
            if (obj is UniqueTag other)
                return string.Equals(HashId, other.HashId);
            return false;
        }

        protected bool Equals(UniqueTag other)
        {
            return base.Equals(other) && _hashId == other._hashId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), _hashId);
        }

        public static bool operator ==(UniqueTag a, UniqueTag b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(UniqueTag a, UniqueTag b) => !(a == b);
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits
{
    public class CompositeLoadUnit : ILoadUnit
    {
        private readonly Queue<ILoadUnit[]> _loadUnits = new();

        public string DescriptionToken { get; }

        [Preserve]
        public CompositeLoadUnit(ILoadUnit[] loadUnits, string description)
        {
            _loadUnits.Enqueue(loadUnits);
            DescriptionToken = description;
        }
        
        public CompositeLoadUnit(string description) => 
            DescriptionToken = description;

        public void Append(ILoadUnit[] loadUnits) => 
            _loadUnits.Enqueue(loadUnits);

        public void Append(ILoadUnit loadUnit) => 
            _loadUnits.Enqueue(new[] {loadUnit});

        public virtual async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            while (_loadUnits.Count > 0)
            {
                var loadUnits = _loadUnits.Dequeue();
                var tasks = new List<UniTask>(loadUnits.Length);
                foreach (var loadUnit in loadUnits) 
                    tasks.Add(loadUnit.LoadAsync(token, progress));
                await UniTask.WhenAll(tasks);
            }
        }
    }
}
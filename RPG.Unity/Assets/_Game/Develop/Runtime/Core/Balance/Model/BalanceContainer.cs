using System;
using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Balance.Model
{
    public class BalanceContainer : IDisposable
    {
        private readonly HashSet<IDisposable> _instances = new();

        [Preserve]
        public BalanceContainer() { }
            
        public void Set<T>(T sheet) where T : class, ISheet
        {
            var instance = SheetContainer<T>.Instance;
            _instances.Add(instance);
            instance.Set(sheet);
        }

        public T Get<T>() where T : class, ISheet => 
            SheetContainer<T>.Instance.Get();

        public void Dispose()
        {
            foreach (var instance in _instances) 
                instance.Dispose();
        }
    }
}
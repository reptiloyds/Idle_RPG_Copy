using System;
using Cathei.BakingSheet;

namespace PleasantlyGames.RPG.Runtime.Core.Balance.Container
{
    public class SheetContainer<T> : IDisposable where T : class, ISheet
    {
        private static SheetContainer<T> _instance;
        public static SheetContainer<T> Instance => _instance ??= new SheetContainer<T>();
        
        private T _sheet;
        
        public void Set(T sheet) => _sheet = sheet;

        public T Get() => _sheet;

        public void Dispose()
        {
            _instance = null;
            _sheet = null;
        }
    }
}
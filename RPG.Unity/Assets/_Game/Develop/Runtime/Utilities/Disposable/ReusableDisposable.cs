using System;

namespace PleasantlyGames.RPG.Runtime.Utilities.Disposable
{
    public class ReusableDisposable : IDisposable
    {
        private IDisposable _disposable;
        
        public IDisposable Disposable
        {
            get => _disposable;
            set
            {
                _disposable?.Dispose();
                _disposable = value;
            }
        }
        
        public void Dispose() => 
            _disposable?.Dispose();
    }
}
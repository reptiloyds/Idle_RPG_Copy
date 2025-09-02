using System;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Save.Models
{
    public abstract class BaseDataProvider<T> : IDataProvider, IInitializable, IDisposable
    {
        [Inject] private IDataRepository _repository;
        [Inject] private ISaveService _saveService;
        
        protected T Data;
        private readonly bool _loadOnInitialize = false;

        public event Action OnUpdateData;

        public T GetData() => Data;

        [Preserve]
        protected BaseDataProvider()
        {
        }

        protected BaseDataProvider(bool loadOnInitialize) => 
            _loadOnInitialize = loadOnInitialize;

        public virtual void Initialize()
        {
            _saveService.RegisterProvider(this);
            if(_loadOnInitialize)
                LoadData();
        }

        public void Dispose() => 
            _saveService.UnregisterProvider(this);

        public virtual void UpdateData() => 
            OnUpdateData?.Invoke();

        public void SaveData() => _repository.SetData(Data);

        public virtual void LoadData()
        {
            if(Data == null)
                _repository.TryGetData(out Data);
        }
    }
}
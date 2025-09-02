using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.Save.Definitions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Save.Models
{
    public class SaveService : ISaveService, ITickable
    {
        private readonly SaveConfiguration _config;
        private readonly IDataRepository _repository;
        private readonly IPauseService _pauseService;
        
        private readonly HashSet<IDataProvider> _providers = new();
        
        private float _saveTime;
        private float _syncTime;
        
        private bool _isPaused;
        private bool _isDisabled;
        private bool _isSemaphoreBusy;
        private bool _isInitialized;
        private readonly SemaphoreSlim _syncSemaphore = new(1, 1);

        public bool IsSaveDisabled => _isPaused || _isDisabled;
        public bool IsSaveLocked => IsSaveDisabled || _isSemaphoreBusy;

        [Preserve]
        [Inject]
        public SaveService(IDataRepository repository,
            SaveConfiguration config,
            IPauseService pauseService)
        {
            _repository = repository;
            _config = config;
            _pauseService = pauseService;
        }

        public void RegisterProvider(IDataProvider provider) => 
            _providers.Add(provider);

        public void UnregisterProvider(IDataProvider provider) => 
            _providers.Remove(provider);

        public void Initialize()
        {
#if RPG_DEV && SAVE_DISABLED
            _isDisabled = true;
#endif
            _pauseService.OnPause += OnPause;
            _pauseService.OnContinue += OnContinue;

            Load();
            
            _saveTime = Time.time + _config.AutoSaveDelay;
            _syncTime = Time.time + _config.AutoSyncDelay;
            
            _isInitialized = true;
        }

        private void OnPause(PauseType type)
        {
            if (type.HasFlag(PauseType.Save)) 
                _isPaused = true;
        }

        private void OnContinue(PauseType type)
        {
            if (type.HasFlag(PauseType.Save)) 
                _isPaused = false; 
        }

        void ITickable.Tick()
        {
            if(!_isInitialized) return;
            if(IsSaveLocked) return;

            if (Time.time > _syncTime) AutoSaveAndLoadToCloudAsync().Forget();
            else if (Time.time > _saveTime) Save();
        }

        public void Load()
        {
            foreach(var dataLoader in _providers) 
                dataLoader.LoadData();
        }
        
        public async UniTask SaveAndLoadToCloudAsync()
        {
            if(IsSaveDisabled) return;

            _isSemaphoreBusy = true;
            await _syncSemaphore.WaitAsync();
            Save();
            await LoadToCloudAsync();
            _syncSemaphore.Release();
            _isSemaphoreBusy = false;
        }

        private async UniTaskVoid AutoSaveAndLoadToCloudAsync() => 
            await SaveAndLoadToCloudAsync();

        private void Save()
        {
            foreach (var provider in _providers)
            {
                provider.UpdateData();
                provider.SaveData();
            }
            _repository.Save();

            _saveTime = Time.time + _config.AutoSaveDelay;
        }

        private async UniTask LoadToCloudAsync()
        {
            await _repository.SaveToCloudAsync();
            _syncTime = Time.time + _config.AutoSyncDelay;
        }
    }
}
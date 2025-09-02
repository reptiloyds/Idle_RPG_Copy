using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Defenition;
using PleasantlyGames.RPG.Runtime.Core.Resource.Save;
using PleasantlyGames.RPG.Runtime.Core.Resource.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.Model
{
    public class ResourceService : IDisposable
    {
        [Inject] private ResourceConfiguration _configuration;
        [Inject] private BalanceContainer _balance;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private ResourceDataProvider _dataProvider;
        
        private ResourceDataContainer _data;
        
        private readonly List<ResourceModel> _resources = new ();
        
        public event Action<ResourceModel, BigDouble.Runtime.BigDouble> OnResourceSpend;
        public event Action<ResourceModel, BigDouble.Runtime.BigDouble> OnResourceAdd;
        
        private readonly ReactiveProperty<ResourceFXRequest> _fxRequest = new ();
        public ReadOnlyReactiveProperty<ResourceFXRequest> FXRequest => _fxRequest;

        [Preserve]
        public ResourceService() { }

        public void Initialize()
        {
            _data = _dataProvider.GetData();
            CreateModels();
        }

        void IDisposable.Dispose()
        {
            foreach (var resource in _resources)
            {
                resource.OnAdd -= OnAddResource;
                resource.OnSpend -= OnSpendResource;   
            }
        }

        private void CreateModels()
        {
            var resourceSheet = _balance.Get<ResourceSheet>();
            var atlasName = Asset.MainAtlas;
            foreach (var data in _data.Resources)
            {
                if (!resourceSheet.Contains(data.Type)) continue;
                var config = resourceSheet[data.Type];
                var resource = new ResourceModel(config, data, _spriteProvider.GetSprite(atlasName, config.ImageName));
                resource.OnAdd += OnAddResource;
                resource.OnSpend += OnSpendResource;
                _resources.Add(resource);   
            }
        }

        public ResourceModel GetResource(ResourceType type)
        {
            foreach (var model in _resources)
            {
                if(model.Type != type) continue;
                return model;
            }

            return null;
        }

        public void AddResource(ResourceType type, BigDouble.Runtime.BigDouble amount, ResourceFXRequest fxRequest = default)
        {
            if(amount <= 0) return;
            if(type == ResourceType.None) return;
            var resource = GetResource(type);
            resource?.Add(amount);

            if (fxRequest != default)
            {
                fxRequest.Type = type;
                fxRequest.Amount = amount;   
            }
            _fxRequest.Value = fxRequest;
        }

        public void SetFxRequest(ResourceFXRequest fxRequest)
        {
            _fxRequest.Value = fxRequest;
        }

        public void SpendResource(ResourceType type, BigDouble.Runtime.BigDouble amount)
        {
            if(amount <= 0) return;
            if(type == ResourceType.None) return;
            var resource = GetResource(type);
            resource?.Spend(amount);
        }

        public bool IsEnough(ResourceType type, BigDouble.Runtime.BigDouble value)
        {
            var resource = GetResource(type);
            if (resource == null) return false;
            return resource.IsEnough(value);
        }

        private void OnAddResource(ResourceModel resourceModel, BigDouble.Runtime.BigDouble delta) =>
            OnResourceAdd?.Invoke(resourceModel, delta);

        private void OnSpendResource(ResourceModel resourceModel, BigDouble.Runtime.BigDouble delta) =>
            OnResourceSpend?.Invoke(resourceModel, delta);
    }
}

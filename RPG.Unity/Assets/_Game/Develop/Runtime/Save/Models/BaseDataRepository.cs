using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.Save.DataTransformers;
using PleasantlyGames.RPG.Runtime.Save.Serializers;
using PleasantlyGames.RPG.Runtime.Save.Snapshot;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Save;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Save.Models
{
    public abstract class BaseDataRepository : IDataRepository, IRawDataProvider
    {
        protected IDataSerializer DataSerializer;
        protected IDataTransformer DataTransformer;
        protected Dictionary<string, string> Data;
        private string _rawData;
        
        public static string ClearDataKey => "ClearData";
        public static string PlayerProfileKey => "PlayerProfile";
        
        [Preserve]
        protected BaseDataRepository()
        {
        }

        [Inject]
        public virtual void Construct(IDataSerializer dataSerializer)
        {
            DataSerializer = dataSerializer;
            DataTransformer = dataSerializer;
        }
        
        void IRawDataProvider.Set(string rawData) => 
            _rawData = rawData;
        
        string IRawDataProvider.Get() => 
            SerializeContainer(Data);

        public abstract void Save();

        public abstract UniTask<bool> SaveToCloudAsync();

        public abstract UniTask LoadAsync();

        public void SetData<T>(T value) => 
            Data[typeof(T).Name] = SerializeData(value);

        public bool TryGetData<T>(out T value) => 
            TryGetDataFromDictionary(out value, Data);

        protected bool HasClearFlag() => 
            PlayerPrefs.HasKey(ClearDataKey);

        protected void RemoveClearFlag()
        {
            PlayerPrefs.DeleteKey(ClearDataKey);
            PlayerPrefs.Save();
        }

        protected virtual (string rawData, bool hasSnapshot) GetLocalRawData() =>
            (_rawData, !string.IsNullOrEmpty(_rawData));

        protected abstract string SerializeContainer<T>(T data);

        protected abstract T DeserializeContainer<T>(string data);
        
        protected abstract string SerializeData<T>(T data);
        
        protected abstract T DeserializeData<T>(string data);

        protected void DeserializeNewestData(string local, string remote)
        {
            var isRemoteEmpty = IsDataEmpty(remote);
            var isLocalEmpty = IsDataEmpty(local);
            if (isRemoteEmpty)
                Data ??= new Dictionary<string, string>();
            else if (isLocalEmpty)
                Data = DeserializeContainer<Dictionary<string, string>>(remote);
            else
            {
                var localData = DeserializeContainer<Dictionary<string, string>>(local);
                var remoteData = DeserializeContainer<Dictionary<string, string>>(remote);
                Data = GetNewestData(localData, remoteData, false);
            }
        }

        private Dictionary<string, string> GetNewestData(Dictionary<string, string> local, Dictionary<string, string> remote, bool localIsPreferred)
        {
            var localResult = TryGetDataFromDictionary(out TimeDataContainer firstDateData, local);
            var remoteResult = TryGetDataFromDictionary(out TimeDataContainer secondDateData, remote);
            if (localResult == false) return remote;
            if (remoteResult == false) return local;
            
            var compareResult = DateTime.Compare(firstDateData.SaveTime, secondDateData.SaveTime);
            switch (compareResult)
            {
                case < 0:
                    return remote;
                case 0:
                    return localIsPreferred ? local : remote;
                case > 0:
                    return local;
            }
        }

        private bool TryGetDataFromDictionary<T>(out T value, Dictionary<string, string> data)
        {
            if (data.TryGetValue(typeof(T).Name, out var serializedData))
            {
                value = DeserializeData<T>(serializedData);
                return true;
            }

            value = default;
            return false;
        }

        private bool IsDataEmpty(string data) => 
            data.IsNullOrWhitespace() || data == default || data == "";
    }
}
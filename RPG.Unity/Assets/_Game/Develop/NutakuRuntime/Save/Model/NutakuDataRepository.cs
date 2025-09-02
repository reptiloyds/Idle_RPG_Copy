using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Systems.Server;
using _Game.Scripts.Systems.Server.Data;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Save.DataTransformers;
using PleasantlyGames.RPG.Runtime.Save.Models;
using PleasantlyGames.RPG.Runtime.Save.Serializers;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.NutakuRuntime.Save.Model
{
    public class NutakuDataRepository : BaseDataRepository
    {
        [Inject]
        private readonly ServerSystem _serverSystem;

        [Preserve]
        public NutakuDataRepository() { }

        public override void Construct(IDataSerializer dataSerializer)
        {
            base.Construct(dataSerializer);
            DataTransformer = new GzipDataCompressor(DataTransformer);
        }

        public override void Save()
        {
            var dataString = SerializeContainer(Data);
            PlayerPrefs.SetString(PlayerProfileKey, dataString);
            PlayerPrefs.Save();
        }

        public override async UniTask<bool> SaveToCloudAsync()
        {
            var dataString = SerializeContainer(Data);
            await _serverSystem.LoadMainSaveJsonToCloud(dataString, true);
            return true;
        }

        protected override (string rawData, bool hasSnapshot) GetLocalRawData()
        {
            var tuple = base.GetLocalRawData();
            return tuple.hasSnapshot ? tuple : (PlayerPrefs.GetString(PlayerProfileKey), true);
        }

        public override async UniTask LoadAsync()
        {
            var localTuple = GetLocalRawData();
            var hasClearFlag = HasClearFlag();
            if (_serverSystem.IsActive && !hasClearFlag && !localTuple.hasSnapshot)
            {
                var rawRemote = await _serverSystem.RequestCloudMainSaveJson();
                var cloudData = JsonConvert.DeserializeObject<ServerSnapShotsQuery>(rawRemote);
                
                var cloudJson = cloudData?.data.FirstOrDefault()?.data;
                string remote;
                if (cloudJson == null)
                {
                    PlayerPrefs.SetString(PlayerProfileKey, "");
                    remote = string.Empty;
                }
                else
                    remote = cloudJson.ToString();
                
                DeserializeNewestData(localTuple.rawData, remote);   
            }
            else
            {
                if (hasClearFlag)
                {
                    RemoveClearFlag();
                    Data = new Dictionary<string, string>();
                }
                else
                    Data = DeserializeContainer<Dictionary<string, string>>(localTuple.rawData);
                if (hasClearFlag && _serverSystem.IsActive) SaveToCloudAsync().Forget();
            }
            
            Data ??= new Dictionary<string, string>();
        }

        protected override string SerializeContainer<T>(T data) => 
            DataSerializer.Serialize(data);

        protected override T DeserializeContainer<T>(string data) => 
            DataSerializer.Deserialize<T>(data);

        protected override string SerializeData<T>(T data) => 
            DataTransformer.Transform(DataSerializer.Serialize(data));

        protected override T DeserializeData<T>(string data) => 
            DataSerializer.Deserialize<T>(DataTransformer.Reverse(data));
    }
}
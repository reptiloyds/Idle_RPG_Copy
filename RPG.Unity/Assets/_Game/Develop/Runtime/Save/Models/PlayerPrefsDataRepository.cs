using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Save.DataTransformers;
using PleasantlyGames.RPG.Runtime.Save.Serializers;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Save.Models
{
    public class PlayerPrefsDataRepository : BaseDataRepository
    {
        [Preserve]
        public PlayerPrefsDataRepository() : base()
        {
        }

        public override void Construct(IDataSerializer dataSerializer)
        {
            base.Construct(dataSerializer);

            DataTransformer = new GzipDataCompressor(DataTransformer);
            DataTransformer = new AesDataEncryption(DataTransformer, "gh5j8DR41");
        }

        public override void Save()
        {
            PlayerPrefs.SetString(PlayerProfileKey, SerializeContainer(Data));
            PlayerPrefs.Save();
        }

        public override UniTask<bool> SaveToCloudAsync() =>
            UniTask.FromResult(true);
        
        protected override (string rawData, bool hasSnapshot) GetLocalRawData()
        {
            var tuple = base.GetLocalRawData();
            return tuple.hasSnapshot ? tuple : (PlayerPrefs.GetString(PlayerProfileKey), true);
        }
        
        public override UniTask LoadAsync()
        {
            var localTuple = GetLocalRawData();
            if (!string.IsNullOrEmpty(localTuple.rawData))
                Data = DeserializeContainer<Dictionary<string, string>>(localTuple.rawData);
            Data ??= new Dictionary<string, string>();

            return UniTask.CompletedTask;
        }

        protected override string SerializeContainer<T>(T data) => 
            DataTransformer.Transform(DataSerializer.Serialize(data));

        protected override T DeserializeContainer<T>(string data) => 
            DataSerializer.Deserialize<T>(DataTransformer.Reverse(data));

        protected override string SerializeData<T>(T data) =>
            DataSerializer.Serialize(data);

        protected override T DeserializeData<T>(string data) => 
            DataSerializer.Deserialize<T>(data);
    }
}
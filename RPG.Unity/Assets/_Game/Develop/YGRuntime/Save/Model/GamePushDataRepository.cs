using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FastMigrations.Runtime;
using GamePush;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Save.DataTransformers;
using PleasantlyGames.RPG.Runtime.Save.Models;
using PleasantlyGames.RPG.Runtime.Save.Serializers;
using PleasantlyGames.RPG.YGRuntime.Const;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace PleasantlyGames.RPG.YGRuntime.Save.Model
{
    public class GamePushDataRepository : BaseDataRepository, IInitializable, IDisposable
    {
        private UniTaskCompletionSource<bool> _syncCompletionSource;

        [Preserve]
        public GamePushDataRepository(string dataKey) : base(dataKey)
        {
        }

        public override void Construct(IDataSerializer dataSerializer)
        {
            base.Construct(dataSerializer);
            
            DataTransformer = new GzipDataCompressor(DataTransformer);
            DataTransformer = new AesDataEncryption(DataTransformer, "gh5j8DR41");
        }

        void IInitializable.Initialize()
        {
            GP_Player.OnSyncComplete += OnSyncComplete;
            GP_Player.OnSyncError += OnSyncError;
        }

        void IDisposable.Dispose()
        {
            GP_Player.OnSyncComplete -= OnSyncComplete;
            GP_Player.OnSyncError -= OnSyncError;
        }

        public override void Save() => 
            SaveLocal(SerializeContainer(Data));

        public override async UniTask<bool> SaveToCloudAsync()
        {
            Save();
            GP_Player.Sync();

            _syncCompletionSource = new UniTaskCompletionSource<bool>();
#if UNITY_EDITOR
            _syncCompletionSource.TrySetResult(true);
#endif
            var result = await _syncCompletionSource.Task;

            return result;
        }

        private void OnSyncError() => 
            _syncCompletionSource.TrySetResult(false);

        private void OnSyncComplete() => 
            _syncCompletionSource.TrySetResult(true);

        public override UniTask LoadAsync()
        {
            var local = PlayerPrefs.GetString(DataKey);
            var remote = GP_Player.GetString(DataKey);

            if (string.IsNullOrEmpty(remote)) 
                PlayerPrefs.SetString(DataKey, "");
            
            DeserializeNewestData(local, remote);
            
            return UniTask.CompletedTask;
        }
        
        private void SaveLocal(string dataString)
        {
            GP_Player.Set(DataKey, dataString);
            PlayerPrefs.SetString(DataKey, dataString);
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
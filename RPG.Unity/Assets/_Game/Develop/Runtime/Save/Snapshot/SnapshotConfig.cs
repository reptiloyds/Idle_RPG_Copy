using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using System.Linq;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Save.Snapshot
{
    [CreateAssetMenu(fileName = nameof(SnapshotConfig), menuName = "SO/" + nameof(SnapshotConfig))]
    public class SnapshotConfig : ScriptableObject
    {
        [SerializeField] private bool _useSnapshot;
        [ListDrawerSettings(
            DraggableItems = true,
            ShowFoldout = false,
            ShowPaging = false,
            HideAddButton = true,
            HideRemoveButton = true,
            OnEndListElementGUI = "DrawElementBackground"
        )]
        [SerializeField] private List<Snapshot> _snapshots = new();
        
        private void DrawElementBackground(int index)
        {
            if(!_useSnapshot) return;
            if (index != 0) return;
            var rect = GUILayoutUtility.GetLastRect();
            GUI.color = Color.green;
            GUI.Box(rect, GUIContent.none);
            GUI.color = Color.white;
        }
        
        private ISaveService _saveService;
        private IRawDataProvider _rawDataProvider;

        [Inject]
        private void Construct(ISaveService saveService, IRawDataProvider dataRepository)
        {
            _rawDataProvider = dataRepository;
            _saveService = saveService;

            if (_useSnapshot) 
                _rawDataProvider.Set(GetSnapshotData());
        }
        
        [Button, HideInEditorMode]
        public async void MakeSnapshot(string snapshotKey)
        {
            var snapshot = _snapshots.FirstOrDefault(item => string.Equals(item.SnapshotName, snapshotKey));
            if (snapshot != null)
            {
                Logger.LogError($"Snapshot with name {snapshotKey} already exists");
                return;
            }

            await _saveService.SaveAndLoadToCloudAsync();
            var rawData = _rawDataProvider.Get();
            _snapshots.Add(new Snapshot(snapshotKey));
            PlayerPrefs.SetString(snapshotKey, rawData);
            PlayerPrefs.Save();
        }

        [Button, HideInPlayMode]
        public void ClearSnapshot(string snapshotKey)
        {
            var snapshot = _snapshots.FirstOrDefault(item => string.Equals(item.SnapshotName, snapshotKey));
            if (snapshot == null)
            {
                Logger.LogError($"Snapshot with name {snapshotKey} does not exist");
                return;
            }
            _snapshots.Remove(snapshot);
            PlayerPrefs.DeleteKey(snapshot.SnapshotName);
            PlayerPrefs.Save();
        }

        private string GetSnapshotData()
        {
            var snapshotKey = _snapshots.Count == 0 ? string.Empty : _snapshots[0].SnapshotName;
            return PlayerPrefs.GetString(snapshotKey);
        }
        
        [ContextMenu("Clear All Snapshots")]
        private void ClearAllSnapshots()
        {
            var snapshotAmount = _snapshots.Count;
            for (var i = 0; i < snapshotAmount; i++) 
                ClearSnapshot(_snapshots[0].SnapshotName);
        }
        
        [System.Serializable]
        public class Snapshot
        {
            [ReadOnly]
            public string SnapshotName;

            public Snapshot(string snapshotName) => 
                SnapshotName = snapshotName;
        }
    }
}
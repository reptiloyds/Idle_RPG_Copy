using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace PleasantlyGames.RPG.Runtime.Core.Branches.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class BranchMarkHandler : MonoBehaviour
    {
        [SerializeField] private bool _showBranchMark;
        [SerializeField, Required, HideIf("@this._showBranchMark == false")]
        private BranchMarkView _branchMarkPrefab;
        [SerializeField, Required, HideIf("@this._showBranchMark == false")] private RectTransform _poolRoot;
        
        private ObjectPool<BranchMarkView> _markPool;
        private readonly Dictionary<Transform, BranchMarkView> _activeMarks = new();

        protected bool ShowBranchMark => _showBranchMark;

        protected virtual void Awake()
        {
            if (_showBranchMark) 
                CreateMarkPool();
        }
        
        private void CreateMarkPool()
        {
            _markPool = new ObjectPool<BranchMarkView>(CreateFunc, GetFunc, ReleaseFunc, DestroyFunc);
            return;

            BranchMarkView CreateFunc() => 
                Instantiate(_branchMarkPrefab, _poolRoot);

            void GetFunc(BranchMarkView view) => 
                view.gameObject.SetActive(true);
            
            void ReleaseFunc(BranchMarkView view)
            {
                view.gameObject.SetActive(false);
                view.transform.SetParent(_poolRoot);
            }
            
            void DestroyFunc(BranchMarkView view) => 
                Destroy(view.gameObject);
        }

        protected void AddMark(Transform target, Branch branch)
        {
            var mark = _markPool.Get();
            mark.Setup(branch.Sprite, target);
            _activeMarks.Add(target, mark);
        }

        protected void RemoveMark(Transform target)
        {
            if (!_activeMarks.TryGetValue(target, out var mark)) return;
            _markPool.Release(mark);
            _activeMarks.Remove(target);
        }

        protected void ClearAllMarks()
        {
            foreach (var kvp in _activeMarks) 
                _markPool.Release(kvp.Value);
            _activeMarks.Clear();
        }
    }
}
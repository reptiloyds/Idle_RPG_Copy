using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units
{
    [Serializable]
    public class SpawnPointBranch
    {
        public int BranchAmount;
        public List<UnitSpawnPoint> Points = new();
        public Transform Root;
        public Color GizmosColor;
    }
    
    [Serializable]
    public class UnitSpawnPoint
    {
        public GameObject Unit;
        public Transform Point;
    }
    
    [DisallowMultipleComponent, HideMonoScript]
    public class UnitSpawnProvider : MonoBehaviour
    {
        [SerializeField] private bool _drawGizmos = true;
        [SerializeField, HideIf("@this._drawGizmos == false")] private bool _drawAllBranches = false;
        [SerializeField, HideIf("@this._drawGizmos == false")] private bool _drawNumbers = true;
        [SerializeField, HideIf("@this._drawNumbers == false"), MinValue(1)] private int _fontSize = 16;
        [SerializeField, HideIf("@this._drawNumbers == false")] private Vector3 _numberOffset = new(0, 0.5f, 0);
        [SerializeField, MinValue(0.1f)] private float _gizmosRaduis = 0.25f;
        [SerializeField, Required] private TeamType _teamType;
        [SerializeField] private List<SpawnPointBranch> _branches = new();
        [SerializeField] private Vector3 _lookDirection;
        [Inject] private BranchService _branchService;
        
        private readonly Dictionary<GameObject, UnitSpawnPoint> _occupiedPoints = new();

        private SpawnPointBranch _currentBranch;
        private UnitSpawnPoint _middleSpawnPoint;

        public event Action OnUpdateBranch;

        public TeamType TeamType => _teamType;
        public Vector3 LookDirection => _lookDirection;

        private void Awake()
        {
            _branchService.BranchUnlock += OnBranchUnlock;
            UpdateBranch();
        }

        private void OnDestroy() => 
            _branchService.BranchUnlock -= OnBranchUnlock;

        private void OnBranchUnlock(Branch branch) => 
            UpdateBranch();

        private void UpdateBranch()
        {
            var branchAmount = _branchService.UnlockedBranchesCount();
            _currentBranch = null;
            foreach (var branch in _branches)
            {
                if(branch.BranchAmount != branchAmount) continue;
                _currentBranch = branch;
                break;
            }

            _currentBranch ??= _branches[^1];

            _middleSpawnPoint = FindMiddleSpawnPoint();
            
            OnUpdateBranch?.Invoke();
        }

        [Button]
        public void Add(int branchAmount, Transform root)
        {
            List<UnitSpawnPoint> children = new();
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                children.Add(new UnitSpawnPoint()
                {
                    Point = child,
                });
            }

            var branch = _branches.FirstOrDefault(item => item.BranchAmount == branchAmount);
            if (branch != null) 
                _branches.Remove(branch);
            
            _branches.Add(new SpawnPointBranch
            {
                BranchAmount = branchAmount,
                Points = children,
                Root = root
            });
            
            _branches = _branches.OrderBy(item => item.BranchAmount).ToList();
        }

        public List<UnitSpawnPoint> GetCurrentPoints() => 
            _currentBranch.Points;

        public void Remove(int branchAmount)
        {
            foreach (var branch in _branches)
            {
                if(branch.BranchAmount != branchAmount) continue;
                _branches.Remove(branch);
                break;
            }
            
            _branches = _branches.OrderBy(item => item.BranchAmount).ToList();
        }

        public UnitSpawnPoint GetFreePoint()
        {
            foreach (var point in _currentBranch.Points)
                if (point.Unit == null) return point;

            return null;
        }

        public UnitSpawnPoint GetRandomFreePoint()
        {
            var skipCount = new Vector2Int(0, _currentBranch.Points.Count - _occupiedPoints.Count - 1).Random();

            foreach (var point in _currentBranch.Points)
            {
                if(point.Unit != null) continue;
                if (skipCount == 0) return point;
                skipCount--;
            }

            return null;
        }

        public UnitSpawnPoint GetMiddleSpawnPoint() =>
            _middleSpawnPoint;

        public UnitSpawnPoint GetSpawnPoint(int id)
        {
            if (id >= _currentBranch.Points.Count || id < 0) return null;
            return _currentBranch.Points[id];
        }
        
        public void Occupy(GameObject occupant, UnitSpawnPoint spawnPoint)
        {
            _occupiedPoints.Add(occupant, spawnPoint);
            spawnPoint.Unit = occupant;
        }

        public void Free(GameObject occupant)
        {
            if (!_occupiedPoints.TryGetValue(occupant, out var spawnPoint)) return;
            spawnPoint.Unit = null;
            _occupiedPoints.Remove(occupant);
        }
        
        private UnitSpawnPoint FindMiddleSpawnPoint()
        {
            var sum = Vector3.zero;
            foreach (var spawnPoint in _currentBranch.Points) 
                sum += spawnPoint.Point.position;
            
            var centerPoint = sum / _currentBranch.Points.Count;
            UnitSpawnPoint closestSpawnPoint = null;
            var fakeMinDistance = float.MaxValue;

            foreach (var spawnPoint in _currentBranch.Points)
            {
                var fakeDistance = Vector3.SqrMagnitude(spawnPoint.Point.position - centerPoint);
                if (fakeDistance >= fakeMinDistance) continue;
                
                fakeMinDistance = fakeDistance;
                closestSpawnPoint = spawnPoint;
            }

            return closestSpawnPoint;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if(!_drawGizmos) return;

            if (Application.isPlaying)
            {
                if(_currentBranch != null)
                    DrawBranch(_currentBranch);   
            }
            else
            {
                if (_drawAllBranches)
                {
                    foreach (var branch in _branches) 
                        DrawBranch(branch);
                }
                else
                {
                    var selectedTransform = Selection.activeTransform;
                    if(selectedTransform == null) return;
                    foreach (var branch in _branches)
                    {
                        if (branch.Root != selectedTransform && !selectedTransform.IsChildOf(branch.Root)) continue;
                        DrawBranch(branch);
                        break;
                    }   
                }      
            }
#endif
        }

        private void DrawBranch(SpawnPointBranch branch)
        {
            for (var i = 0; i < branch.Points.Count; i++)
            {
                var point = branch.Points[i];
                DrawPoint(point.Point.position, _gizmosRaduis, branch.GizmosColor);
                if (_drawNumbers) 
                    DrawTextInSceneView($"{i + 1}", point.Point.position, branch.GizmosColor);
            }
        }

        private static void DrawPoint(Vector3 center, float radius, Color color)
        {
            const int segments = 5;
            float angle = 0;

            Gizmos.color = color;

            var from = center + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;
            for (var i = 0; i < segments + 1; i++)
            {
                var to = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                Gizmos.DrawLine(from, to);
                from = to;
                angle += 2 * Mathf.PI / segments;
            }
        }
        
        private void DrawTextInSceneView(string text, Vector3 worldPosition, Color textColor)
        {
#if UNITY_EDITOR
            var style = new GUIStyle
            {
                normal =
                {
                    textColor = textColor
                },
                fontSize = _fontSize,
                alignment = TextAnchor.MiddleCenter
            };

            Handles.Label(worldPosition + _numberOffset, text, style);      
#endif
        }
    }
}
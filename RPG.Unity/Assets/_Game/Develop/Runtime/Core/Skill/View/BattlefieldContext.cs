using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Skill.Contract;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BattlefieldContext : MonoBehaviour, ISkillLocationDataProvider
    {
        [Serializable]
        public class BattlefieldSetup
        {
            public int BranchAmount;
            public Vector3 ColliderCenter;
            public Vector3 ColliderSize;
            public Vector3 SkillCastOffset;
            public Vector2 SkillCastArea;
            public Vector2 EmptyCastArea;
        }
        
        [SerializeField, Required] private BoxCollider _boundCollider;
        [SerializeField] private Color _castAreaColor = Color.red;
        [SerializeField] private Color _emptyCastAreaColor = Color.yellow;
        [SerializeField] private List<BattlefieldSetup> _setups = new();
        [SerializeField, HideInInspector] private BattlefieldSetup _editorSetup;
        [SerializeField] private bool _detectUnitsInCameraView = true;
        [HideIf("@this._detectUnitsInCameraView == false"), Range(0, 1)] private float _detectionCameraOffset = 0.02f;

        [Inject] private LocationUnitCollection _locationUnitCollection;
        [Inject] private BranchService _branchService;
        [Inject] private SkillTargetProvider _skillTargetProvider;
        
        private BattlefieldSetup _currentBranch;
        private CancellationToken _cancellationToken;
        private readonly List<UnitView> _invisibleUnitBuffer = new(10);
        private UnityEngine.Camera _camera;

        [Button]
        public void Draw(int branchAmount)
        {
            _editorSetup = _setups.FirstOrDefault(item => item.BranchAmount == branchAmount);
            if(_editorSetup == null) return;
            _boundCollider.center = _editorSetup.ColliderCenter;
            _boundCollider.size = _editorSetup.ColliderSize;
        }

        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
            _skillTargetProvider.SetSkillData(this);
            _branchService.BranchUnlock += OnBranchUnlock;
            UpdateBranch();
            _invisibleUnitBuffer.Clear();
            _cancellationToken = this.GetCancellationTokenOnDestroy();
        }

        private void OnDestroy()
        {
            _branchService.BranchUnlock -= OnBranchUnlock;
            _locationUnitCollection.Clear();
        }

        Vector3 ISkillLocationDataProvider.GetCenterXZPosition() => 
            transform.position + new Vector3(_currentBranch.SkillCastOffset.x, 0, _currentBranch.SkillCastOffset.y);

        Vector3 ISkillLocationDataProvider.GetCenterPosition() =>
            _boundCollider.transform.TransformPoint(_boundCollider.center);

        Vector3 ISkillLocationDataProvider.FindClosestPoint(Vector3 targetPosition)
        {
            var boxCenter = _boundCollider.transform.TransformPoint(_boundCollider.center);
            var boxSize = _boundCollider.size;
            
            var boxMin = boxCenter - boxSize * 0.5f;
            var boxMax = boxCenter + boxSize * 0.5f;
            
            var clampedX = Mathf.Clamp(targetPosition.x, boxMin.x, boxMax.x);
            var clampedY = Mathf.Clamp(targetPosition.y, boxMin.y, boxMax.y);
            var clampedZ = Mathf.Clamp(targetPosition.z, boxMin.z, boxMax.z);
            
            return new Vector3(clampedX, clampedY, clampedZ);
        }

        Vector3 ISkillLocationDataProvider.GetRandomPosition()
        {
            var center = transform.position + new Vector3(_currentBranch.SkillCastOffset.x, 0, _currentBranch.SkillCastOffset.y);
            var randomX = new Vector2(center.x - _currentBranch.SkillCastArea.x * 0.5f, center.x + _currentBranch.SkillCastArea.x * 0.5f).Random();
            var randomZ = new Vector2(center.z - _currentBranch.SkillCastArea.y * 0.5f, center.z + _currentBranch.SkillCastArea.y * 0.5f).Random();
            
            return new Vector3(randomX, transform.position.y, randomZ);
        }

        Vector3 ISkillLocationDataProvider.GetRandomEmptyPosition()
        {
            var center = transform.position + new Vector3(_currentBranch.SkillCastOffset.x, 0, _currentBranch.SkillCastOffset.y);
            var randomX = new Vector2(center.x - _currentBranch.EmptyCastArea.x * 0.5f, center.x + _currentBranch.EmptyCastArea.x * 0.5f).Random();
            var randomZ = new Vector2(center.z - _currentBranch.EmptyCastArea.y * 0.5f, center.z + _currentBranch.EmptyCastArea.y * 0.5f).Random();
            
            return new Vector3(randomX, transform.position.y, randomZ);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out UnitView unit)) return;
            // if (unit.TeamType != TeamType.Enemy) return;
            if (_detectUnitsInCameraView && unit.TeamType == TeamType.Enemy)
                _invisibleUnitBuffer.Add(unit);
            else
                _locationUnitCollection.AppendUnit(unit);
        }

        public void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out UnitView unit)) return;
            // if (unit.TeamType != TeamType.Enemy) return;
            if (_detectUnitsInCameraView && unit.TeamType == TeamType.Enemy)
                _invisibleUnitBuffer.Remove(unit);
            else
                _locationUnitCollection.RemoveUnit(unit);
        }

        private void Update()
        {
            for (int i = 0; i < _invisibleUnitBuffer.Count; i++)
            {
                var unit = _invisibleUnitBuffer[i];
                if (IsVisible(_invisibleUnitBuffer[i].transform))
                {
                    _locationUnitCollection.AppendUnit(unit);
                    _invisibleUnitBuffer.RemoveAt(i);
                    i--;
                }
            }
        }
        
        bool IsVisible(Transform target)
        {
            Vector3 viewportPoint = _camera.WorldToViewportPoint(target.position);
            var reverseOffset = 1 - _detectionCameraOffset;
            bool isVisible = viewportPoint.x > _detectionCameraOffset && viewportPoint.x < reverseOffset && viewportPoint.y > _detectionCameraOffset && viewportPoint.y < reverseOffset;
            return isVisible;
        }

        private void OnBranchUnlock(Branch branch) => 
            UpdateBranch();

        private void UpdateBranch()
        {
            var branchAmount = _branchService.UnlockedBranchesCount();
            _currentBranch = null;
            foreach (var branch in _setups)
            {
                if(branch.BranchAmount != branchAmount) continue;
                _currentBranch = branch;
                break;
            }

            _currentBranch ??= _setups[^1];
            
            _boundCollider.center = _currentBranch.ColliderCenter;
            _boundCollider.size = _currentBranch.ColliderSize;
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                if (_currentBranch != null)
                {
                    DrawBranch(_currentBranch.SkillCastOffset, _currentBranch.SkillCastArea, _castAreaColor);   
                    DrawBranch(_currentBranch.SkillCastOffset, _currentBranch.EmptyCastArea, _emptyCastAreaColor);
                }
            }
            else if (_editorSetup != null)
            {
                DrawBranch(_editorSetup.SkillCastOffset, _editorSetup.SkillCastArea, _castAreaColor);   
                DrawBranch(_editorSetup.SkillCastOffset, _editorSetup.EmptyCastArea, _emptyCastAreaColor);   
            }
        }

        private void DrawBranch(Vector3 offset, Vector3 area, Color color)
        {
            var center = transform.position + new Vector3(offset.x, 0, offset.y);
                
            var halfX = area.x * 0.5f;
            var halfZ = area.y * 0.5f;
            
            var topLeft = new Vector3(center.x - halfX, center.y, center.z + halfZ);
            var topRight = new Vector3(center.x + halfX, center.y, center.z + halfZ);
            var bottomLeft = new Vector3(center.x - halfX, center.y, center.z - halfZ);
            var bottomRight = new Vector3(center.x + halfX, center.y, center.z - halfZ);
            
            Gizmos.color = color;
            
            Gizmos.DrawLine(topLeft, topRight);       
            Gizmos.DrawLine(topRight, bottomRight);   
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
            
            Gizmos.color = Color.white;   
        }

        public void Clear()
        {
            _invisibleUnitBuffer.Clear();
            _locationUnitCollection.Clear();
            if (gameObject != null)
            {
                gameObject.SetActive(false);
                ActivateOnNextFrameAsync().Forget();
            }
        }

        private async UniTaskVoid ActivateOnNextFrameAsync()
        {
            await UniTask.NextFrame(_cancellationToken);
            if(gameObject != null)
                gameObject.SetActive(true);
        }
    }
}
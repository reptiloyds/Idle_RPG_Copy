using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Health;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Movement;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Rotation;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using Debug = UnityEngine.Debug;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public sealed class UnitView : MonoBehaviour
    {
        [ShowInInspector, ReadOnly, HideInEditorMode] private string _id;
        [ShowInInspector, ReadOnly, HideInEditorMode] private string _prefabId;
        [ShowInInspector, ReadOnly, HideInEditorMode] private TeamType _teamType;
        [ShowInInspector, ReadOnly, HideInEditorMode] private List<UnitSubType> _subTypes;
        [ShowInInspector, ReadOnly, HideInEditorMode] private readonly Dictionary<string, Vector3> _positions = new();
        [ShowInInspector, ReadOnly, HideInEditorMode] private readonly Dictionary<string, Vector3> _lookDirection = new();
        [ShowInInspector, HideInEditorMode] private IReadOnlyList<UnitStat> _stats;

        [SerializeField, ReadOnly] private UnitData _data;
        
        [SerializeField, FoldoutGroup("Base")] private GameObject _visual;
        [SerializeField, FoldoutGroup("Base")] private Transform _projectileTarget;
        [SerializeField, FoldoutGroup("Base")] private Transform _particlePoint;
        [SerializeField, FoldoutGroup("Base")] private bool _overrideColliderData = false;
        [SerializeField, FoldoutGroup("Base")] private CapsuleCollider _capsuleCollider;

        [SerializeField, FoldoutGroup("Components")] private List<UnitComponent> _components;
        [SerializeField, FoldoutGroup("Components")] private BaseBrain _brain;
        [SerializeField, FoldoutGroup("Components")] private UnitHealthPresenter _healthPresenter;
        [SerializeField, FoldoutGroup("Components")] private BaseMovement _movement;
        [SerializeField, FoldoutGroup("Components")] private BaseRotator _rotator;
        [SerializeField, FoldoutGroup("Components")] private Combat.Combat _combat;
        [SerializeField, FoldoutGroup("Components")] private Targets _targets;
        [SerializeField, FoldoutGroup("Components")] private AttackSpeed _attackSpeed;
        [SerializeField, FoldoutGroup("Components")] private Equipment _equipment;
        [SerializeField, FoldoutGroup("Components")] private VisualStateMachine _stateMachine;
        [SerializeField, FoldoutGroup("Components")] private EnemyTargets _enemyTargets;

        [Inject] private HealthBarFactory _healthBarFactory;
        
        private bool _isInitialized;
        private bool _isDead;
        private float _modelHeight;
        private string _targetKey;

        private UnitHealth _health;

        public Transform ParticlePoint => _particlePoint;
        public Transform ProjectileTarget => _projectileTarget;
        public float Radius => _movement.Radius;
        public float Height => _movement.Height;
        public UnitHealth Health => _health;
        public BaseMovement BaseMovement => _movement;
        public BaseRotator Rotator => _rotator;
        public Targets Targets => _targets;
        public EnemyTargets EnemyTargets => _enemyTargets;
        public Combat.Combat Combat => _combat;
        public AttackSpeed AttackSpeed => _attackSpeed;
        public Equipment Equipment => _equipment;
        public VisualStateMachine StateMachine => _stateMachine;

        public event Action<UnitView> OnAchieveTarget;
        public event Action<UnitView> OnDie;

        public GameObject Visual => _visual;
        public string TargetKey => _targetKey;
        public bool HasTargetKey => !_targetKey.IsNullOrWhitespace();
        public string Id => _id;
        public string PrefabId => _prefabId;
        public TeamType TeamType => _teamType;
        public bool IsDead => _isDead;
        public bool IsActive => gameObject.activeSelf && !_isDead;
        public Transform HealthBarPoint => _healthPresenter.HealthBarPoint;
        public List<UnitSubType> SubTypes => _subTypes;
        public UnitData.Render Render => _data.RenderData;

        public event Action SpawnEvent;
        public event Action<UnitView> DisposeEvent;

        #region Setters

        public void SetStats(IReadOnlyList<UnitStat> stats) => _stats = stats;
        public void SetId(string id, string prefabId)
        {
            _id = id;
            _prefabId = prefabId;
        }

        public void SetTeamType(TeamType teamType) => _teamType = teamType;
        public void SetSubTypes(List<UnitSubType> types) => _subTypes = types;

        #endregion
        
        private void Reset() =>
            GetComponents();
        
        private void OnValidate() => 
            GetComponents();

        private void GetComponents()
        {
            _brain ??= GetComponentInChildren<Brain.Brain>();
            _healthPresenter ??= GetComponentInChildren<UnitHealthPresenter>();
            _movement ??= GetComponentInChildren<BaseMovement>();
            _rotator ??= GetComponentInChildren<BaseRotator>();
            _combat ??= GetComponentInChildren<Combat.Combat>();
            _attackSpeed ??= GetComponentInChildren<AttackSpeed>();
            _targets ??= GetComponentInChildren<Targets>();
            _equipment ??= GetComponentInChildren<Equipment>();
            _stateMachine ??= GetComponentInChildren<VisualStateMachine>();
            _enemyTargets ??= GetComponentInChildren<EnemyTargets>();
            _components = GetComponentsInChildren<UnitComponent>().ToList();
        }

        public void SetEnemyTargets(EnemyTargets targets) => 
            _enemyTargets = targets;

        public void SetData(UnitData data)
        {
            if(data == null) return;
            _data = data;
            ApplyData();
        }

        private void ApplyData()
        {
            _visual.transform.localPosition = _data.VisualOffset;
            _projectileTarget.localPosition = Vector3.up * _data.ProjectileOffset; 
            _healthPresenter.SetupData(_data.HealthData);
            _movement.SetupData(_data.MovementData);
            if (!_overrideColliderData)
            {
                _capsuleCollider.height = _data.PhysicsData.Height;
                _capsuleCollider.center = Vector3.up * (_data.PhysicsData.Height / 2);
                _capsuleCollider.radius = _data.PhysicsData.Radius;
            }

            _combat.SetupData(_data.CombatData);
        }

        public void SetParticlePoint(Transform particleTarget) => 
            _particlePoint = particleTarget;

        public void Initialize()
        {
            if (!_isInitialized)
            {
                ApplyData();
                _modelHeight = _visual.transform.localPosition.y;
                foreach (var unitComponent in _components) 
                    unitComponent.Initialize();
                _isInitialized = true;
            }
            OnSpawn();
        }

        private void OnSpawn()
        {
            _isDead = false;
            foreach (var unitComponent in _components) 
                unitComponent.OnSpawn();
            
            SpawnEvent?.Invoke();
        }

        private void OnZeroHealth()
        {
            _isDead = true;
            OnDie?.Invoke(this);
        }

        public void Dispose()
        {
            _positions.Clear();
            _lookDirection.Clear();
            
            foreach (var unitComponent in _components) 
                unitComponent.Dispose();

            _targetKey = null;
            
            DisposeEvent?.Invoke(this);
            _stats = null;
        }

        public void SetHealth(UnitHealth health)
        {
            _healthPresenter.Set(health);
            _health = health;
            _health.OnZeroHealth += OnZeroHealth;
        }

        public void RemoveHealth()
        {
            _healthPresenter.Remove();
            _health.OnZeroHealth -= OnZeroHealth;
            _health = null;
        }

        private void OnDestroy() => Dispose();

        public UnitStat GetStat(UnitStatType type)
        {
            foreach (var stat in _stats)
            {
                if (stat.Type != type) continue;
                return stat;
            }

            return null;
        }

        public IReadOnlyList<UnitStat> GetStats() => _stats;

        public void SetPosition(Vector3 position) => 
            _movement.SetPosition(position);

        public void SetRotation(Vector3 direction)
        {
            _movement.SetRotation(direction);
            _rotator.LookInstantly(direction);
            _enemyTargets.FixPoints();
        }

        public void AppendPosition(string key, Vector3 position) => 
            _positions.Add(key, position);
        
        public void AppendLookDirection(string key, Vector3 direction) =>
            _lookDirection.Add(key, direction);

        public (bool success, Vector3 position) GetPosition(string key)
        {
            var success = _positions.TryGetValue(key, out var position);
            return (success, position);
        }
        
        public (bool success, Vector3 direction) GetLookDirection(string key)
        {
            var success = _lookDirection.TryGetValue(key, out var direction);
            return (success, direction);
        }

        public Transform GetTargetForEnemy(GameObject key) => 
            _enemyTargets.GetTarget(key);

        public void ReleaseTargetForEnemy(GameObject key) => 
            _enemyTargets.ReleasePoint(key);

        public void SetupTargetKey(string targetKey) => 
            _targetKey = targetKey;

        public void SetBehaviour(UnitBehaviourType type) => 
            _brain.SwitchBehaviour(type);

        [Button, HideInEditorMode]
        public void TakeDamage(BigDouble.Runtime.BigDouble value)
        {
            if (_health == null)
            {
                Debug.LogWarning("HealthIsNull");
                return;
            }
            _healthPresenter.BeforeDamage();
            _health.ApplyDamage(value);
            _healthPresenter.AfterDamage();
        }

        [Button, HideInEditorMode]
        public void Heal(float value) => 
            _health.ApplyHeal(value);

        [Button, HideInEditorMode]
        public void FullHealth() => 
            _health.FullHealth();

        internal void AchieveMovementTarget()
        {
            _targetKey = null;
            OnAchieveTarget?.Invoke(this);
        }

        public bool Has(UnitSubType subType) => 
            _subTypes.Contains(subType);

        public void NormalizeModelHeight()
        {
            var localPosition = _visual.transform.localPosition;
            _visual.transform.localPosition = new Vector3(localPosition.x, _modelHeight, localPosition.z);
        }

        public void SetModelHeight(float height)
        {
            var localPosition = _visual.transform.localPosition;
            _visual.transform.localPosition = new Vector3(localPosition.x, height, localPosition.z);
        }

        private void OnDrawGizmos()
        {
            if(_projectileTarget == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_projectileTarget.position, 0.2f);
            Gizmos.color = Color.white;
        }

#if RPG_PROD
        [Conditional("DUMMY_UNUSED_DEFINE")]
#endif
        public void SetStatValue_CheatOnly(UnitStatType type, int value)
        {
            foreach (var stat in _stats)
            {
                if(stat.Type != type) continue;
                stat.SetValue_CheatOnly(value);
            }
        }
    }
}
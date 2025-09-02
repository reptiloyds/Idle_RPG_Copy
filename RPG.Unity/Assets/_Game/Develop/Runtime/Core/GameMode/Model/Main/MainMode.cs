using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement.Forecast;
using PleasantlyGames.RPG.Runtime.Core.Ally;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Companion;
using PleasantlyGames.RPG.Runtime.Core.Enemy;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Definition;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Type;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using StatModifier = PleasantlyGames.RPG.Runtime.Core.Stats.Model.StatModifier;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main
{
    public class MainMode : IGameMode, IAssetUser, ITickable
    {
        private enum EnemyType
        {
            SequencedWave = 0,
            RandomWave = 1,
            Boss = 2,
        }

        [Inject] private BalanceContainer _balance;
        [Inject] private LocationFactory _locationFactory;
        [Inject] private CompanionSquad _companionSquad;
        [Inject] private ItemSkillService _itemSkillService;
        [Inject] private CharacterSkillService _characterSkillService;
        [Inject] private AllySquad _allySquad;
        [Inject] private EnemySquad _enemySquad;
        [Inject] private LocationUnitCollection _locationUnits;
        [Inject] private MainModeConfiguration _configuration;
        [Inject] private ResourceService _resourceService;
        [Inject] private TimeService _time;
        
        [Inject] private ITranslator _translator;
        [Inject] private GlobalStatProvider _globalStatProvider;

        private UnityEngine.Camera _camera;
        private GlobalStat _bossTimeStat;
        private GlobalStat _incomeStat;
        private MainModeSheet _sheet;
        private MainModeSheet Sheet => _sheet ??= _balance.Get<MainModeSheet>();
        private MainModeDataContainer _data;
        private int _waveIndex;
        private MainModeSheet.Row _stageConfig;
        private MainModeSheet.Elem _levelConfig;

        private StatModifier _locationSpeedModifier;
        private bool _isBossFight = false;
        private int _enemyLevel;
        private bool _isLaunched;
        private string _fullName;
        private string _modeName;

        private EnemyType _enemyType;
        private bool _isMoveToEnemy;
        private float _movementTimer;
        
        private readonly Vector3 _spawnRewardOffset = Vector3.up;
        private const int MAX_VIEW_REWARD = 5;

        private const string StatModifierName = "MainMode";
        private const float BOSS_MOVEMENT_DURATION = 0.01f;
        private readonly Dictionary<UnitStatType, StatModifier> _enemyModifiers = new();
        private readonly Dictionary<UnitStatType, StatModifier> _bossModifiers = new();
        
        private readonly SerialDisposable _loseTimer = new();
        private readonly ReactiveProperty<float> _loseDelay = new();
        public ReadOnlyReactiveProperty<float> LoseDelay => _loseDelay;
        
        public List<List<EnemyData>> Waves => _levelConfig.Waves;
        public int WaveIndex => _waveIndex;
        public int WaveAmount => _levelConfig.Waves.Count;
        public bool HasBossOnLevel => !string.IsNullOrEmpty(_levelConfig.BossUnitId);
        public bool WaveIsCleared => _data.WavesIsCleared;
        public bool IsBossFight => _isBossFight;
        public float SwitchWaveDuration => _configuration.SwitchWaveDuration;
        public float SwitchWaveDurationUI => _configuration.SwitchWaveDurationUI;
        public int Id => _data.Id;
        public int Level => _data.Level;
        public GameModeType Type => GameModeType.Main;
        public bool IsLaunched => _isLaunched;
        
        public event Action<IGameMode> OnLaunched;
        public event Action OnRestarted;
        public event Action<IGameMode> OnDisposed;
        public event Action OnBossTriggered;
        public event Action OnBossLaunched;
        public event Action OnBossDefeated;
        public event Action OnWaveIncremented;
        public event Action<IGameMode> OnLose;
        public event Action<IGameMode> OnWin;
        public event Action OnLevelChanged;
        public event Action OnLevelEntered; 
        public event Action OnBoseLose;
        public event Action<UnitView> OnEnemyDied;
        
        [Preserve]
        public MainMode() { }

        public void SetData(MainModeDataContainer data)
        {
            _data = data;
            _stageConfig = Sheet[_data.Id];
            _levelConfig = _stageConfig.Arr.FirstOrDefault(item => item.Level == _data.Level);
        }

        void IGameMode.Initialize()
        {
            _modeName = _translator.Translate(Type.ToString());
            _camera = UnityEngine.Camera.main;
            _bossTimeStat = _globalStatProvider.GetStat(GlobalStatType.BossTime);
            _incomeStat = _globalStatProvider.GetStat(GlobalStatType.Income);
        }

        public string GetFormattedStageName(int id) => 
            Sheet.TryGetValue(id, out var row) ? _translator.Translate(row.Complexity) : string.Empty;

        public int GetStageNumber(int id)
        {
            if (id >= Sheet.Count) return -1;
            var stageConfig = Sheet[id];
            foreach (var row in Sheet)
            {
                if (!string.Equals(row.Complexity, stageConfig.Complexity)) continue;
                return stageConfig.Id - Sheet.IndexOf(row) + 1;
            }

            return -1;
        }

        async UniTask IGameMode.LaunchAsync()
        {
            _isLaunched = true;
            _itemSkillService.UnlockAutoCast();
            _characterSkillService.UnlockAutoCast();
            
            UpdateStaticData();
            await PrepareModeAsync();
            WalkToEnemy(_waveIndex < 0 ? EnemyType.RandomWave : EnemyType.SequencedWave);
            OnLaunched?.Invoke(this);
            OnLevelEntered?.Invoke();
        }

        void IGameMode.Dispose()
        {
            _isLaunched = false;
            CleanUp();
            _itemSkillService.BlockAutoCast();
            _characterSkillService.BlockAutoCast();
            OnDisposed?.Invoke(this);
        }

        private void CleanUp()
        {
            _isBossFight = false;
            _isMoveToEnemy = false;
            _loseTimer.Disposable?.Dispose();

            _locationUnits.OnUnitEnter -= OnLocalUnitEnter;
            _enemySquad.OnDie -= OnEnemyDie;
            _enemySquad.OnAllUnitsDead -= WinWave;
            _allySquad.OnAllUnitsDead -= LoseWave;
            _allySquad.OnAchieveMovementTarget -= OnAllyAchieveMovementTarget;
            _companionSquad.OnAchieveMovementTarget -= OnAllyAchieveMovementTarget;
            
            _allySquad.Clear();
            _companionSquad.Clear();
            _enemySquad.Clear();
            _itemSkillService.StopSkills();
            _characterSkillService.StopSkills();
        }

        public void OnResultPresented()
        {
            if(_isLaunched)
                RestartAsync().Forget();
        }

        public void OnResultWasClosed() => 
            OnLevelEntered?.Invoke();

        private async UniTask RestartAsync()
        {
            CleanUp();
            UpdateStaticData();
            await PrepareModeAsync();
            WalkToEnemy(_waveIndex < 0 ? EnemyType.RandomWave : EnemyType.SequencedWave);
            OnRestarted?.Invoke();
        }

        public string GetFormattedFullName() => 
            _fullName;

        public string GetFormattedModeName() => 
            _modeName;

        private void UpdateStaticData()
        {
            _stageConfig = Sheet[_data.Id];
            UpdateLevelConfig();
            UpdateModifiers();
            _fullName = $"{_translator.Translate(_stageConfig.Complexity)} {GetStageNumber(_stageConfig.Id)} - {_levelConfig.Level}";
            _enemyLevel = CalculateEnemyLevel();
            
            OnNeedsChanged?.Invoke();
        }

        private async UniTask PrepareModeAsync()
        {
            _locationFactory.ClearCurrentLocation();
            await _locationFactory.CreateAsync(_levelConfig.LocationId);
            _allySquad.SetSpawn(_locationFactory.Location.AllySpawn);
            _companionSquad.SetSpawn(_locationFactory.Location.CompanionSpawn);
            _enemySquad.SetSpawn(_locationFactory.Location.EnemySpawn);
            
            _allySquad.MergeHealth();
            await UniTask.WhenAll(_allySquad.SpawnUnitsAsync(), _companionSquad.SpawnUnitsAsync());
            
            _allySquad.OnAchieveMovementTarget += OnAllyAchieveMovementTarget;
            _companionSquad.OnAchieveMovementTarget += OnAllyAchieveMovementTarget;
            _enemySquad.OnDie += OnEnemyDie;
            
            _locationSpeedModifier = new StatModifier(_configuration.BonusUnitSpeed, StatModType.Flat, StatModifierName, GroupOrder.None);
            _waveIndex = _data.WavesIsCleared ? -1 : 0;
            
            _itemSkillService.ResetAllSkill();
            _characterSkillService.ResetAllSkill();
        }

        public void Tick()
        {
            if(!_isLaunched) return;

            if (!_isMoveToEnemy) return;
            _movementTimer -= Time.deltaTime;
            if (_movementTimer > 0) return;
            _isMoveToEnemy = false;
            CreateEnemyAsync(_enemyType).Forget();
        }

        private void OnEnemyDie(UnitView unitView)
        {
            OnEnemyDied?.Invoke(unitView);
            var reward = unitView.GetStat(UnitStatType.Reward);
            if(reward == null || reward.Value == 0) return;
            SpawnReward(reward.Value, unitView.transform.position);
        }

        private void SpawnReward(BigDouble.Runtime.BigDouble value, Vector3 worldPosition)
        {
            var rewardValue = BigDouble.Runtime.BigDouble.Ceiling(value * _incomeStat.Value);
            var rewardPosition = _camera.WorldToScreenPoint(worldPosition + _spawnRewardOffset);
            _resourceService.AddResource(ResourceType.Soft, rewardValue, ResourceFXRequest.Create(spawnPosition: rewardPosition,
                context: PopupIconContext.Gameplay, maxViewReward: MAX_VIEW_REWARD, forceToTarget: true));
        }

        private void WalkToEnemy(EnemyType enemyType)
        {
            _allySquad.SimulateMovement();
            _companionSquad.SimulateMovement();
            
            _locationFactory.Location.StartMovement(_configuration.LocationSpeed);

            _enemyType = enemyType;
            _isMoveToEnemy = true;
            if (_enemyType == EnemyType.Boss)
                _movementTimer = BOSS_MOVEMENT_DURATION;
            else
                _movementTimer = SwitchWaveDuration;
        }

        private async UniTask CreateEnemyAsync(EnemyType enemyType)
        {
            _enemySquad.OnAllUnitsDead += WinWave;
            _allySquad.OnAllUnitsDead += LoseWave;
            _locationUnits.OnUnitEnter += OnLocalUnitEnter;

            switch (enemyType)
            {
                case EnemyType.SequencedWave:
                    await SpawnWaveAsync(_levelConfig.Waves[_waveIndex]);
                    break;
                case EnemyType.RandomWave:
                    await SpawnWaveAsync(_levelConfig.Waves.GetRandomElement());
                    break;
                case EnemyType.Boss:
                    await SpawnBossAsync();
                    break;
            }

            _enemySquad.AddModifier(UnitStatType.MoveSpeed, _locationSpeedModifier);
            _enemySquad.MoveForward();
        }

        private void WinWave()
        {
            _enemySquad.OnAllUnitsDead -= WinWave;
            _allySquad.OnAllUnitsDead -= LoseWave;
            
            if (_waveIndex >= 0)
                IncrementWave();

            if (_isBossFight)
            {
                _loseTimer.Disposable?.Dispose();
                OnBossDefeated?.Invoke();
            }

            if (_isBossFight || (_data.WavesIsCleared && !HasBossOnLevel))
                WinLevel();
            else
            {
                _allySquad.MoveToSpawn(UnitPointKey.Spawn);
                _companionSquad.MoveToSpawn(UnitPointKey.Spawn);
            }
        }

        private void LoseWave()
        {
            _loseTimer.Disposable?.Dispose();
            _enemySquad.OnAllUnitsDead -= WinWave;
            _allySquad.OnAllUnitsDead -= LoseWave;
            
            _allySquad.Idle();
            _companionSquad.Idle();
            _enemySquad.Idle();

            OnLose?.Invoke(this);
            if (_isBossFight)
                OnBoseLose?.Invoke();
        }

        public void CheatSwitch(int id, int level)
        {
            _enemySquad.Idle();
            _allySquad.MoveForward();
            _companionSquad.MoveForward();
            
            if (Sheet.Contains(id))
                _data.Id = id;
            else
            {
                var stageConfig = Sheet.Last();
                _data.Id = stageConfig.Id;
            }

            if (Sheet[_data.Id].Arr.FirstOrDefault(item => item.Level == level) != null)
                _data.Level = level;
            else
                _data.Level = Sheet[_data.Id].Arr.Last().Level;

            _data.WavesIsCleared = false;
            
            OnLevelChanged?.Invoke();
            OnWin?.Invoke(this);
        }

        private void WinLevel()
        {
            _enemySquad.Idle();
            _allySquad.MoveForward();
            _companionSquad.MoveForward();
            IncreaseLevel();
            
            OnWin?.Invoke(this);
        }

        private void IncrementWave()
        {
            _waveIndex++;
            if (_levelConfig.Waves.Count == _waveIndex)
                _data.WavesIsCleared = true;
            OnWaveIncremented?.Invoke();
        }

        private void IncreaseLevel()
        {
            if (_data.Level == _stageConfig.GetMaxLevel())
            {
                if (Sheet.IndexOf(_stageConfig) < Sheet.Count - 1)
                {
                    _data.Id++;
                    _data.Level = 1;  
                } 
            }
            else
                _data.Level++;

            _data.WavesIsCleared = false;
            OnLevelChanged?.Invoke();
        }

        private void OnAllyAchieveMovementTarget()
        {
            if(_allySquad.HasAnyMovementTarget() || _companionSquad.HasAnyMovementTarget()) return;
            
            ExecuteAllyWinActions();

            if (_data.WavesIsCleared && _waveIndex >= 0)
                TriggerBoss();
            else
                WalkToEnemy(_waveIndex < 0 ? EnemyType.RandomWave : EnemyType.SequencedWave);
        }

        private void ExecuteAllyWinActions()
        {
            foreach (var actionType in _configuration.ActionAfterAllyWin)
                ExecuteAllyAction(actionType);
        }

        private void ExecuteAllyAction(SquadActionType actionType)
        {
            switch (actionType)
            {
                case SquadActionType.FullHealth:
                    _allySquad.FullHealth();
                    break;
            }
        }

        private void OnLocalUnitEnter(UnitView unitView)
        {
            if (unitView.TeamType != _enemySquad.TeamType) return;
            
            _locationUnits.OnUnitEnter -= OnLocalUnitEnter;
            StartCombat();
        }

        private void StartCombat()
        {
            _enemySquad.RemoveModifier(UnitStatType.MoveSpeed, _locationSpeedModifier);
            _locationFactory.Location.StopMovement();
            _allySquad.Fight();
            _companionSquad.Fight();
            _enemySquad.Fight();
        }

        public void TriggerBoss()
        {
            CleanUp();
            OnBossTriggered?.Invoke();
        }

        public async UniTask LaunchBoss()
        {
            await PrepareModeAsync();
            WalkToEnemy(EnemyType.Boss);

            _loseDelay.Value = (float)_bossTimeStat.Value.ToDouble();
            _time.LaunchLocalTimer(_loseTimer, _loseDelay, LoseWave);
            OnBossLaunched?.Invoke();
        }

        private async UniTask SpawnWaveAsync(List<EnemyData> waveList) => 
            await _enemySquad.SpawnWaveAsync(waveList, _enemyLevel, _enemyModifiers);

        private async UniTask SpawnBossAsync()
        {
            await _enemySquad.SpawnBossAsync(_levelConfig.BossUnitId, _configuration.BossHealthBarSize, _enemyLevel,
                _levelConfig.BossManualStats, _bossModifiers);
            _isBossFight = true;
        }

        private void UpdateLevelConfig()
        {
            MainModeSheet.Elem levelConfig = null;
            
            foreach (var elem in _stageConfig)
            {
                if(elem.Level != _data.Level) continue;
                levelConfig = elem;
                break;
            }

            if (levelConfig == null) 
                Debug.LogError($"Level {_data.Level} not found by Id {_data.Id}");

            _levelConfig = levelConfig;
        }

        private void UpdateModifiers()
        {
            _enemyModifiers.Clear();
            _bossModifiers.Clear();
        }

        public int GetLevelsTo(int id, int level)
        {
            var sum = 0;
            foreach (var row in Sheet)
            {
                if(row.Index < id)
                    sum += row.Count;
                else
                {
                    sum += level;
                    break;
                }
            }

            return sum;
        }

        public int CalculateEnemyLevel()
        {
            int level = 0;
            foreach (var row in Sheet)
            {
                if (row.Index < _stageConfig.Index) 
                    level += row.Count;
                else
                {
                    level += _levelConfig.Level;
                    break;
                }
            }

            return level;
        }

        public event Action OnNeedsChanged;

        public void FillNeeds(in Dictionary<AssetType, HashSet<string>> needs) =>
            FillNeededContent(needs[AssetType.Unit], needs[AssetType.Location], 2);

        private void FillNeededContent(HashSet<string> units, HashSet<string> locations, int offset)
        {
            var unitSheet = _balance.Get<UnitsSheet>();
            var dataId = _data.Id;
            var dataLevel = _data.Level;
            var stage = _sheet[dataId];
            var level = stage.FirstOrDefault(item => item.Level == dataLevel);
            if(_levelConfig != null)
                HandleLevelConfig(_levelConfig);
            do
            {
                HandleLevelConfig(level);
                offset--;
                if (offset <= 0) continue;
                if (dataLevel == stage.GetMaxLevel())
                {
                    if (Sheet.IndexOf(stage) >= Sheet.Count - 1) continue;
                    dataId++;
                    dataLevel = 1;
                }
                else
                    dataLevel++;
                stage = _sheet[dataId];
                level = stage.FirstOrDefault(item => item.Level == dataLevel);
            } while (offset > 0);

            void HandleLevelConfig(MainModeSheet.Elem levelConfig)
            {
                foreach (var waves in levelConfig!.Waves)
                foreach (var wave in waves)
                    units.Add(unitSheet[wave.UnitId].GetPrefabId(0));
                if(!string.IsNullOrEmpty(levelConfig.BossUnitId))
                    units.Add(unitSheet[levelConfig.BossUnitId].GetPrefabId(0));
                locations.Add(levelConfig.LocationId);
            }
        }
    }
}
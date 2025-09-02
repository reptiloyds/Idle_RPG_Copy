using System;
using PleasantlyGames.RPG.Runtime.Core.PopupNumbers.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.DamageDealer.Decorator;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.States;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat
{
    [DisallowMultipleComponent]
    public class Combat : UnitComponent
    {
        [SerializeField, ReadOnly] private UnitData.Combat _data;
        
        [SerializeField, TabGroup("Debug")] public bool _logAnimationDelay;
        [SerializeField, TabGroup("Debug")] public bool _logAttackInterval;
        
        [Inject] private PopupTextFactory _popupTextFactory;
        [Inject] private UnitStatService _statService;

        private bool _isActive;
        private bool _onCooldown;
        private float _cooldownEnd;

        private IDamageProvider _damageProvider;
        private float _startAnimationTime;
        private readonly CompositeDisposable _disposable = new();

        public event Action OnBeforeAttack;
        public event Action OnAttackTrigger;

        public void SetupData(UnitData.Combat data) => 
            _data = data;

        public override void Initialize()
        {
            base.Initialize();
            
            Unit.Equipment.Weapon.OnAttackPerformed += TakeDamage;

            switch (_data.ProviderType)
            {
                default:
                case DamageProviderType.Self:
                    _damageProvider = new SelfDamageProvider(Unit);
                    break;
                case DamageProviderType.Player:
                    _damageProvider = new PlayerDamageProvider(_statService);
                    break;
            }

            if ((_data.Decorator & DamageDecoratorType.NormalEnemyDamage) != 0) 
                _damageProvider = new NormalEnemyDamageDecorator(Unit, _damageProvider);
            if ((_data.Decorator & DamageDecoratorType.AdvancedDamage) != 0) 
                _damageProvider = new AdvancedDamageDecorator(Unit, _damageProvider);
            if((_data.Decorator & DamageDecoratorType.CriticalDamage) != 0)
                _damageProvider = new CriticalDamageDecorator(Unit, _damageProvider);
            if ((_data.Decorator & DamageDecoratorType.PercentDamage) != 0)
                _damageProvider = new PercentDamageDecorator(Unit, _damageProvider);
            
            Unit.Targets.Enemy
                .Skip(1)
                .Subscribe(value =>
                {
                    if (value != null) OnEnemySet();
                    else OnEnemyNull();
                })
                .AddTo(_disposable);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
            Unit.Equipment.Weapon.OnAttackPerformed -= TakeDamage;
        }

        private void OnEnemySet()
        {
            _isActive = true;
            if(!_onCooldown)
                StartAttack();
        }

        private void OnEnemyNull()
        {
            _isActive = false;
            Unit.Equipment.Weapon.CancelAttack();
            Unit.StateMachine.SetSubState(SubStateType.AttackIdle);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            _damageProvider.Enable();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _damageProvider.Disable();
        }

        private void StartAttack()
        {
            if (!_isActive) 
                return;
            
            OnBeforeAttack?.Invoke();

            if (_logAttackInterval) 
                Logger.Log($"Attack interval: {Time.time - _startAnimationTime}");
            
            _startAnimationTime = Time.time;
            Unit.StateMachine.SetSubState(SubStateType.Attack);
        }

        public virtual void TriggerAttack()
        {
            if (!_isActive) 
                return;
            
            Unit.Equipment.Weapon.PerformAttack(Unit.Targets.Enemy.Value);
            
            if (_logAnimationDelay) 
                Logger.Log($"Attack delay: {Time.time - _startAnimationTime}");
            
            _onCooldown = true;
            _cooldownEnd = Time.time + Unit.AttackSpeed.Delay.Value;
            
            OnAttackTrigger?.Invoke();
        }

        private void TakeDamage(UnitView unitView)
        {
            if(!unitView.IsActive) return;
            if(unitView == null || !unitView.IsActive) return;
            
            var damage = _damageProvider.GetDamage(unitView);
            unitView.TakeDamage(damage);
            _popupTextFactory.SpawnFromWorldPosition(unitView.HealthBarPoint.position,
                $"-{StringExtension.Instance.CutBigDouble(damage)}",
                _damageProvider.GetColor().color);
        }

        private void FixedUpdate()
        {
            if(!_onCooldown) return;
            if(Time.time < _cooldownEnd) return;
            _onCooldown = false;
            StartAttack();
        }
    }
    
    public enum DamageProviderType
    {
        Self = 0,
        Player = 1,
    }
        
    [Flags]
    public enum DamageDecoratorType
    {
        None = 0,
        NormalEnemyDamage = 1 << 0,
        AdvancedDamage = 1 << 1,
        CriticalDamage = 1 << 2,
        PercentDamage = 1 << 3,
            
        DefaultAlly = NormalEnemyDamage | AdvancedDamage | CriticalDamage,
        DefaultEnemy = None,
        DefaultCompanion = PercentDamage,
    }
}
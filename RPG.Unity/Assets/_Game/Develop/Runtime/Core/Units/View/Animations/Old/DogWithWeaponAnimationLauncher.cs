using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations.Old
{
    public class DogWithWeaponAnimationLauncher : AnimationLauncher
    {
        [SerializeField, FoldoutGroup("Attack"), Required] private WeaponAnimator _weaponAnimator;
        [SerializeField, Required] private UnityEngine.Animator _animator;
        [SerializeField] private Vector2 _firstMoveDelay = new(0, 0);
        
        private readonly int _attackSpeed = UnityEngine.Animator.StringToHash("AttackSpeed");
        private readonly int _movementSpeed = UnityEngine.Animator.StringToHash("MovementSpeed");
        private readonly int _isMoving = UnityEngine.Animator.StringToHash("IsMoving");
        private readonly int _attack = UnityEngine.Animator.StringToHash("Attack");
        private readonly int _stopAttack = UnityEngine.Animator.StringToHash("StopAttack");
        private readonly int _hit = UnityEngine.Animator.StringToHash("Hit");
        
        private bool _isFirstMove;
        private Tween _firstMoveTween;
        
        protected override void GetComponents()
        {
            base.GetComponents();
            _animator ??= GetComponent<UnityEngine.Animator>();
        }

        protected override void Awake()
        {
            base.Awake();
            _weaponAnimator.OnShoot += InvokeAttackTrigger;
        }

        private void OnDestroy() => 
            _weaponAnimator.OnShoot -= InvokeAttackTrigger;

        public override void OnSpawn()
        {
            base.OnSpawn();
            _isFirstMove = true;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            _firstMoveTween.Stop();
        }

        public override void MultiplyAttackSpeed(float k) => 
            _animator.SetFloat(_attackSpeed, k);

        protected override void Idle() => 
            _animator.SetBool(_isMoving, false);

        protected override void Move()
        {
            if (_isFirstMove && _firstMoveDelay != Vector2.zero)
            {
                _firstMoveTween.Stop();
                var delay = _firstMoveDelay.Random();
                _firstMoveTween = Tween.Delay(delay, SetMove);
                _isFirstMove = false;   
            }
            else
                SetMove();
        }

        protected override void Attack() => 
            _weaponAnimator.Play();

        private void OnAttack() => 
            InvokeAttackTrigger();

        protected override void StopAttack() => 
            _animator.SetTrigger(_stopAttack);

        private void SetMove() =>
            _animator.SetBool(_isMoving, true);
    }
}
using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations.Old
{
    public enum UnitAnimationType
    {
        None = 0,
        Idle = 1,
        Move = 2,
        Attack = 3,
        StopAttack = 4,
        Hit = 5,
    }
    
    [DisallowMultipleComponent]
    public abstract class AnimationLauncher : UnitComponent
    {
        public float DefaultAttackDelay { get; protected set; }
        public event Action AttackTrigger;
        
        [SerializeField] private ShakeSettings _shakeSettings = new()
        {
            duration = 0.25f,
            strength = new Vector3(-0.25f, -0.25f, -0.25f),
            frequency = 2,
        };
        
        private Tween _hitTween;
        private Vector3 _originScale;

        public event Action<UnitAnimationType> OnAnimationPlay;

        protected virtual void Awake()
        {
            _originScale = transform.localScale;
        }

        public void Play(UnitAnimationType type)
        {
            switch (type)
            {
                case UnitAnimationType.Idle:
                    Idle();
                    break;
                case UnitAnimationType.Move:
                    Move();
                    break;
                case UnitAnimationType.Attack:
                    Attack();
                    break;
                case UnitAnimationType.StopAttack:
                    StopAttack();
                    break;
                case UnitAnimationType.Hit:
                    Hit();
                    break;
            }
            
            OnAnimationPlay?.Invoke(type);
        }

        public abstract void MultiplyAttackSpeed(float k);

        protected void InvokeAttackTrigger() => 
            AttackTrigger?.Invoke();

        [Button]
        protected abstract void Idle();

        [Button]
        protected abstract void Move();

        [Button]
        protected abstract void Attack();

        [Button]
        protected abstract void StopAttack();

        [Button]
        protected virtual void Hit()
        {
            _hitTween.Stop();
            transform.localScale = _originScale;
            _hitTween = Tween.PunchScale(transform, _shakeSettings);
            //_animator.SetTrigger(_hit);
        }
    }
}

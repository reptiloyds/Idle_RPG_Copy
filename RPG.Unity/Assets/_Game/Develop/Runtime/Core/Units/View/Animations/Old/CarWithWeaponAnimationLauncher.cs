using PleasantlyGames.RPG.Runtime.TweenUtilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations.Old
{
    public class CarWithWeaponAnimationLauncher : AnimationLauncher
    {
        [SerializeField, FoldoutGroup("Attack"), Required] private WeaponAnimator _weaponAnimator;
        [SerializeField, FoldoutGroup("Movement")] private BaseAnimation _wheels;
        
        private float _previousSpeed;
        private float _currentSpeed;


        protected override void Awake()
        {
            base.Awake();
            
            _weaponAnimator.OnShoot += InvokeAttackTrigger;
        }

        private void OnDestroy() => 
            _weaponAnimator.OnShoot -= InvokeAttackTrigger;

        public override void MultiplyAttackSpeed(float k) => 
            _weaponAnimator.SetAttackSpeedK(k);

        protected override void Idle()
        {
            _wheels.Stop();
        }

        protected override void Move()
        {
            _wheels.Play();
        }

        protected override void Attack() => 
            _weaponAnimator.Play();

        protected override void StopAttack()
        {
        }
    }
}

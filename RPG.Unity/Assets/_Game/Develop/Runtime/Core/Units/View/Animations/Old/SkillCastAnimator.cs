using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Animations.Old
{
    public class SkillCastAnimator : WeaponAnimator
    {
        [SerializeField, Required] private ParticleSystem _particle;
        [SerializeField] private bool _stopOnComplete;
        [SerializeField, MinValue(0)] private float _defaultAttackDelay;
        
        private Sequence _attackSequence;
        
        public override void Play()
        {
            if (_attackSequence.isAlive)
            {
                _attackSequence.Complete();
                _attackSequence.Stop();
            }
            
            _attackSequence = Sequence.Create();
            _particle.Play();
            _attackSequence.Group(Tween.Delay(_defaultAttackDelay / AttackSpeedK, ExecuteOnShoot));
            if (_stopOnComplete) 
                _attackSequence.OnComplete(_particle.Stop);
        }
    }
}
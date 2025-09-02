using PrimeTween;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    public class ShakeEffect : VisualEffect
    {
        [SerializeField] private ShakeSettings _shakeSettings = new()
        {
            duration = 0.25f,
            strength = new Vector3(-0.25f, -0.25f, -0.25f),
            frequency = 2,
        };
        
        private Tween _tween;
        private readonly Vector3 _originalScale = Vector3.one;

        public override void Activate(UnitView unitView)
        {
            base.Activate(unitView);

            var target = unitView.Visual.transform;
            Cancel(target);
            _tween = Tween.PunchScale(target, _shakeSettings);
        }

        public override void Deactivate(UnitView unitView)
        {
            base.Deactivate(unitView);
            
            Cancel(unitView.Visual.transform);
        }

        private void Cancel(Transform target)
        {
            _tween.Stop();
            target.localScale = _originalScale;
        }
    }
}
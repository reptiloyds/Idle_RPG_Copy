using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Skill.View.Component.Base;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.View.Component
{
    public class SkillScaleTween : SkillViewComponent
    {
        [SerializeField] private List<Transform> _targets;
        [SerializeField] private bool _resetScaleOnSpawn = true;
        [SerializeField] private bool _animateSpawn;
        [SerializeField, FoldoutGroup("SpawnSettings"), HideIf("@this._animateSpawn == false")]
        private bool _spawnStartFromCurrent;
        [SerializeField, FoldoutGroup("SpawnSettings"), HideIf("@this._animateSpawn == false")]
        private TweenSettings<Vector3> _spawnSettings;
        [SerializeField] private bool _animateDespawn;
        [SerializeField, FoldoutGroup("DespawnSettings"), HideIf("@this._animateDespawn == false")]
        private bool _despawnStartFromCurrent;
        [SerializeField, FoldoutGroup("DespawnSettings"), HideIf("@this._animateDespawn == false")]
        private TweenSettings<Vector3> _despawnSettings;

        private readonly List<Tween> _tweens = new();
        
        public override void OnSpawn()
        {
            if (_resetScaleOnSpawn)
                foreach (var target in _targets) 
                    target.localScale = Vector3.one;
            
            if(!_animateSpawn) return;
            StopTweens();
            foreach (var target in _targets)
            {
                var tween = Tween.Scale(target, _spawnSettings.WithDirection(true, _spawnStartFromCurrent));
                _tweens.Add(tween);
            }
        }

        public override void OnDespawn()
        {
            if(!_animateDespawn) return;
            StopTweens();
            foreach (var target in _targets)
            {
                var tween = Tween.Scale(target, _despawnSettings.WithDirection(true, _despawnStartFromCurrent));
                _tweens.Add(tween);
            }
        }

        private void StopTweens()
        {
            foreach (var tween in _tweens)
                tween.Stop();
            _tweens.Clear();
        }

        public override float GetDespawnTime()
        {
            return _despawnSettings.settings.startDelay + _despawnSettings.settings.endDelay +
                   _despawnSettings.settings.duration;
        }
    }
}
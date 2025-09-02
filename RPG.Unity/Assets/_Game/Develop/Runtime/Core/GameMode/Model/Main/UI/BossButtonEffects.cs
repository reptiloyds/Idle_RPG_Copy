using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BossButtonEffects : MonoBehaviour
    {
        [SerializeField] private Image _blinkImage;
        [SerializeField, Range(0, 1)] private float _blinkValue = 0.75f;
        [SerializeField] private List<Image> _hollowCircles;
        [SerializeField] private TweenSettings<Vector3> _scaleSettings;
        [SerializeField, Min(0)] private float _fadeIn = 0.5f;
        [SerializeField, Min(0)] private float _spawnCirclesDuration = 0.5f;
        [SerializeField, Min(0)] private float _fadeOut = 0.5f;
        [SerializeField, Min(0)] private float _cooldown = 0.75f;
        
        private Sequence _sequence;
        private float _nextRunTime;
        private bool _onCooldown;

        private void OnEnable() => 
            Play();

        private void OnDisable() => 
            Stop();

        private void Play()
        {
            _onCooldown = false;
            _sequence = Sequence.Create(useUnscaledTime: true);
            _blinkImage.gameObject.SetActive(true);
            var color = _blinkImage.color;
            color.a = 0;
            _blinkImage.color = color;
            _sequence.Chain(Tween.Alpha(_blinkImage, _blinkValue, _fadeIn));
            _sequence.Group(SpawnCircles());
            _sequence.Group(Tween.Alpha(_blinkImage, 0, _fadeOut, startDelay: _fadeIn * 1.1f));
            _sequence.OnComplete(this, target => target.OnComplete());
        }
        
        private void OnComplete()
        {
            _onCooldown = true;
            _nextRunTime = Time.unscaledTime + _cooldown;
            DisableEffectGameObjects();
        }

        private void Update()
        {
            if(!_onCooldown) return;
            if (Time.unscaledTime >= _nextRunTime) 
                Play(); 
        }

        private Sequence SpawnCircles()
        {
            var spawnDelay = _spawnCirclesDuration / _hollowCircles.Count;
            var spawnCircles = Sequence.Create(useUnscaledTime: true);

            spawnCircles.ChainDelay(_fadeIn * 0.75f);
            for (var i = 0; i < _hollowCircles.Count; i++)
            {
                var circle = _hollowCircles[i];
                circle.gameObject.SetActive(true);
                circle.transform.localScale = Vector3.zero;
                var color = circle.color;
                color.a = 1;
                circle.color = color;
                
                var startDelay = i * spawnDelay;
                var scaleDuration = 0.5f;
                spawnCircles.Group(Tween.Scale(circle.transform, Vector3.one, scaleDuration, Ease.Default, 1, CycleMode.Restart, startDelay));
                spawnCircles.Group(Tween.Alpha(circle, 0, scaleDuration * 0.35f, Ease.Default, 1, CycleMode.Restart, startDelay + scaleDuration * 0.65f));
            }

            return spawnCircles;
        }

        private void Stop()
        {
            _onCooldown = false;
            _sequence.Stop();

            DisableEffectGameObjects();
        }

        private void DisableEffectGameObjects()
        {
            foreach (var circle in _hollowCircles) 
                circle.gameObject.SetActive(false);
            
            _blinkImage.gameObject.SetActive(false);
        }
    }
}

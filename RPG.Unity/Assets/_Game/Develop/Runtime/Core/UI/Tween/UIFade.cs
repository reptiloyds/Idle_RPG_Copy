using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Tween
{
    public class UIFade : MonoBehaviour
    {
        [SerializeField] private List<Graphic> _graphics;
        [SerializeField] private TweenSettings<float> _settings;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _stopOnDisable = true;
        
        private readonly List<PrimeTween.Tween> _tweens = new(8);
        
        private void OnEnable()
        {
            if (_playOnEnable)
                Play();
        }

        private void OnDisable()
        {
            if(_stopOnDisable)
                Stop();
        }

        [Button]
        public void Play()
        {
            Stop();
            foreach (var graphic in _graphics) 
                _tweens.Add(PrimeTween.Tween.Alpha(graphic, _settings));
        }

        [Button]
        public void Stop()
        {
            foreach (var tween in _tweens) 
                tween.Stop();
            _tweens.Clear();
        }
    }
}

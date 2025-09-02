using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Animation
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIRotationAnimation : MonoBehaviour
    {
        [SerializeField] private TweenSettings<Vector3> _tweenSettings = new TweenSettings<Vector3>()
        {
            endValue = new Vector3(0, 0, 180),
            settings = new TweenSettings()
            {
                duration = 1f,
                cycles = -1,
                cycleMode = CycleMode.Incremental,
                useUnscaledTime = true
            }
        };
        
        private PrimeTween.Tween _tween;
        
        private void OnEnable() => Play();

        private void OnDisable() => Stop();
        
        [Button]
        private void Play()
        {
            Stop();
            _tween = PrimeTween.Tween.LocalRotation(transform, _tweenSettings.WithDirection(true, false));
        }

        [Button]
        private void Stop() =>
            _tween.Stop();
    }
}
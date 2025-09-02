using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BlessingLevelUpView : MonoBehaviour
    {
        [SerializeField] private float _fadeInDuration;
        [SerializeField] private float _stableDuration;
        [SerializeField] private float _fadeOutDuration;
        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private TextMeshProUGUI _blessingName;
        [SerializeField] private TextMeshProUGUI _blessingLevel;

        private Sequence _sequence;

        private void Awake() => 
            _canvas.alpha = 0;

        public void Show(string blessingName, int blessingLevel)
        {
            _sequence.Stop();
            _canvas.alpha = 0;
            
            _blessingName.SetText(blessingName);
            _blessingLevel.SetText(blessingLevel.ToString());
            
            _sequence = Sequence.Create();
            _sequence.Chain(Tween.Alpha(_canvas, 1, _fadeInDuration));
            _sequence.ChainDelay(_stableDuration);
            _sequence.Chain(Tween.Alpha(_canvas, 0, _fadeOutDuration));
        }
    }
}

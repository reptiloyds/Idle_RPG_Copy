using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class MainModeLevelPresenter : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI _levelStatic;
        [SerializeField, Required] private TextMeshProUGUI _levelDynamic;

        [SerializeField, MinValue(0)] private float _emergeTime = 0.5f;
        [SerializeField] private Ease _emergeEase = Ease.Linear;
        [SerializeField, MinValue(0)] private float _disappearTime = 0.5f;
        [SerializeField] private Ease _disappearEase = Ease.Linear;
        [SerializeField, MinValue(0)] private float _middlePointTime = 0.5f;
        [SerializeField] private Ease _middlePointEase = Ease.Linear;
        [SerializeField, Required] private Transform _moveObject;
        [SerializeField, Required] private Transform _startPoint;
        [SerializeField, Required] private Transform _firstMiddlePoint;
        [SerializeField, Required] private Transform _secondMiddlePoint;
        [SerializeField, Required] private Transform _endPoint;

        private Sequence _sequence;
        
        public void Present(string level)
        {
            _levelStatic.SetText(level);
            _levelDynamic.SetText(level);
            Tween.Delay(0.25f, StartAnimation);
        }
        
        [Button]
        private void StartAnimation()
        {
            EnableMoveObject();
            _moveObject.transform.position = _startPoint.position;
            _sequence.Stop();
            _sequence = Sequence.Create();
            //_sequence.ChainCallback(() => _moveObject.transform.position = _startPoint.position);
            _sequence.Chain(Tween.Position(_moveObject, _firstMiddlePoint.position, _emergeTime, _emergeEase));
            _sequence.Chain(Tween.Position(_moveObject, _secondMiddlePoint.position, _middlePointTime, _middlePointEase));
            _sequence.Chain(Tween.Position(_moveObject, _endPoint.position, _disappearTime, _disappearEase));
            _sequence.OnComplete(DisableMoveObject);
        }

        private void EnableMoveObject()
        {
            _moveObject.gameObject.SetActive(true);
        }

        private void DisableMoveObject()
        {
            _moveObject.gameObject.SetActive(false);
        }
    }
}
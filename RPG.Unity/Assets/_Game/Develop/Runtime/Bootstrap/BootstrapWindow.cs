using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Bootstrap
{
    [DisallowMultipleComponent]
    public class BootstrapWindow : MonoBehaviour, IBootstrapStarter
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private bool _launchImmediately;

        private UniTaskCompletionSource _completionSource;
        private bool _isReady;

        private void Awake()
        {
            _startButton.onClick.AddListener(OnStartClicked);
            gameObject.SetActive(!_launchImmediately);

            if (_launchImmediately)
                Ready();
        }

        private void OnDestroy() =>
            _startButton.onClick.RemoveAllListeners();

        private void OnStartClicked()
        {
            if (_launchImmediately) return;
            _startButton.gameObject.SetActive(false);
            Ready();
        }

        public async UniTask WaitForReady()
        {
            _completionSource = new UniTaskCompletionSource();
            if (_isReady) _completionSource.TrySetResult();
            await _completionSource.Task;
        }

        private void Ready()
        {
            _isReady = true;
            if (_completionSource != null)
                _completionSource.TrySetResult();
        }
    }
}
using PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Model;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.View;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.View
{
    public class InternetConnectionLostView : TechnicalMessage
    {
        [SerializeField] private Button _retryButton;

        [Inject] private InternetConnectionService _connectionService;

        private void Awake() => 
            _retryButton.onClick.AddListener(OnRetryClick);

        private void OnDestroy() => 
            _retryButton.onClick.RemoveListener(OnRetryClick);

        private void OnRetryClick() => 
            _connectionService.RestoreConnection();
    }
}
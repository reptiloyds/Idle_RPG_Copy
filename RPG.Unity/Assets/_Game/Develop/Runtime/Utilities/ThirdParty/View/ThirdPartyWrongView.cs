using PleasantlyGames.RPG.Runtime.Utilities.ApplicationCloser.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.View;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.View
{
    public class ThirdPartyWrongView : TechnicalMessage
    {
        [SerializeField] private Button _restartButton;

        [Inject] private IApplicationCloser _applicationCloser;
        
        private void Awake()
        {
            if(_restartButton != null)
                _restartButton.onClick.AddListener(OnRestartClick);
        }

        private void OnDestroy()
        {
            if(_restartButton != null)
                _restartButton.onClick.RemoveListener(OnRestartClick);
        }

        private void OnRestartClick() =>
            _applicationCloser.Close();
    }
}
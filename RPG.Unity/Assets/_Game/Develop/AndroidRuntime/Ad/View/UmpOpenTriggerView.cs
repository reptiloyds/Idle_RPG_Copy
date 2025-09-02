using GoogleMobileAds.Ump.Api;
using PleasantlyGames.RPG.AndroidRuntime.Ad.Model;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using PleasantlyGames.RPG.Runtime.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.AndroidRuntime.Ad.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UmpOpenTriggerView : MonoBehaviour
    {
        [SerializeField] private BaseButton _button;
        [SerializeField] private CanvasGroup _group;

        [Inject] private UmpService _umpService;
        [Inject] private IPauseService _pauseService;

        private void Awake()
        {
            UpdateInteraction();
            _button.OnClick += OnClick;
        }

        private void OnDestroy() => 
            _button.OnClick -= OnClick;

        private async void OnClick()
        {
            _group.interactable = false;
            _button.SetInteractable(false);
            _pauseService.Pause(PauseType.Time);
            await _umpService.TryUpdateConsent(true);
            Application.Quit();
        }

        private void UpdateInteraction() => 
            _button.SetInteractable(ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required);
    }
}
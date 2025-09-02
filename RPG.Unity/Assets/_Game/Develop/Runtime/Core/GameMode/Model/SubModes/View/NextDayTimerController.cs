using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.View
{
    [DisallowMultipleComponent, HideMonoScript]
    [RequireComponent(typeof(TextTimer))]
    public class NextDayTimerController : MonoBehaviour
    {
        [SerializeField, Required] private TextTimer _timer;

        [Inject] private TimeService _timeService;

        private void Reset() => 
            _timer = GetComponent<TextTimer>();

        private void OnEnable() => 
            RestartTimer();

        private void RestartTimer()
        {
            if(_timeService == null) return;
            _timer.Listen(_timeService.TimeToEndDay, false);
        }
    }
}
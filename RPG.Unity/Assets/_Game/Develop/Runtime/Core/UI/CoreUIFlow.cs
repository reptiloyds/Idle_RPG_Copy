using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Resource.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.UI
{
    public class CoreUIFlow : IStartable
    {
        [Inject] private NotificationService _notificationService;
        private readonly UniTaskCompletionSource _completionSource;

        [Preserve]
        public CoreUIFlow(UniTaskCompletionSource completionSource)
        {
            _completionSource = completionSource;
        }

        public void Start()
        {
            InitializeAsync().Forget();   
        }

        private async UniTask InitializeAsync()
        {
            await _notificationService.WarmUpAsync();
            _notificationService.Initialize();
            _completionSource.TrySetResult();   
        }
    }
}
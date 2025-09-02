using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Story
{
    public class StoryService
    {
        [Inject] private StoryDataProvider _dataProvider;
        [Inject] private IPauseService _pauseService;
        [Inject] private IWindowService _windowService;
        private Func<UniTask> _completeCallbackAsync;
        
        [Preserve]
        public StoryService()
        {
        }

        public async UniTask InitializeAsync(Func<UniTask> completeCallback)
        {
            _completeCallbackAsync = completeCallback;
            var data = _dataProvider.GetData();
            if (!data.Completed)
            {
                data.Completed = true;
                await _windowService.GetAsync<StoryWindow>(false);
                await RunStory();
            }
            else
                await _completeCallbackAsync.Invoke();
        }

        private async UniTask RunStory()
        { 
            _pauseService.Pause(PauseType.Time);
            _pauseService.Pause(PauseType.Save);
            var window = await _windowService.OpenAsync<StoryWindow>();
            window.Play(OnStoryEnd);
        }

        private async void OnStoryEnd()
        {
            _pauseService.Continue(PauseType.Time);
            _pauseService.Continue(PauseType.Save);
            if(_completeCallbackAsync != null)
                await _completeCallbackAsync.Invoke();
        }
    }
}

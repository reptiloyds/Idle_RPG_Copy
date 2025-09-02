using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Content
{
    public class ContentUnlockTutorialTrigger : TutorialTrigger
    {
        private readonly ContentService _contentService;
        private readonly ContentTutorialData _data;

        public ContentUnlockTutorialTrigger(ContentService contentService, string dataJSON)
        {
            _contentService = contentService;
            _data = JsonConvert.DeserializeObject<ContentTutorialData>(dataJSON);
        }
        
        public override void Initialize()
        {
            _contentService.OnContentUnlocked += OnContentUnlocked;
            if(_contentService.IsUnlocked(_data.Id))
                Execute();
        }

        public override void Dispose()
        {
            _contentService.OnContentUnlocked -= OnContentUnlocked;
        }

        private void OnContentUnlocked(ContentControl.Model.Content content)
        {
            if(!string.Equals(content.Id, _data.Id)) return;
            Execute();
        }
    }
}
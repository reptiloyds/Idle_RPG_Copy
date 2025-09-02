using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Content
{
    public class ContentManualTutorialTrigger : TutorialTrigger
    {
        private readonly ContentService _contentService;
        private readonly ContentTutorialData _data;
        private ContentControl.Model.Content _content; 

        public ContentManualTutorialTrigger(ContentService contentService, string dataJSON)
        {
            _contentService = contentService;
            _data = JsonConvert.DeserializeObject<ContentTutorialData>(dataJSON);
        }
        
        public override void Initialize()
        {
            _content = _contentService.GetById(_data.Id);
            if (_content.IsUnlocked || _content.IsReadyForManualUnlock) 
                Execute();
            else
                _content.OnReadyToManualUnlock += OnContentUnlocked;
        }

        public override void Dispose()
        {
            if(_content != null)
                _content.OnReadyToManualUnlock -= OnContentUnlocked;
        }

        private void OnContentUnlocked()
        {
            _content.OnReadyToManualUnlock -= OnContentUnlocked;
            _content = null;
            Execute();
        }
    }
}
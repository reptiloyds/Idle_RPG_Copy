using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Accent;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Accent.Model
{
    public class AccentService
    {
        private readonly List<AccentTutorialData> _requests = new();

        public IReadOnlyList<AccentTutorialData> Requests => _requests;

        public event Action<AccentTutorialData> OnRequestAdded;
        public event Action<AccentTutorialData> OnRequestRemoved;
        
        public void Add(AccentTutorialData data)
        {
            _requests.Add(data);
            OnRequestAdded?.Invoke(data);
        }

        public void Remove(AccentTutorialData data)
        {
            foreach (var request in _requests)
            {
                if (request != data) continue;
                _requests.Remove(request);
                OnRequestRemoved?.Invoke(request);
                break;
            }
        }
    }
}
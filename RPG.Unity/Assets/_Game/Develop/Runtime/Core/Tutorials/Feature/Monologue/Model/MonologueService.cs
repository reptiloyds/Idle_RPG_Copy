using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Monologue;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Monologue.Model
{
    public class MonologueService
    {
        private readonly List<MonologueTutorialData> _requests = new();

        public IReadOnlyList<MonologueTutorialData> Requests => _requests;

        public event Action<MonologueTutorialData> OnRequestAdded;
        public event Action<MonologueTutorialData> OnRequestRemoved;
        
        public void Add(MonologueTutorialData data)
        {
            _requests.Add(data);
            OnRequestAdded?.Invoke(data);
        }

        public void Remove(MonologueTutorialData data)
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
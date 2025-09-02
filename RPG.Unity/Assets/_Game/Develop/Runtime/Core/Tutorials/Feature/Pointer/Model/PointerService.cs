using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Pointer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.Model
{
    public class PointerService
    {
        private readonly List<PointerTutorialData> _requests = new();

        public IReadOnlyList<PointerTutorialData> Requests => _requests;

        public event Action<PointerTutorialData> OnRequestAdded;
        public event Action<PointerTutorialData> OnRequestRemoved;
        
        public void Add(PointerTutorialData data)
        {
            _requests.Add(data);
            OnRequestAdded?.Invoke(data);
        }

        public void Remove(PointerTutorialData data)
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
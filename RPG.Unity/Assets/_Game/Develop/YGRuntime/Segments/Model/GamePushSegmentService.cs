using System;
using System.Collections.Generic;
using GamePush;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace PleasantlyGames.RPG.YGRuntime.Segments.Model
{
    public class GamePushSegmentService : IInitializable, IDisposable
    {
        public event Action<string> OnSegmentEntered;
        public event Action<string> OnSegmentLeaved;
        
        private HashSet<string> _segments = new();
        
        [Preserve]
        public GamePushSegmentService()
        {
        }
        
        void IInitializable.Initialize()
        {
            GP_Segments.OnSegmentEnter += OnSegmentEnter;
            GP_Segments.OnSegmentLeave += OnSegmentLeave;

            var listJson = GP_Segments.List();
            if (string.IsNullOrEmpty(listJson)) 
                _segments = new HashSet<string>();
            else
                _segments = JsonConvert.DeserializeObject<HashSet<string>>(listJson);
        }

        void IDisposable.Dispose()
        {
            GP_Segments.OnSegmentEnter -= OnSegmentEnter;
            GP_Segments.OnSegmentLeave -= OnSegmentLeave;
        }

        public bool HasSegment(string segment) => 
            GP_Segments.Has(segment) || _segments.Contains(segment);

        private void OnSegmentEnter(string segment)
        {
            _segments.Add(segment);
            OnSegmentEntered?.Invoke(segment);
        }

        private void OnSegmentLeave(string segment)
        {
            _segments.Remove(segment);
            OnSegmentLeaved?.Invoke(segment);
        }
    }
}
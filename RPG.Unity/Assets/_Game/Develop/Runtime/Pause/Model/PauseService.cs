using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Pause.Model
{
    public class PauseService : IPauseService, IInitializable, IDisposable
    {
        private readonly List<PauseModel> _components = new ();

        public event Action<PauseType> OnPause;
        public event Action<PauseType> OnContinue;

        [Preserve]
        public PauseService()
        {
        }
        
        public virtual void Initialize() => 
            CreateComponents();

        public virtual void Dispose()
        {
            foreach (var component in _components)
            {
                component.OnPause -= OnPaused;
                component.OnContinue -= OnContinued;
            }
        }

        private void CreateComponents()
        {
            AddPause(new PauseModel(PauseType.Input));
            AddPause(new PauseModel(PauseType.CursorLock));
            AddPause(new PauseModel(PauseType.CursorVision));
            AddPause(new PauseModel(PauseType.Audio));
            AddPause(new PauseModel(PauseType.Time));
            AddPause(new PauseModel(PauseType.Ad));
            AddPause(new PauseModel(PauseType.UIInput));
            AddPause(new PauseModel(PauseType.Save));
        }

        private void AddPause(PauseModel pauseModel)
        {
            _components.Add(pauseModel);
            pauseModel.OnPause += OnPaused;
            pauseModel.OnContinue += OnContinued;
        }

        private void OnPaused(PauseModel model) => 
            OnPause?.Invoke(model.Type);

        private void OnContinued(PauseModel model) => 
            OnContinue?.Invoke(model.Type);

        public bool IsPauseEnabled(PauseType pauseType)
        {
            foreach (var component in _components)
            {
                if (component.IsPaused && pauseType.HasFlag(component.Type))
                    return true;
            }

            return false;
        }

        public void Pause(PauseType pauseType)
        {
            foreach (var component in _components)
            {
                if (!pauseType.HasFlag(component.Type)) continue;
                component.Pause();
            }
        }

        public void Continue(PauseType pauseType)
        {
            foreach (var component in _components)
            {
                if (!pauseType.HasFlag(component.Type)) continue;
                component.Continue();
            }
        }
    }
}
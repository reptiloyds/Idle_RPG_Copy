using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class DungeonHubPage<T> : MonoBehaviour, IInitializable where T : DungeonMode
    {
        [Inject] protected T Dungeon;

        public virtual void Initialize()
        {
            Dungeon.OnLaunched += OnLaunched;
            Dungeon.OnDisposed += OnDisposed;
            if(Dungeon.IsLaunched)
                Enable();
            else
                Disable();
        }

        protected virtual void OnDestroy()
        {
            if (Dungeon == null) return;
            Dungeon.OnLaunched -= OnLaunched;
            Dungeon.OnDisposed -= OnDisposed;
        }

        private void OnLaunched(IGameMode mode) =>
            Enable();
        private void OnDisposed(IGameMode mode) =>
            Disable();

        protected virtual void Enable() => 
            gameObject.SetActive(true);

        protected virtual void Disable() => 
            gameObject.SetActive(false);
    }
}
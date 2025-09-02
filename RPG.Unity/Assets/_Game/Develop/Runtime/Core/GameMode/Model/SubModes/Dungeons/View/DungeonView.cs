using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class DungeonView<T> : MonoBehaviour, IInitializable where T : DungeonMode
    {
        [SerializeField] private TextMeshProUGUI _labelText;

        protected TextMeshProUGUI LableText => _labelText;
        
        [Inject] protected T Dungeon;
        
        public virtual void Initialize()
        {
            Dungeon.OnLaunched += OnLaunched;
            Dungeon.OnDisposed += OnDisposed;
            Dungeon.OnWin += OnWin;
            Dungeon.OnLose += OnLose;
            
            if (Dungeon.IsLaunched)
                Enable();
            else
                Disable();
        }

        protected virtual void OnDestroy()
        {
            Dungeon.OnLaunched -= OnLaunched;
            Dungeon.OnDisposed -= OnDisposed;
            Dungeon.OnWin -= OnWin;
            Dungeon.OnLose -= OnLose;
        }

        protected virtual void OnLaunched(IGameMode mode) => 
            Enable();

        protected virtual void OnDisposed(IGameMode mode) => 
            Disable();

        protected virtual void OnWin(IGameMode mode)
        {
            
        }

        protected virtual void OnLose(IGameMode mode)
        {
            
        }

        protected virtual void Enable()
        {
            LableText.SetText(Dungeon.GetFormattedFullName());
            gameObject.SetActive(true);
        }

        protected virtual void Disable() => 
            gameObject.SetActive(false);
    }
}
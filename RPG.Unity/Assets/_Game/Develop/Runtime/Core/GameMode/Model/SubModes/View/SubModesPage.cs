using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SubModesPage : MonoBehaviour, IInitializable
    {
        [SerializeField] private SubModeCardView _cardPrefab;
        [SerializeField] private RectTransform _container;

        private readonly Dictionary<DungeonMode, SubModeCardView> _dungeonViews = new();
        private readonly Dictionary<RouletteMode, SubModeCardView> _rouletteViews = new();

        [Inject] private DungeonModeFacade _modeFacade;
        [Inject] private RouletteFacade _rouletteFacade;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private TimeService _timeService;

        void IInitializable.Initialize()
        {
            foreach (var dungeon in _modeFacade.Dungeons)
            {
                var view = CreateView();
                view.Setup(dungeon);
                view.EnterButton.ChangeButtonId($"enterLauncher_{dungeon.Type.ToString()}");
                view.OnEnterClick += OnEnterDungeon;
                _dungeonViews.Add(dungeon, view);
            }

            foreach (var roulette in _rouletteFacade.RouletteModes)
            {
                var view = CreateView();
                view.Setup(roulette);
                view.EnterButton.ChangeButtonId($"enterLauncher_{roulette.Type.ToString()}");
                view.OnEnterClick += OnRouletteEnter;
                _rouletteViews.Add(roulette, view);
            }
            
            _modeFacade.OnLaunched += OnDungeonLaunched;
            _modeFacade.OnDisposed += OnDungeonDisposed;
        }

        private void OnDestroy()
        {
            foreach (var kvp in _dungeonViews) 
                kvp.Value.OnEnterClick -= OnEnterDungeon;
            _dungeonViews.Clear();
            
            _modeFacade.OnLaunched -= OnDungeonLaunched;
            _modeFacade.OnDisposed -= OnDungeonDisposed;
        }

        private void OnDungeonLaunched(DungeonMode dungeonMode) => 
            Disable();

        private void OnDungeonDisposed(DungeonMode dungeonMode) => 
            Enable();

        private void Enable() => 
            gameObject.SetActive(true);

        private void Disable() => 
            gameObject.SetActive(false);

        private SubModeCardView CreateView() => 
            _objectResolver.Instantiate(_cardPrefab, _container);

        private async void OnEnterDungeon(SubModeCardView card)
        {
            foreach (var kvp in _dungeonViews)
            {
                if(kvp.Value != card) continue;
                await kvp.Key.Select();
                break;
            }
        }

        private async void OnRouletteEnter(SubModeCardView card)
        {
            foreach (var kvp in _rouletteViews)
            {
                if(kvp.Value != card) continue;
                await kvp.Key.Select();
                break;
            }
        }

        public SubModeCardView GetViewByModel(DungeonMode mode) => 
            _dungeonViews.GetValueOrDefault(mode);

        public SubModeCardView GetViewByModel(RouletteMode mode) => 
            _rouletteViews.GetValueOrDefault(mode);
    }
}
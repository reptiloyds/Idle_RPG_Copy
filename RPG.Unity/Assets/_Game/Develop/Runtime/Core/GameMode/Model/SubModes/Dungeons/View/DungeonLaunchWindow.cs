using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.View;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View
{
    public abstract class DungeonLaunchWindow<T> : BaseWindow where T : DungeonMode
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private SubModeEnterResourceView _enterResourceView;
        [SerializeField] private SubModeEnterButtons _enterButtons;
        
        [Inject] protected DungeonModeFacade DungeonModeFacade;
        [Inject] protected IWindowService WindowService;
        [Inject] protected T Dungeon;

        protected override void Awake()
        {
            base.Awake();
            
            _enterButtons.OnEnter += OnEnter;
            _enterButtons.Setup(Dungeon, false);
            _enterButtons.OnBonusEnter += OnBonusEnter;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _enterButtons.OnEnter -= OnEnter;
            _enterButtons.OnBonusEnter -= OnBonusEnter;
        }

        public override void Open()
        {
            base.Open();
            
            Dungeon.OnBonusEnterSpent += RedrawDynamic;
            Dungeon.EnterResource.OnChange += RedrawDynamic;
            RedrawDynamic();
        }

        public override void Close()
        {
            if (Dungeon != null)
            {
                Dungeon.OnBonusEnterSpent -= RedrawDynamic;
                if(Dungeon.EnterResource != null)
                    Dungeon.EnterResource.OnChange -= RedrawDynamic;   
            }
            base.Close();
        }

        protected virtual void OnEnter()
        {
            DungeonModeFacade.Launch(Dungeon);
            Close();
        }

        protected virtual void OnBonusEnter()
        {
            DungeonModeFacade.BonusLaunch(Dungeon);
            Close();
        }

        protected virtual void RedrawDynamic()
        {
            _enterButtons.Redraw();
            _enterResourceView.Redraw(Dungeon);
        }
    }
}
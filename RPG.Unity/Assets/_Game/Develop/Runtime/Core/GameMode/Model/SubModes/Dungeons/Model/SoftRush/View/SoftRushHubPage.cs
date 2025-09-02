using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SoftRushHubPage : DungeonHubPage<SoftRushMode>
    {
        [SerializeField] private DungeonAutoSweepView _autoSweepView;
        [SerializeField] private BaseButton _leaveButton;

        public override void Initialize()
        {
            base.Initialize();
            _leaveButton.OnClick += OnLeaveClick;
            _autoSweepView.OnToggled += OnAutoSweepToggled;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _leaveButton.OnClick -= OnLeaveClick;
            _autoSweepView.OnToggled -= OnAutoSweepToggled;
        }

        private void OnAutoSweepToggled() => 
            Dungeon.SetAutoSweep(_autoSweepView.IsOn);

        protected override void Enable()
        {
            _autoSweepView.SetValue(Dungeon.IsAutoSweep);
            base.Enable();
        }

        private void OnLeaveClick() => 
            Dungeon.Leave();
    }
}
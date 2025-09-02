using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.View
{
    public class BossRushView : DungeonView<BossRushMode>
    {
        [SerializeField] private TextMeshProUGUI _bossCounter;
        [SerializeField] private TextTimer _timer;
        
        public override void Initialize()
        {
            base.Initialize();
            Dungeon.OnBossDefeated += RedrawBossCounter;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Dungeon.OnBossDefeated -= RedrawBossCounter;
        }
        
        protected override void OnWin(IGameMode mode)
        {
            base.OnWin(mode);
            _timer.Stop();
        }

        protected override void OnLose(IGameMode mode)
        {
            base.OnLose(mode);
            _timer.Stop();
        }

        protected override void Enable()
        {
            base.Enable();
            _timer.Listen(Dungeon.LoseDelay);
            RedrawBossCounter();
        }

        protected override void Disable()
        {
            base.Disable();
            _timer.Stop();
        }

        private void RedrawBossCounter() => 
            _bossCounter.SetText($"{Dungeon.DeadBossAmount}/{Dungeon.MaxBossAmount}");
    }
}
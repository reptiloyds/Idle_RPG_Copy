using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.View
{
    public class SoftRushView : DungeonView<SoftRushMode>
    {
        [SerializeField] private TextMeshProUGUI _lootedSoftText;
        [SerializeField] private Image _resourceImage;
        [SerializeField] private TextTimer _timer;

        public override void Initialize()
        {
            base.Initialize();
            Dungeon.OnLootChanged += RedrawLoot;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Dungeon.OnLootChanged -= RedrawLoot;
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
            _resourceImage.sprite = Dungeon.RewardImage;
            _timer.Listen(Dungeon.LoseDelay);
            RedrawLoot();
        }

        protected override void Disable()
        {
            base.Disable();
            _timer.Stop();
        }

        private void RedrawLoot() => 
            _lootedSoftText.SetText(StringExtension.Instance.CutBigDouble(Dungeon.LootedResourceAmount, true));
    }
}
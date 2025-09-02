using PleasantlyGames.RPG.Runtime.Ad.Definitions;
using PleasantlyGames.RPG.Runtime.AssetManagement.Definition;
using PleasantlyGames.RPG.Runtime.Audio.Definition;
using PleasantlyGames.RPG.Runtime.Balance.Definition;
using PleasantlyGames.RPG.Runtime.Core.Cheats;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Definition;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Definition;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Definition;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Definition;
using PleasantlyGames.RPG.Runtime.Core.Music.Definition;
using PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Definition;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Definitions;
using PleasantlyGames.RPG.Runtime.Core.Resource.Defenition;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Definition;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Pause.Definition;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.Definition;
using PleasantlyGames.RPG.Runtime.Save.Definitions;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PleasantlyGames.RPG.Runtime.Utilities.InternetConnection.Definition;
using UnityEngine;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "SO/GameConfig")]
    public class GameConfig : ScriptableObject
    {
	    public PauseConfiguration PauseConfiguration;
	    public SaveConfiguration SaveConfiguration;
	    public BalanceConfiguration BalanceConfiguration;
	    public AssetProviderDefinition AssetProviderDefinition;
	    public AdDefinition AdDefinition;
	    [Space]
		public MainModeConfiguration MainModeConfiguration;
	    public DungeonConfiguration DungeonConfiguration;
		public BossRushConfiguration BossRushConfiguration;
		public SoftRushConfiguration SoftRushConfiguration;
		public SlotRushConfiguration SlotRushConfiguration;
		[Space]
		public ResourceConfiguration ResourceConfiguration;
		public ItemConfiguration ItemConfiguration;
		public LootboxConfiguration LootboxConfiguration;
		public PassiveIncomeConfiguration PassiveIncomeConfiguration;
		public SkillConfiguration SkillConfiguration;
		public NotificationConfiguration NotificationConfiguration;
		public BigValueConfiguration BigValueConfiguration;
		public AudioConfig AudioConfiguration;
		public MusicConfiguration MusicConfiguration;
		public CheatConfiguration CheatConfiguration;
		public InternetConnectionConfiguration InternetConnectionConfiguration;
		public PrivacyPolicyConfiguration PrivacyPolicyConfiguration;
		public ChatDefinition ChatDefinition;
    }
}

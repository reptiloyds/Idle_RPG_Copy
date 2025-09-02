using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Characters
{
    public class CharacterPeriodicReward : PeriodicReward
    {
        private readonly CharacterService _characterService;
        private readonly CharacterPeriodicData _data;

        private Character _character;

        public CharacterPeriodicReward(PeriodicRewardSheet.Elem config, ISpriteProvider spriteProvider, ITranslator translator, CharacterService characterService)
            : base(config, spriteProvider, translator)
        {
            _characterService = characterService;
            _data = JsonConvert.DeserializeObject<CharacterPeriodicData>(config.RewardJSON);
        }

        public override void Initialize()
        {
            _character = _characterService.GetCharacter(_data.Id);
            base.Initialize();
            Text = _character.FormattedName;
            TypeText = Translator.Translate(TranslationConst.Character);
        }

        public override void Apply(Vector3 viewWorldPosition) => 
            _characterService.Own(_character);

        protected override Sprite GetImageInternal() => 
            _character.GetEvolutionSprite(_character.MaxEvolution);
    }
}
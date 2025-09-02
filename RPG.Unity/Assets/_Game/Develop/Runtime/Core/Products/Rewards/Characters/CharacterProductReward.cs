using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Characters
{
    public class CharacterProductReward : ProductReward
    {
        private readonly CharacterService _characterService;
        private readonly CharacterProductRewardData _data;
        private Sprite _sprite;

        public Character CharacterModel { get; }
        public string Id => _data.Id;
        public override Sprite Sprite => _sprite;
        public override ProductItemType Type => ProductItemType.Character;
        public override string Name => CharacterModel.FormattedName;

        public CharacterProductReward(CharacterService characterService,
            CharacterProductRewardData data,
            Character characterModel, Color backColor, ProductElem config,
            IUnlockable unlockable) : base(backColor, config, unlockable)
        {
            _characterService = characterService;
            CharacterModel = characterModel;
            _data = data;
        }

        public override void Initialize()
        {
            base.Initialize();
            _sprite = CharacterModel.GetEvolutionSprite(_data.UseLastImage ? CharacterModel.MaxEvolution : 0);
        }

        public override void Apply() => 
            _characterService.Own(CharacterModel);
    }
}
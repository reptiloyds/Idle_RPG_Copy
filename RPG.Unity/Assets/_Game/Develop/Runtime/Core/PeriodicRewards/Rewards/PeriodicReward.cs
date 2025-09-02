using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards
{
    public abstract class PeriodicReward
    {
        protected readonly PeriodicRewardSheet.Elem Config;
        protected readonly ISpriteProvider SpriteProvider;
        protected readonly ITranslator Translator;
        public string Text { get; protected set; }
        public string TypeText { get; protected set; }

        public Sprite Image { get; private set; }
        public Color Color => Config.Color;

        protected PeriodicReward(PeriodicRewardSheet.Elem config, ISpriteProvider spriteProvider, ITranslator translator)
        {
            Config = config;
            SpriteProvider = spriteProvider;
            Translator = translator;
        }

        public virtual void Initialize()
        {
            if (!string.IsNullOrEmpty(Config.Image)) 
                Image = SpriteProvider.GetSprite(Asset.MainAtlas, Config.Image);
            else
                Image = GetImageInternal();
        }

        public abstract void Apply(Vector3 viewWorldPosition);

        protected abstract Sprite GetImageInternal();
    }
}
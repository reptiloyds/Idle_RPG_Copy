using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model.Decorator
{
    public abstract class LootboxDecorator : ILootboxDecorator
    {
        private readonly ILootboxDecorator _decorator;

        protected LootboxDecorator(ILootboxDecorator decorator = null) =>
            _decorator = decorator;

        public virtual bool IsItemAvailable(Item item) => 
            _decorator == null || _decorator.IsItemAvailable(item);
    }
}
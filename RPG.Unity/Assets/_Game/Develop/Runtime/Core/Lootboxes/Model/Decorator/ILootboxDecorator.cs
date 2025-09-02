using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model.Decorator
{
    public interface ILootboxDecorator
    {
        bool IsItemAvailable(Item item);
    }
}
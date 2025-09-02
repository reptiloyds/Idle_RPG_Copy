using PleasantlyGames.RPG.Runtime.NodeMachine.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node
{
    internal abstract class UnitNode : BaseNode
    {
        protected readonly UnitView UnitView;

        protected UnitNode(UnitView unitView) => UnitView = unitView;
    }
}
namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Building
{
    public interface IUpdateableBuildElement : IBuildElement
    {
        void UpdateState(UnitView unitView);
    }
}
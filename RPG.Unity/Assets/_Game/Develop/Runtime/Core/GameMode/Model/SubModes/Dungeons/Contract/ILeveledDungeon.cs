namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Contract
{
    public interface ILeveledDungeon : IGameMode
    { 
        int LaunchLevel { get; }
        int AvailableLevel { get; }
        bool IsAutoSweep { get; }
        void SetLaunchLevel(int launchLevel);
    }
}
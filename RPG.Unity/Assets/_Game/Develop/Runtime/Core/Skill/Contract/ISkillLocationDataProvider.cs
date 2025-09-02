using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Contract
{
    public interface ISkillLocationDataProvider
    {
        Vector3 GetCenterPosition();
        Vector3 FindClosestPoint(Vector3 targetPosition);
        Vector3 GetRandomPosition();
        Vector3 GetRandomEmptyPosition();
        Vector3 GetCenterXZPosition();
    }
}
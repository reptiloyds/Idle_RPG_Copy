using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Launchers.View
{
    public class BomberProjectileLauncher : ProjectileLauncherView
    {
        [SerializeField] private Transform _shootPoint;
        public override void Focus(Vector3 position)
        {
        }

        public override Transform GetShootPoint() => 
            _shootPoint;
    }
}
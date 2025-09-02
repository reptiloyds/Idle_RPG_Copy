using System;
using PleasantlyGames.RPG.Runtime.Core.Location.View.Component.Movement;
using PleasantlyGames.RPG.Runtime.Core.Skill.View;
using PleasantlyGames.RPG.Runtime.Core.Units;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Location.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class LocationView : MonoBehaviour
    {
        [SerializeField] private UnitSpawnProvider _allySpawnProvider;
        [SerializeField] private UnitSpawnProvider _companionSpawnProvider;
        [SerializeField] private UnitSpawnProvider _enemySpawnProvider;
        [SerializeField] private UnitSpawnProvider _softRushSpawnProvider;
        [SerializeField, Required] private LocationMovement _locationMovement;
        [SerializeField, Required] private BattlefieldContext _battlefieldContext;

        public UnitSpawnProvider AllySpawn => _allySpawnProvider;
        public UnitSpawnProvider CompanionSpawn => _companionSpawnProvider;
        public UnitSpawnProvider EnemySpawn => _enemySpawnProvider;
        public UnitSpawnProvider SoftRushSpawn => _softRushSpawnProvider;

        public event Action OnMovementStarted;
        public event Action OnMovementStopped;

        private void Reset() => 
            _locationMovement = GetComponentInChildren<LocationMovement>();

        public float GetSpeed() => 
            _locationMovement.Speed;

        public void StartMovement(float speed)
        {
            _locationMovement.StartMovement(speed);
            OnMovementStarted?.Invoke();
        }

        public void StopMovement()
        {
            _locationMovement.StopMovement();
            OnMovementStopped?.Invoke();
        }

        public LocationPlatform GetClosestPlatform(Vector3 targetPosition) => 
            _locationMovement.GetClosestPlatform(targetPosition);

        public void AppendChild(Transform child)
        {
            var platform = GetClosestPlatform(child.transform.position);
            platform.AppendChild(child);
        }

        public void Clear()
        {
            _locationMovement.Clear();
            _battlefieldContext.Clear();
        }
    }
}
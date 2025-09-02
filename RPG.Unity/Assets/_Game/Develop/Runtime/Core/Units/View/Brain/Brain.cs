using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Behaviour;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Type;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain
{
    public class Brain : BaseBrain
    {
        [SerializeField] private SearchType _enemySearchType = SearchType.Closest;
        [Inject] private IObjectResolver _objectResolver;

        public override void Initialize()
        {
            base.Initialize();
            
            var idle = new IdleBehaviour(Unit);
            _objectResolver.Inject(idle);
            idle.Initialize();
            AddBehaviour(UnitBehaviourType.Idle, idle);

            var moveForward = new MoveForwardBehaviour(Unit);
            _objectResolver.Inject(moveForward);
            moveForward.Initialize();
            AddBehaviour(UnitBehaviourType.MoveForward, moveForward);

            var fight = new FightBehaviour(Unit, _enemySearchType);
            _objectResolver.Inject(fight);
            fight.Initialize();
            AddBehaviour(UnitBehaviourType.Fight, fight);
            
            var simulateMovement = new SimulateMovementBehaviour(Unit);
            _objectResolver.Inject(simulateMovement);
            simulateMovement.Initialize();
            AddBehaviour(UnitBehaviourType.SimulateMovement, simulateMovement);
            
            var backToSpawn = new MoveToTargetBehaviour(Unit);
            _objectResolver.Inject(backToSpawn);
            backToSpawn.Initialize();
            AddBehaviour(UnitBehaviourType.MoveToTarget, backToSpawn);
        }
    }
}
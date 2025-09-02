using PleasantlyGames.RPG.Runtime.Core.Units.Team;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Node;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates;
using PleasantlyGames.RPG.Runtime.NodeMachine.Model;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Behaviour
{
    internal class FightBehaviour : BehaviourMachine
    {
        [Inject] private MortalUnitContainer _mortalUnitContainer;
        
        private readonly VariableContainer<UnitView> _target = new();
        private readonly SearchType _enemySearchType;
        
        public FightBehaviour(UnitView unitView, SearchType enemySearchType) : base(unitView)
        {
            _enemySearchType = enemySearchType;
        }

        public override void Initialize()
        {
            var idle = new Idle(UnitView, 0.1f);
            var searchTarget = new SearchTarget(UnitView, _mortalUnitContainer, _enemySearchType, _target);
            var followForAttack = new FollowForAttack(UnitView, _target, 0.5f);
            var attack = new Attack(UnitView, _target);
            
            AddTransition(searchTarget, idle, () => searchTarget.Failed);
            AddTransition(idle, searchTarget, () => idle.Completed);
            AddTransition(searchTarget, followForAttack, () => searchTarget.Completed);
            AddTransition(followForAttack, searchTarget, () => followForAttack.Failed);
            AddTransition(followForAttack, attack, () => followForAttack.Completed);
            AddTransition(attack, searchTarget, () => attack.Completed || attack.Failed);

            StartNode = searchTarget;
        }

        public override void Enter()
        {
            base.Enter();
            UnitView.StateMachine.SwitchState(StateType.Combat);
        }

        public override void Exit()
        {
            base.Exit();

            var unit = _target.Get();
            if (unit != null) 
                unit.ReleaseTargetForEnemy(UnitView.gameObject);
            
            UnitView.StateMachine.SwitchState(StateType.Normal);
        }
    }
}
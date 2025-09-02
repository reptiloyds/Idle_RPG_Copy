using System;
using PleasantlyGames.RPG.Runtime.NodeMachine.Model;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Brain.Behaviour
{
    public abstract class BehaviourMachine
    {
        private readonly NodeMachine.Model.NodeMachine _nodeMachine = new(10);
        protected BaseNode StartNode;
        protected readonly UnitView UnitView;

        protected BehaviourMachine(UnitView unitView) => UnitView = unitView;
        
        public event Action OnNodeChanged;

        public abstract void Initialize();

        public virtual void Enter()
        {
            _nodeMachine.OnNodeChanged += TriggerNodeChanged; 
            _nodeMachine.SetState(StartNode);
        }

        public virtual void Exit()
        {
            _nodeMachine.Clear();
            _nodeMachine.OnNodeChanged -= TriggerNodeChanged;
        }

        private void TriggerNodeChanged() => 
            OnNodeChanged?.Invoke();

        public void AddTransition(BaseNode from, BaseNode to, Func<bool> predicate) => 
            _nodeMachine.AddTransition(from, to, predicate);

        public virtual void Update() => 
            _nodeMachine.Update();
        
        public string GetNodeName() => 
            _nodeMachine.CurrentNode.ToString();
    }
}
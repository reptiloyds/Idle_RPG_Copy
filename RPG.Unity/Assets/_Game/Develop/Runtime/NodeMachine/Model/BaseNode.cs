using System;

namespace PleasantlyGames.RPG.Runtime.NodeMachine.Model
{
    public abstract class BaseNode
    {
        private Action _onEnter;
        private Action _onExit;
        private Action _onCompleteAction;
        private Action _onFailAction;
        
        private bool _isActive = true;
        
        public bool Completed { get; private set; }
        public bool Failed { get; private set; }

        public virtual void Enter()
        {
            _isActive = true;
            _onEnter?.Invoke();
        }

        public virtual void Exit()
        {
            _isActive = false;
            Completed = false;
            Failed = false;
            _onExit?.Invoke();
        }
        
        public void OnEnter(Action action) => _onEnter = action;
        
        public void OnExit(Action action) => _onExit = action;
        
        public void OnComplete(Action action) => _onCompleteAction = action;

        public void OnFail(Action action) => _onFailAction = action;

        public virtual void Update() { }

        protected virtual void Complete()
        {
            if(!_isActive) return;
            _onCompleteAction?.Invoke();
            Completed = true;
        }

        protected virtual void Fail()
        {
            if(!_isActive) return;
            _onFailAction?.Invoke();
            Failed = true;
        }
    }
}
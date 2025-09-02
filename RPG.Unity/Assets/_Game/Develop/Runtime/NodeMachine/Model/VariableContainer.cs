using System;

namespace PleasantlyGames.RPG.Runtime.NodeMachine.Model
{
    public class VariableContainer<T>
    {
        protected T Variable;

        public event Action OnChange;

        public VariableContainer() { }

        public VariableContainer(T variable) => Variable = variable;
        
        public virtual void Set(T variable)
        {
            Variable = variable;
            OnChange?.Invoke();
        }

        public virtual T Get()
        {
            return Variable;
        }
    }
}
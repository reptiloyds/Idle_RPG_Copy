using System;
using System.Collections.Generic;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.NodeMachine.Model
{
    public class NodeMachine
    {
        private class Transition
        {
            public Func<bool> Condition { get; }
            public BaseNode From { get; }
            public BaseNode To { get; }

            public Transition(BaseNode from, BaseNode to, Func<bool> condition)
            {
                From = from;
                To = to;
                Condition = condition;
            }
        }
        
        private static readonly List<Transition> EmptyTransitions = new (0);
        
        private readonly Dictionary<Type, List<Transition>> _transitions = new ();
        private readonly List<Transition> _anyTransitions = new ();
        
        private List<Transition> _currentTransitions = new ();

        private readonly int _interval;
        private float _nextCheckTime;
        
        public BaseNode CurrentNode { get; private set; }
        public event Action OnNodeChanged;
        
        public NodeMachine(int interval = 10) => _interval = interval;

        public void Update()
        {
            if (Time.frameCount % _interval == 0)
            {
                Transition transition = GetTransition();
                if (transition != null)
                    SetState(transition.To);
            }
      
            CurrentNode?.Update();
        }
        
        public void SetState(BaseNode newNode)
        {
            if (newNode == CurrentNode)
                return;

            CurrentNode?.Exit();
      
            CurrentNode = newNode;
      
            _transitions.TryGetValue(CurrentNode.GetType(), out _currentTransitions);
            if (_currentTransitions == null)
                _currentTransitions = EmptyTransitions;

            CurrentNode.Enter();
            
            OnNodeChanged?.Invoke();
        }
        
        public void AddTransition(BaseNode from, BaseNode to, Func<bool> predicate)
        {
            if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
            {
                transitions = new List<Transition>();
                _transitions[from.GetType()] = transitions;
            }

            transitions.Add(new Transition(from, to, predicate));
        }
        
        public void AddAnyTransition(BaseNode node, Func<bool> predicate)
        {
            _anyTransitions.Add(new Transition(null, node, predicate));
        }
        
        private Transition GetTransition()
        {
            foreach (Transition transition in _anyTransitions)
            {
                if (transition.Condition()) 
                    return transition;
            }

            foreach (Transition transition in _currentTransitions)
            {
                if (transition.Condition() && transition.From == CurrentNode) 
                    return transition;
            }

            return null;
        }

        public void Clear()
        {
            if (CurrentNode == null) return;
            CurrentNode.Exit();
            CurrentNode = null;
        }
    }
}
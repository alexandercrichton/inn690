using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Simulation.WorldState
{
    public delegate void GoalSatisfiedHandler(Goal goal);
    
    /// <summary>
    /// A goal is just a collection of states with a specific value, typically defined as string literals.
    /// When a goal is satisfied, it fires off its completed event. 
    /// </summary>
    public class Goal
    {
        private bool _isSatisfied;
        public List<State> GoalStates { get; set; }
        public event GoalSatisfiedHandler GoalSatisfied;
        
        public Goal()
        {
            _isSatisfied = false;
            GoalStates = new List<State>();
        }

        public void Satisfy()
        {
            _isSatisfied = true;
            OnSatisfied();
        }

        public bool IsSatisfied()
        {
            return _isSatisfied;
        }

        private void OnSatisfied()
        {
            if (GoalSatisfied != null)
                GoalSatisfied(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Goal: ");
            foreach (var state in GoalStates)
            {
                sb.Append(state.StateLiteral());
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Simulation.WorldState
{
    /// <summary>
    /// The goal service determines whether goals are completed based on the world state.
    /// When the world state is updated, it checks whether any of its goals are
    /// completed. If a goal is completed, it is removed. 
    /// </summary>
    public class GoalService
    {
        private WorldStateService _stateService;
        private readonly StateComparer _stateComparer;
        private List<Goal> _registeredGoals;

        public GoalService(WorldStateService stateService)
        {
            _stateService = stateService;
            _registeredGoals = new List<Goal>();
            _stateComparer = new StateComparer();
            _stateService.WorldStateUpdated += ValidateGoals;
        }

        public void RegisterGoal(Goal goal)
        {
            // Make sure its not already satisfied
            bool satisfied = ValidateGoal(goal);
            if (satisfied)
                goal.Satisfy();
            else
                _registeredGoals.Add(goal);            
        }

        /// <summary>
        /// Checks all registered goals against the world state to
        /// test for completion. If a goal is satisfied, it is
        /// removed from the list of registered goals. 
        /// </summary>
        public void ValidateGoals()
        {
            lock (_registeredGoals)
            {
                var satisfiedGoals = new List<Goal>();
                foreach (Goal goal in _registeredGoals)
                {
                    bool satisfied = ValidateGoal(goal);
                    if (satisfied)
                        satisfiedGoals.Add(goal);
                }
                satisfiedGoals.ForEach(g => g.Satisfy());
                _registeredGoals.RemoveAll(g => g.IsSatisfied());
            }
        }

        /// <summary>
        /// Checks if a Goal's states is satisfied by the world state.
        /// </summary>
        /// <returns>True if all goal states are satisfied</returns>
        public bool ValidateGoal(Goal goal)
        {
            // Find the world states relavent to the goal states
            var allStates = _stateService.GetAll();
            var relevantStates = allStates
                .Where(state => goal.GoalStates.Any(goalstate => 
                    String.Compare(goalstate.Asset, state.Asset, true) == 0 && 
                    String.Compare(goalstate.Predicate,state.Predicate, true) == 0));

            // For each goal state, compare its existance in the list of total states
            foreach (State goalState in goal.GoalStates)
            {
                // If the goal state is not in the world state
                if (!relevantStates.Contains(goalState, _stateComparer))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Removes all goals from this goal service
        /// </summary>
        public void ClearGoals()
        {
            _registeredGoals.Clear();
        }
    }
}

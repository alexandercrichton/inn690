using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;
using Veis.Simulation.WorldState;
using Veis.Data.Entities;
using Veis.Data.Services;
using Veis.Services.Interfaces;
using Veis.Common.Math;
using Veis.Common;
using Veis.Bots;
using Veis.Data;
using Veis.Data.Repositories;

namespace Veis.Planning
{
    /// <summary>
    /// This planner uses HTN knowlegde to extract the necessary methods that
    /// the NPC needs to perform in order to complete the work item.
    /// At: 10/08/12 - Functionality for determining correct methods (via HTN) is not implemented
    ///                Waiting for Hanwen's HTN resolving. CURRENTLY VERY LIMITED.
    /// </summary>
    public class GoalBasedWorkItemPlanner : Planner<WorkItem>
    {
        private const int PAUSE_TIME = 2; // seconds
        private readonly IDecompositionService<WorkItem> _decompService;
        private readonly IActivityMethodService _methodService;
        private readonly ISceneService _sceneService;
        private readonly IRepository<WorldState> _worldState;

        public GoalBasedWorkItemPlanner(IDecompositionService<WorkItem> decompService, 
            IActivityMethodService methodService,
            IRepository<WorldState> worldState, 
            ISceneService sceneService)
        {
            _decompService = decompService;
            _methodService = methodService;
            _sceneService = sceneService;
            _worldState = worldState;
        }
        
        // eg. Bed_1:At;Bay_10
        // Find a function that involves the asset des 
        public PlanResult MakePlan(WorkItem input)
        {
            var plan = new PlanResult();
            var goals = _decompService.Decompose(input);
            foreach (var goal in goals)
            {
                // For each goal state, find the method that satisfied the condition, given the asset
                foreach (var goalState in goal.GoalStates)
                {
                    plan.Tasks.AddRange(FormulateNPCTasks(goalState));
                }
            }
            plan.Tasks.Add("COMPLETEWORK:" + input.TaskID);
            return plan;
        }

        /// <summary>
        /// Generates the list of atomic tasks that the NPC needs to 
        /// perform in order to execute an action on an asset.
        /// </summary>
        /// <returns>List of atomic tasks</returns>
        public List<String> FormulateNPCTasks(State goalState)
        {
            var tasks = new List<string>();

            // MOVE to object (if exists)
            tasks.Add(AvailableActions.WALKTO + ":" + goalState.Asset);

            // EXECUTE METHOD on OBJECT
            tasks.Add(AvailableActions.ASSETINTERACTION 
                + ":" + goalState.Asset 
                + ":" + goalState.Predicate
                + ":" + goalState.Value);

            ActivityMethod method = FindMethodToSatisfyPredicate(goalState);
            if (method != null)
            {
                tasks.Add(AvailableActions.ASSETSERVICEROUTINE
                + ":" + goalState.Asset
                + ":" + method.Name
                + ":" + goalState.Value);
            }

            // TODO: Animations when interacting with assets (e.g. typing animation)
			tasks.Add(AvailableActions.ANIMATE + ":" + method.Name);


            return tasks;
        }

        /// <summary>
        /// Determines which method is necessary to reach the given goal state
        /// </summary>
        public ActivityMethod FindMethodToSatisfyPredicate(State goalState)
        {
            var assetMethods = _methodService.GetMethodsByAsset(goalState.Asset); // Other asset's methods may apply to this goal state. TODO

            // find method that has postcondition with the given predicate
            var applicable = assetMethods
                .Where(m => m.Postconditions
                    .Any(post => post.Predicate
                        .Equals(goalState.Predicate, StringComparison.OrdinalIgnoreCase)));

            // TODO: Check for preconditions
            // OR choose the first method
            var chosenMethod = applicable.FirstOrDefault();

            return chosenMethod;
        }

        #region Unnecessary?

        // These methods seem quite unnecessary to achieve goals. Bots only need to do two key things:
        // update the asset world state, and (if applicable) insert an asset service routine.
        // The appropriate world state asset predicate can be selected 
        // using goalState.Asset and goalState.Predicate, and its value can be updated using
        // goalState.Value. These properties are already present as part of goalState, surely
        // they don't need to be queried again?
        // Asset service routines aren't necessary for every asset interaction. One is required
        // if an associated method can be selected based on goalState.Asset and goalState.Predicate.
        // If a service routine is required only the associated method and the goalState.Value
        // need to be passed to the bot. There is no need to formulate parameters. Any parameters
        // formed are just decomposed back to their original state at the other end anyway.
        // Based on those thoughts these methods have been retired in favour of a simpler approach
        // using values already present in goalState.

        /*

        /// <summary>
        /// Determines the value of the method parameters that are needed to reach the goal state.
        /// </summary>
        public IDictionary<string, string> FormulateMethodParameters(State goalState, ActivityMethod activityMethod)
        {
            var parameters = new Dictionary<string, string>();

            // Get all variables from parameters of method
            var allVariables = activityMethod.Parameters.Select(p => p.Variable).ToList();
            // Get asset states
            var assetStates = _worldState.Find(new WorldStateRepository.ByAssetSpecification(goalState.Asset)).ToList();

            // Each variabl has an associated predicate from the method_preconditions
            var hiddenVariableValuePairs = new Dictionary<string, string>();
            var freeVariables = new List<string>();
            // Free variables are those which don't have an existing world state with the same predicate

            allVariables.ForEach(v =>
            {
                // Find the matching predicate
                var predicate = activityMethod.Preconditions
                    .Where(p => p.Variable.Equals(v, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.Predicate)
                    .FirstOrDefault();
                //if (predicate == null) return;
                // If there is a world state that matches the predicate, then use it as a hidden variable
                var potentionalValue = assetStates
                    .Where(s => s.PredicateLabel.Equals(predicate, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                if (potentionalValue != null)
                {
                    hiddenVariableValuePairs.Add(v, potentionalValue.Value);
                }
                else // It is a free variable, and needs to be provided with a value
                {
                    freeVariables.Add(v);
                }

            });

            // TODO: Determine values of all variables. Find any extra parameters that need to be given a value
            if (freeVariables.Count() > 0)
            {
                parameters.Add(freeVariables.ElementAt(0), goalState.Value.ToString());
            }
            // TODO: I couldn't figure out why the the parameters mapped to the current world state were
            // being passed. Bots don't need to know the current state of the world to update the database
            // and/or insert a service routine, they only need the parameters matching the goal state. 
            // In fact it just seems to make it harder because then
            // at the other end the current state parameters need to be filtered out again so the bots
            // aren't writing the same state back into the database. I'm just leaving this here in case 
            // I missed something, but presently it just poses a problem.

            //foreach (var hiddenVariable in hiddenVariableValuePairs)
            //{
            //    parameters.Add(hiddenVariable.Key, hiddenVariable.Value);
            //}
            return parameters;
        }
          
        */

        #endregion
    }
}

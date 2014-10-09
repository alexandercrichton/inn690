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
                    var method = FindMethodToSatisfyPredicate(goalState);
                    IDictionary<string, string> parameters = new Dictionary<string, string>();
                    if (method != null)
                    {
                        // Determine the necessary parameters to add to the method execution
                        parameters = FormulateMethodParameters(goalState, method);
                    }

                    plan.Tasks.AddRange(FormulateNPCTasks(goalState.Asset, method, parameters));
                }
            }
            return plan;
        }

        /// <summary>
        /// Determines which method is necessary to reach the given goal state
        /// </summary>
        public ActivityMethod FindMethodToSatisfyPredicate(State goalState)
        {
            var assetMethods = _methodService.GetMethodsByAsset(goalState.Asset); // Other asset's methods may apply to this goal state. TODO
             
            // find method that has postcondition with the given predicate
            var applicable = assetMethods.Where(
                m => m.Postconditions
                    .Any(post => post.Predicate.Equals(goalState.Predicate, StringComparison.OrdinalIgnoreCase)));
            
            // TODO: Check for preconditions
            // OR choose the first method
            var chosenMethod = applicable.FirstOrDefault();
            return chosenMethod;
        }

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
                var predicate = activityMethod.Preconditions.Where(p => p.Variable.Equals(v, StringComparison.OrdinalIgnoreCase)).Select(p => p.Predicate).FirstOrDefault();
                if (predicate == null) return;
                // If there is a world state that matches the predicate, then use it as a hidden variable
                var potentionalValue = assetStates.Where(s => s.PredicateLabel.Equals(predicate, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
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
            foreach (var hiddenVariable in hiddenVariableValuePairs)
            {
                parameters.Add(hiddenVariable.Key, hiddenVariable.Value);
            }

            return parameters;
        }

        /// <summary>
        /// Generates the list of atomic tasks that the NPC needs to 
        /// perform in order to execute an action on an asset.
        /// </summary>
        /// <returns>List of atomic tasks</returns>
        public List<String> FormulateNPCTasks(string asset, ActivityMethod method, IDictionary<string, string> methodParameters)
        {
            var tasks = new List<string>();
            
            // FIND asset [if it doesn't exist, do nothing]
            //Vector3 location = _sceneService.GetPositionOfObject(asset);
            //if (location == null) return tasks;

            // MOVE to object (if exists)
            tasks.Add(AvailableActions.WALKTO + ":" + asset);

            // FIND SITTABLE OBJECT (named <asset> chair)
            //var chairName = asset + " chair";
            //Vector3 chairLocation = _sceneService.GetPositionOfObject(chairName);
            //if (chairLocation != null)
            //{
            //    // Sit if exists
            //    tasks.Add(AvailableActions.SIT + ":" + chairName);             
            //}

            // TODO: When animation reference system is in place, perform any necessary animations

            // WAIT for a short time (2 seconds)
            //tasks.Add(AvailableActions.WAIT + ":" + PAUSE_TIME);

            // EXECUTE METHOD on OBJECT
            tasks.Add(AvailableActions.EXECUTEACTION + ":" + asset + ":" + method.Name + ":" + StringFormattingExtensions.EncodeParameterString(methodParameters));
			tasks.Add(AvailableActions.ANIMATE + ":" + method.Name);
            // TOUCH OBJECT (which should  be scripted)
            // NOTE: In OpenSim, the object will be scripted to exectute the action via php
            // Other implemenations may do something different when the "EXECUTEACTION" occurs.
            //tasks.Add(AvailableActions.TOUCH + ":" + asset);

            return tasks;
        }

    }
}

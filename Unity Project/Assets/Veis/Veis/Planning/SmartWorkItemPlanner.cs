using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;

namespace Veis.Planning
{
    /// <summary>
    /// This planner holds a number of workitem planners. Based on the workitem,
    /// it determines which planner is the best fit for purpose.
    /// If more planning classes are added, selection logic should be added here.
    /// </summary>
    public class SmartWorkItemPlanner : Planner<WorkItem>
    {
        BasicWorkItemPlanner _basic;
        GoalBasedWorkItemPlanner _goalBased;
        private const string GoalVariableName = "Goals";

        public SmartWorkItemPlanner(BasicWorkItemPlanner basic, GoalBasedWorkItemPlanner goalBased)
        {
            _basic = basic;
            _goalBased = goalBased;
        }

        public PlanResult MakePlan(WorkItem input)
        {
            // If input has goals, use the goal based planner
            if (input.tasksAndGoals.ContainsKey(GoalVariableName) && !String.IsNullOrEmpty(input.tasksAndGoals[GoalVariableName]))
            {
                return _goalBased.MakePlan(input);
            }
            else
            {
                return _basic.MakePlan(input);
            }
        }
    }
}

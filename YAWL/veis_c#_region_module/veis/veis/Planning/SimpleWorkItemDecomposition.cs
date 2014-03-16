using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;
using Veis.Simulation.WorldState;
using Veis.Workflow.YAWL;

namespace Veis.Planning
{
    public class SimpleWorkItemDecomposition : IDecompositionService<WorkItem>
    {
        
        private const string GoalVariableName = "Goals";
        
        public List<Goal> Decompose(WorkItem input)
        {
            // The workitem should contain a list of goals in the goal "task variable"
            if (!input.taskVariables.ContainsKey(GoalVariableName)) return new List<Goal>();
            
            var goalVariable = input.taskVariables[GoalVariableName];
            Goal newGoal = goalVariable.ExtractGoal();
            return new List<Goal> { newGoal };
        }

    }
}

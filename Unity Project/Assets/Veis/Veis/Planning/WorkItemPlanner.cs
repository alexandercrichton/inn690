using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;

namespace Veis.Planning
{
    public class WorkItemPlanner : Planner<WorkItem>
    {
        public PlanResult MakePlan(WorkItem input)
        {
            // TODO: Fill this in with HANWEN's stuff
            PlanResult plan = new PlanResult();

            String animation = String.Empty;

            if (input.TaskName.ToLower().Contains("laugh"))
            {
                animation = "EXPRESS_LAUGH".ToLower();
            }
            else if (input.TaskName.ToLower().Contains("dance"))
            {
                animation = "DANCE1".ToLower();
            }
            else if (input.TaskName.ToLower().Contains("wave"))
            {
                animation = "BLOWKISS".ToLower();
            }
            else if (input.TaskName.ToLower().Contains("punch"))
            {
                animation = "PUNCH_ONETWO".ToLower();
            }
            
            // Basic physical plan, that attempts to perform the given animation from the workitem
            plan.Tasks.Add("ANIMATE:" + animation);

            return plan;
        }
    }
}

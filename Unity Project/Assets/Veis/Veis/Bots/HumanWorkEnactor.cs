using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;

using System.Threading;
using Veis.Planning;
using Veis.Simulation.WorldState;

namespace Veis.Bots
{
    public class HumanWorkEnactor : WorkEnactor
    {
        new public HumanAvatar Avatar;

        private readonly IDecompositionService<WorkItem> _decompService; // Like the planner, except it produces some goals for a work item
        private readonly GoalService _goalService;

        private IDictionary<WorkItem, List<Goal>> _workitemGoals;
        private IDictionary<WorkItem, List<Goal>> _completedGoals;


        public HumanWorkEnactor(HumanAvatar user, WorkAgent workAgent,
            WorkflowProvider workflow, IDecompositionService<WorkItem> decompService,
            GoalService goalService)
        {
            Avatar = user;
            WorkflowProvider = workflow;
            WorkAgent = workAgent;
            _decompService = decompService;
            _goalService = goalService;
            _workitemGoals = new Dictionary<WorkItem, List<Goal>>();
            _completedGoals = new Dictionary<WorkItem, List<Goal>>();
        }

        // 1. Work item is added
        public override void AddWork(WorkItem workItem)
        {           
            // 2. Work task is decomposed
            List<Goal> newGoals = _decompService.Decompose(workItem);

            // 3. The goal's satisfied event is registered with this work enactor
            GoalSatisfiedHandler handler = null;
            handler = delegate(Goal g)
                {
                    GoalSatisified(g, workItem);
                    g.GoalSatisfied -= handler;
                };
            newGoals.ForEach(g => g.GoalSatisfied += handler);

            // 4. The goals are added to this enactor's list against the workitem
            //    and also the list of workitems being processed
            _workitemGoals.Add(workItem, newGoals);
            StartWork(workItem);

            // 5. Goals are registered with the world state service
            newGoals.ForEach(_goalService.RegisterGoal);

            // 6. Notify user of goals etc...
            foreach (var goal in newGoals)
            {
                Avatar.NotifyUser(workItem.taskName + " - " + goal.ToString());
            }
            
        }

        public void GoalSatisified(Goal goal, WorkItem workitem)
        {
            if (_workitemGoals.ContainsKey(workitem))
            {
                // 8. Remove the goal from the list of necessary goals
                // _workitemGoals[workitem].Remove(goal);
                // 9. If all goals are satisfied, check out (complete) the work item
                if (_workitemGoals[workitem].Count(g => !g.IsSatisfied()) == 0)
                {
                    CompleteWork(workitem);
                }
            }
        }

        public override void StopTaskIfStarted(WorkItem workItem)
        {
            if (WorkAgent.started.Contains(workItem))
            {
                if (_workitemGoals.ContainsKey(workItem))
                {
                    _workitemGoals.Remove(workItem);
                }
            }
        }

        public override void StartWork(WorkItem workItem)
        {
            lock (WorkAgent.processing)
            {
                WorkAgent.processing.Add(workItem);
            }
            lock (WorkAgent.started)
            {
                WorkAgent.started.Add(workItem);
            }
        }

        public override void CompleteWork(WorkItem workItem)
        {
            if (/*_workAgent.started.Contains(workItem) &&*/ WorkAgent.processing.Contains(workItem))
            {
                Avatar.NotifyUser("Just completed workitem: " + workItem.taskName);
                WorkAgent.Complete(workItem, WorkflowProvider);
            }
            _completedGoals.Add(workItem, _workitemGoals[workItem]);
            _workitemGoals.Remove(workItem);
        }

        public IDictionary<WorkItem, List<Goal>> GetGoals()
        {
            return _workitemGoals;
        }

        public void ClearCompletedGoals()
        {
            _completedGoals.Clear();
        }
    }
}

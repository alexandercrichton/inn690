using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Bots;
using Veis.Workflow;
using Veis.Simulation.WorldState;
using Veis.Workflow.YAWL;

namespace Veis.WebInterface.Models
{
    public class WorklistPersonalModel
    {
        public string UUID { get; set; }
        public string UserName { get; set; }
        public IList<WorkListItemModel> ProcessingWorkItems { get; set; }
        public IList<WorkListItemModel> OfferedWorkItems { get; set; }

        public WorklistPersonalModel()
        {
            ProcessingWorkItems = new List<WorkListItemModel>();
            OfferedWorkItems = new List<WorkListItemModel>();
        }

        public void Bind(HumanAvatar worklistOwner, string uuid)
        {
            UUID = uuid;
            UserName = worklistOwner.Name ?? worklistOwner.ActingName;
            var goals = worklistOwner.WorkProvider.GetGoals();

            foreach (var goal in goals)
            {
                var model = new WorkListItemModel();
                model.Bind(goal.Key, goal.Value);
                ProcessingWorkItems.Add(model);
            }

            foreach (var offered in worklistOwner.WorkProvider.WorkAgent.offered)
            {
                var model = new WorkListItemModel();
                model.Bind(offered, new List<Goal>());
                OfferedWorkItems.Add(model);
            }
        }
    }

    public class WorkListItemModel
    {
        public string Name { get; set; }
        public IEnumerable<string> Tasks { get; set; }
        public IEnumerable<string> Goals { get; set; }

        public void Bind(WorkItem workItem, List<Goal> goals)
        {
            Name = workItem.FormatWorkitemName();
            Goals = goals.SelectMany(g => g.GoalStates.Select(s => String.Format("{0} {1} {2}", s.Asset, s.Predicate, s.Value))).ToList();
            Tasks = workItem.taskVariables.ContainsKey("Tasks") ? workItem.taskVariables["Tasks"].ExtractTasks() : new List<string>();
        }
    }
}

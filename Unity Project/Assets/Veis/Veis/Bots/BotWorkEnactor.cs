using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;
using Veis.Planning;
using Veis.Data.Logging;

namespace Veis.Bots
{
    public class BotWorkEnactor : WorkEnactor
    {
        new private readonly BotAvatar Avatar;
        private readonly Planner<WorkItem> _planner;
        private WorkItem currentWorkItem;

        public BotWorkEnactor(BotAvatar avatar, WorkflowProvider provider, WorkAgent workAgent, Planner<WorkItem> planner)
        {
            Avatar = avatar;
            WorkflowProvider = provider;
            WorkAgent = workAgent;
            _planner = planner;
        }
        
        public override void AddWorkItem(WorkItem workItem)
        {
            if (workItem.TaskQueue == WorkAgent.STARTED
                && !WorkAgent.completed.Any(i => i.TaskID == workItem.TaskID))
            {
                WorkAgent.started.Add(workItem);
            }
            //Avatar.AddTaskToQueue("STARTWORK:" + workItem.TaskID);
        }

        public bool IsWorkAvailable()
        {
            return (WorkAgent.processing.Count <= 0 && WorkAgent.started.Count > 0);
        }

        public Queue<string> GetNextTasks()
        {
            StartWorkItem(WorkAgent.started.FirstOrDefault());

            WorkItem workItem = WorkAgent.processing[0];
            IList<string> tasks = _planner.MakePlan(workItem).Tasks; // HERE is where the workitem tasks are EXTRACTED
            return new Queue<string>(tasks);
        }

        public override void StartWorkItem(WorkItem workItem)
        {
            WorkAgent.processing.Add(workItem);
        }

        public void StartWork(string taskID)
        {
            StartWorkItem(WorkAgent.GetWorkItem(taskID, WorkAgent.started));
        }

        public override void CompleteWorkItem(WorkItem workItem)
        {
            if (WorkAgent.processing.Contains(workItem))
            {
                WorkAgent.Complete(workItem, WorkflowProvider);
            }
        }

        public void CompleteWork(string taskID)
        {
            CompleteWorkItem(WorkAgent.processing.FirstOrDefault(w => w.TaskID == taskID));
        }

        public override void ClearAll()
        {
            WorkAgent.started.Clear();
            WorkAgent.processing.Clear();
            lock (Avatar.taskQueue)
            {
                Avatar.taskQueue.Clear();
            }
        }

        public override void StopWorkItem(WorkItem workItem)
        {
            //if (WorkAgent.started.Contains(workItem))
            //{
            //    lock (Avatar.taskQueue)
            //    {
            //        if (WorkAgent.processing.Contains(workItem))
            //        {
            //            //We have a slight problem?
            //            while (Avatar.taskQueue.First.Value != "COMPLETEWORK:" + workItem.TaskID && Avatar.taskQueue.Count > 0)
            //            {
            //                Avatar.taskQueue.RemoveFirst();
            //            }
            //            Avatar.taskQueue.RemoveFirst();
            //        }
            //        else
            //        {
            //            //remove the start item thingo
            //            Stack<String> reverseTasks = new Stack<string>();
            //            while (Avatar.taskQueue.First.Value != "STARTWORK:" + workItem.TaskID && Avatar.taskQueue.Count > 0)
            //            {
            //                reverseTasks.Push(Avatar.taskQueue.First.Value);
            //                Avatar.taskQueue.RemoveFirst();
            //            }
            //            if (Avatar.taskQueue.Count > 0)
            //            {
            //                Avatar.taskQueue.RemoveFirst();
            //            }
            //            while (reverseTasks.Count > 0)
            //            {
            //                Avatar.taskQueue.AddFirst(reverseTasks.Pop());
            //            }
            //        }
            //    }
            //}
        }

        public WorkAgent GetWorkAgent()
        {
            return WorkAgent;
        }

        public WorkflowProvider GetWorkflowProvider()
        {
            return WorkflowProvider;
        }
    }
}

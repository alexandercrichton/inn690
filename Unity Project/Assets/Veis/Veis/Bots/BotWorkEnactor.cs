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

        public BotWorkEnactor(BotAvatar avatar, WorkflowProvider provider, WorkAgent workAgent, Planner<WorkItem> planner)
        {
            Avatar = avatar;
            WorkflowProvider = provider;
            WorkAgent = workAgent;
            _planner = planner;
        }
        
        public override void AddWork(WorkItem workItem)
        {
            lock (WorkAgent.started)
            {
                Avatar.AddTaskToQueue("STARTWORK:" + workItem.TaskID);
            }
        }

        public override void StartWork(WorkItem workItem)
        {
            lock (WorkAgent.processing)
            {
                WorkAgent.processing.Add(workItem);
            }
            AddWorkTasks(workItem);
        }

        public void StartWork(string workItemId)
        {
            Veis.Unity.Logging.UnityLogger.BroadcastMesage(this, "Starting work: " + WorkAgent.GetWorkItem(workItemId, WorkAgent.started).TaskName);
            StartWork(WorkAgent.GetWorkItem(workItemId, WorkAgent.started));
        }

        public override void CompleteWork(WorkItem workItem)
        {
            if (/*_workAgent.started.Contains(workItem) &&*/ WorkAgent.processing.Contains(workItem))
            {
                WorkAgent.Complete(workItem, WorkflowProvider);
            }
        }

        public void CompleteWork(string workItemId)
        {
            CompleteWork(WorkAgent.GetWorkItem(workItemId, WorkAgent.processing));
        }

        public void AddWorkTasks(WorkItem workItem)
        {
            if (WorkAgent.started.Contains(workItem))
            {
                IList<String> tasklist = _planner.MakePlan(workItem).Tasks; // HERE is where the workitem tasks are EXTRACTED

                lock (Avatar.taskQueue)
                {
                    Avatar.taskQueue.AddFirst("COMPLETEWORK:" + workItem.TaskID);

                    for (int i = tasklist.Count - 1; i >= 0; i--)
                    {
                        Avatar.taskQueue.AddFirst(tasklist[i]);
                    }
                }
            }
        }

        public override void StopTaskIfStarted(WorkItem workItem)
        {
            if (WorkAgent.started.Contains(workItem))
            {
                lock (Avatar.taskQueue)
                {
                    if (WorkAgent.processing.Contains(workItem))
                    {
                        //We have a slight problem?
                        while (Avatar.taskQueue.First.Value != "COMPLETEWORK:" + workItem.TaskID && Avatar.taskQueue.Count > 0)
                        {
                            Avatar.taskQueue.RemoveFirst();
                        }
                        Avatar.taskQueue.RemoveFirst();
                    }
                    else
                    {
                        //remove the start item thingo
                        Stack<String> reverseTasks = new Stack<string>();
                        while (Avatar.taskQueue.First.Value != "STARTWORK:" + workItem.TaskID && Avatar.taskQueue.Count > 0)
                        {
                            reverseTasks.Push(Avatar.taskQueue.First.Value);
                            Avatar.taskQueue.RemoveFirst();
                        }
                        if (Avatar.taskQueue.Count > 0)
                        {
                            Avatar.taskQueue.RemoveFirst();
                        }
                        while (reverseTasks.Count > 0)
                        {
                            Avatar.taskQueue.AddFirst(reverseTasks.Pop());
                        }
                    }
                }
            }
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

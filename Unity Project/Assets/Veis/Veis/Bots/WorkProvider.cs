using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;
using Veis.Planning;

namespace Veis.Bots
{
    public class WorkProvider : IWorkEnactor
    {
        private readonly BotAvatar _avatar;
        private readonly WorkflowProvider _workflow;
        private readonly WorkAgent _workAgent;
        private readonly Planner<WorkItem> _planner;

        public WorkProvider(BotAvatar avatar, WorkflowProvider provider, WorkAgent workAgent, Planner<WorkItem> planner)
        {
            _avatar = avatar;
            _workflow = provider;
            _workAgent = workAgent;
            _planner = planner;
        }
        
        public void AddWork(WorkItem workItem)
        {
            lock (_workAgent.started)
            {
                _avatar.AddTaskToQueue("STARTWORK:" + workItem.taskID);
            }
        }

        public void StartWork(WorkItem workItem)
        {
            lock (_workAgent.processing)
            {
                _workAgent.processing.Add(workItem);
            }
            AddWorkTasks(workItem);
        }

        public void StartWork(string workItemId)
        {
            StartWork(_workAgent.GetWorkItem(workItemId, _workAgent.started));
        }
       
        public void CompleteWork(WorkItem workItem)
        {
            if (/*_workAgent.started.Contains(workItem) &&*/ _workAgent.processing.Contains(workItem))
            {
                _workAgent.Complete(workItem, _workflow);
            }
        }

        public void CompleteWork(string workItemId)
        {
            CompleteWork(_workAgent.GetWorkItem(workItemId, _workAgent.processing));
        }

        public void AddWorkTasks(WorkItem workItem)
        {
            if (_workAgent.started.Contains(workItem))
            {
                IList<String> tasklist = _planner.MakePlan(workItem).Tasks; // HERE is where the workitem tasks are EXTRACTED

                lock (_avatar.taskQueue)
                {
                    _avatar.taskQueue.AddFirst("COMPLETEWORK:" + workItem.taskID);

                    for (int i = tasklist.Count - 1; i >= 0; i--)
                    {
                        _avatar.taskQueue.AddFirst(tasklist[i]);
                    }
                }
            }
        }

        public void StopTaskIfStarted(WorkItem workItem)
        {
            if (_workAgent.started.Contains(workItem))
            {
                lock (_avatar.taskQueue)
                {
                    if (_workAgent.processing.Contains(workItem))
                    {
                        //We have a slight problem?
                        while (_avatar.taskQueue.First.Value != "COMPLETEWORK:" + workItem.taskID && _avatar.taskQueue.Count > 0)
                        {
                            _avatar.taskQueue.RemoveFirst();
                        }
                        _avatar.taskQueue.RemoveFirst();
                    }
                    else
                    {
                        //remove the start item thingo
                        Stack<String> reverseTasks = new Stack<string>();
                        while (_avatar.taskQueue.First.Value != "STARTWORK:" + workItem.taskID && _avatar.taskQueue.Count > 0)
                        {
                            reverseTasks.Push(_avatar.taskQueue.First.Value);
                            _avatar.taskQueue.RemoveFirst();
                        }
                        if (_avatar.taskQueue.Count > 0)
                        {
                            _avatar.taskQueue.RemoveFirst();
                        }
                        while (reverseTasks.Count > 0)
                        {
                            _avatar.taskQueue.AddFirst(reverseTasks.Pop());
                        }
                    }
                }
            }
        }

        public WorkAgent GetWorkAgent()
        {
            return _workAgent;
        }

        public WorkflowProvider GetWorkflowProvider()
        {
            return _workflow;
        }
    }
}

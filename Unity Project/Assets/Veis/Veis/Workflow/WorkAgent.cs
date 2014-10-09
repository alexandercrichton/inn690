using Veis.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Workflow
{
    public abstract class WorkAgent
    {
        // WorkQueue constants
        public const string OFFERED = "0";
        public const string ALLOCATED = "1";
        public const string STARTED = "2";
        public const string SUSPENDED = "3";
        public const string COMPLETED = "4";    // non-standard workflow
        public const string DELEGATED = "5";    // non-standard workflow
        public const string PROCESSING = "6";   // non-standard workflow

        // WorkQueues, to match above
        public ThreadSafeList<WorkItem> offered;      //Tasks offered to potentially a variety of agents
        public ThreadSafeList<WorkItem> allocated;    //Tasks this agent has been assigned to complete
        public ThreadSafeList<WorkItem> started;      //Tasks this agent has started but have not yet completed
        public ThreadSafeList<WorkItem> suspended;    //Tasks this agent is assigned to; started; but have suspended them for the time being
               
        public ThreadSafeList<WorkItem> completed;    //Tasks this agent has completed
        public ThreadSafeList<WorkItem> delegated;    //Tasks this agent were assigned to; but delegated to others
        public ThreadSafeList<WorkItem> processing;   //Tasks this agent is running right this second in the task queue

        public string AgentID { get; set; }   // TODO: Sort out the descrepancy between agentid and yawlid
        public string FirstName { get; set; }   // Workers have an identifying name
        public string LastName { get; set; }    
        public string Appearance { get; set; }  // Workers have an appearance which usually correspond with their role
        public List<string> Roles { get; set; } // Workers have a set of roles they are in
        public List<string> Capabilities { get; set; }  // Workers have a set of things they are capable of

        protected WorkAgent()
        {
            Roles = new List<string>();
            Capabilities = new List<string>();
            FirstName = string.Empty;
            LastName = string.Empty;
            offered = new ThreadSafeList<WorkItem>();
            allocated = new ThreadSafeList<WorkItem>();
            started = new ThreadSafeList<WorkItem>();
            suspended = new ThreadSafeList<WorkItem>();
            completed = new ThreadSafeList<WorkItem>();
            delegated = new ThreadSafeList<WorkItem>();
            processing = new ThreadSafeList<WorkItem>();
        }

        public void AddRole(String potentialNewRole)
        {
            if (!Roles.Contains(potentialNewRole))
            {
                Roles.Add(potentialNewRole);
            }
        }

        public void AddCapability(String potentialNewCapability)
        {
            if (!Capabilities.Contains(potentialNewCapability))
            {
                Capabilities.Add(potentialNewCapability);
            }
        }

        public WorkItem GetWorkItem(string taskID, IList<WorkItem> queue)
        {
            foreach (WorkItem workItem in queue)
            {
                if (workItem.TaskID == taskID)
                {
                    return workItem;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public IList<WorkItem> GetQueueById(string id)
        {
            id = int.Parse(id).ToString();

            if (id == OFFERED)
            {
                return offered;
            }
            else if (id == ALLOCATED)
            {
                return allocated;
            }
            else if (id == STARTED)
            {
                return started;
            }
            else if (id == SUSPENDED)
            {
                return suspended;
            }
            else if (id == COMPLETED)
            {
                return completed;
            }
            else if (id == DELEGATED)
            {
                return delegated;
            }
            else if (id == PROCESSING)
            {
                return processing;
            }

            throw new Exception("No queue exists for that id.");
        }

        public void AddToQueue(string queueID, WorkItem item)
        {
            AddToQueue(GetQueueById(queueID), item);
        }

        public void AddToQueue(IList<WorkItem> queue, WorkItem item)
        {
            queue.Add(item);
        }

        public abstract void Complete(WorkItem workItem, WorkflowProvider provider);

        public abstract void Delegate(WorkItem item, WorkAgent other, WorkflowProvider provider);

        public abstract void Accept(WorkItem item, WorkflowProvider provider);

        public abstract void Suspend(WorkItem item, WorkflowProvider provider);

        public abstract void Unsuspend(WorkItem item, WorkflowProvider provider);
    }
}

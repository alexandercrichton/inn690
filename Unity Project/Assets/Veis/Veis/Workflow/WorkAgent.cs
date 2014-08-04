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
        public List<WorkItem> offered;      //Tasks offered to potentially a variety of agents
        public List<WorkItem> allocated;    //Tasks this agent has been assigned to complete
        public List<WorkItem> started;      //Tasks this agent has started but have not yet completed
        public List<WorkItem> suspended;    //Tasks this agent is assigned to; started; but have suspended them for the time being

        public List<WorkItem> completed;    //Tasks this agent has completed
        public List<WorkItem> delegated;    //Tasks this agent were assigned to; but delegated to others
        public List<WorkItem> processing;   //Tasks this agent is running right this second in the task queue

        public string AgentId { get; set; }   // TODO: Sort out the descrepancy between agentid and yawlid
        public string FirstName { get; set; }   // Workers have an identifying name
        public string LastName { get; set; }    
        public string Appearance { get; set; }  // Workers have an appearance which usually correspond with their role
        public List<String> Roles { get; set; } // Workers have a set of roles they are in
        public List<String> Capabilities { get; set; }  // Workers have a set of things they are capable of

        protected WorkAgent()
        {
            Roles = new List<string>();
            Capabilities = new List<string>();
            FirstName = string.Empty;
            LastName = string.Empty;
            offered = new List<WorkItem>();
            allocated = new List<WorkItem>();
            started = new List<WorkItem>();
            suspended = new List<WorkItem>();
            completed = new List<WorkItem>();
            delegated = new List<WorkItem>();
            processing = new List<WorkItem>();
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

        public WorkItem GetWorkItem(string id, List<WorkItem> queue)
        {
            foreach (WorkItem ifC in queue)
            {
                if (ifC.taskID == id)
                {
                    return ifC;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public List<WorkItem> GetQueueById(string id)
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

        public void AddToQueue(string queueID, WorkItem potentialNewAllocatedItem)
        {
            AddToQueue(GetQueueById(queueID), potentialNewAllocatedItem);
        }

        public void AddToQueue(List<WorkItem> queue, WorkItem potentialNewAllocatedItem)
        {
            IComparer<WorkItem> kkk = new WorkItemComparer();
            if (queue.BinarySearch(potentialNewAllocatedItem, kkk) < 0)
            {
                queue.Add(potentialNewAllocatedItem);
                queue.Sort(kkk);
            }
        }

        public abstract void Complete(WorkItem workItem, WorkflowProvider provider);

        public abstract void Delegate(WorkItem item, WorkAgent other, WorkflowProvider provider);

        public abstract void Accept(WorkItem item, WorkflowProvider provider);

        public abstract void Suspend(WorkItem item, WorkflowProvider provider);

        public abstract void Unsuspend(WorkItem item, WorkflowProvider provider);
    }
}

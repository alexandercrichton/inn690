using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Workflow.YAWL
{
    /// <summary>
    /// This class provides workitem tracking for a single human resource.
    /// It provides some utility methods to notify a workflow provider
    /// of the actions taken on the work items.
    /// Author: Paul Fox
    /// </summary>
    public class YAWLWorkAgent : WorkAgent
    {
        public override void Delegate(WorkItem item, WorkAgent other, WorkflowProvider provider)
        {
            delegated.Add(item);
            provider.Send("WorkItemAction Delegate " + this.AgentID + " " + item.TaskID + " " + other.AgentID);
        }

        public override void Accept(WorkItem item, WorkflowProvider provider)
        {
            provider.Send("WorkItemAction Accept " + this.AgentID + " " + item.TaskID);
        }

        public override void Suspend(WorkItem item, WorkflowProvider provider)
        {
            provider.Send("WorkItemAction Suspend " + this.AgentID + " " + item.TaskID);
        }

        public override void Unsuspend(WorkItem item, WorkflowProvider provider)
        {
            provider.Send("WorkItemAction Unsuspend " + this.AgentID + " " + item.TaskID);
        }      

        public override void Complete(WorkItem workItem, WorkflowProvider provider)
        {
            completed.Add(workItem);
            started.Remove(workItem);
            processing.Remove(workItem);
            provider.EndWorkItem(this, workItem);
        }
        
    }
}

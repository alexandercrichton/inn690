using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Bots;

namespace Veis.Workflow
{
    public abstract class WorkEnactor
    {
        public Avatar Avatar;
        public WorkflowProvider WorkflowProvider;
        public WorkAgent WorkAgent;

        public abstract void AddWorkItem(WorkItem workItem);
        public abstract void StopWorkItem(WorkItem work);
        public abstract void CompleteWorkItem(WorkItem workItem);
        public abstract void StartWorkItem(WorkItem workItem);
    }
}

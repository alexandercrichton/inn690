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

        public abstract void AddWork(WorkItem workItem);
        public abstract void StopTaskIfStarted(WorkItem work);
        public abstract void CompleteWork(WorkItem workItem);
        public abstract void StartWork(WorkItem workItem);
    }
}

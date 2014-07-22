using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Workflow
{
    public interface IWorkEnactor
    {
        void AddWork(WorkItem workItem);
        void StopTaskIfStarted(WorkItem work);
        void CompleteWork(WorkItem workItem);
        void StartWork(WorkItem workItem);
    }
}

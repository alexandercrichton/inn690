using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Workflow
{
    public class WorkItem
    {
        public string CaseID = "";
        public string SpecificationID = "";
        public string UniqueID = "";
        public string WorkItemID = "";
        public string TaskID = "";
        public string TaskName = "";
        public string AgentID = "";
        public string TaskQueue = "0";

        // Task variables are a list of various task variables that can be 
        // transferred from the process model specification
        // eg. Goals, Tasks, etc...
        public Dictionary<string, string> tasksAndGoals;

        public WorkItem()
        {
            tasksAndGoals = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            return TaskID;
        }
    }

    public class WorkItemComparer : IComparer<WorkItem>
    {
        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer<WorkItem>.Compare(WorkItem x, WorkItem y)
        {
            if (y is WorkItem && x is WorkItem)
            {
                return ((new CaseInsensitiveComparer()).Compare((y as WorkItem).TaskID, (x as WorkItem).TaskID));
            }
            else
            {
                return 0;
            }
        }
    }
}

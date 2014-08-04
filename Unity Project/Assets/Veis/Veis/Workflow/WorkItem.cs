using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Workflow
{
    public class WorkItem
    {
        public String taskName = "";
        public String taskID = "";
        public String agentID = "";
        public String taskQueue = "0";

        // Task variables are a list of various task variables that can be 
        // transferred from the process model specification
        // eg. Goals, Tasks, etc...
        public Dictionary<string, string> taskVariables;

        public WorkItem()
        {
            taskVariables = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            return taskID;
        }
    }

    public class WorkItemComparer : IComparer<WorkItem>
    {
        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer<WorkItem>.Compare(WorkItem x, WorkItem y)
        {
            if (y is WorkItem && x is WorkItem)
            {
                return ((new CaseInsensitiveComparer()).Compare((y as WorkItem).taskID, (x as WorkItem).taskID));
            }
            else
            {
                return 0;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Planning
{
    /// <summary>
    /// The most basic plan just involves a list of formatted strings that
    /// represent the tasks that need to be completed in chronologicial order.
    /// </summary>
    public class PlanResult
    {
        // Steps to complete said plan
        public List<string> Tasks { get; set; }

        public PlanResult()
        {
            Tasks = new List<string>();
        }        
    }
}

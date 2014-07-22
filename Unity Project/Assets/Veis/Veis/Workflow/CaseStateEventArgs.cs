using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Workflow
{
    public enum CaseState
    {
        STARTED,
        COMPLETED,
        CANCELLED
    }

    public class CaseStateEventArgs
    {
        public CaseState State { get; set; }
        public String CaseID { get; set; }
        public String SpecificationID { get; set; }
    }
}

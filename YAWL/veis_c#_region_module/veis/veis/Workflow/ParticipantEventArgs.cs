using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Workflow
{
    public class ParticipantEventArgs
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid Id { get; set; }
        public WorkAgent WorkAgent { get; set; }
        public WorkflowProvider WorkflowProvider { get; set; }
    }
}

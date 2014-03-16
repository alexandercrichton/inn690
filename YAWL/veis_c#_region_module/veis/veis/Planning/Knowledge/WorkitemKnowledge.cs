using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Planning.HTN;

namespace Veis.Planning.Knowledge
{
    public class WorkitemKnowledge
    {
        string _id;

        public List<string> StateList { get; set; }
        public List<string> TaskNetworkList { get; set; }
        public List<string> MethodList { get; set; }
        public List<string> OperatorList { get; set; }
        public List<string> ResourceList { get; set; }

        public HTNState StateUniverse { get; set; }
        public HTNState TargetState { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Data.Entities
{
    public class MethodPrecondition
    {
        public string MethodName { get; set; }

        public string Predicate { get; set; }

        public string Variable { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Data.Entities
{
    public class ActivityMethod
    {
        public string Name { get; set; }

        public IList<MethodParameter> Parameters { get; set; }

        public IList<MethodPostcondition> Postconditions { get; set; }

        public IList<MethodPrecondition> Preconditions { get; set; }

        public ActivityMethod()
        {
            Parameters = new List<MethodParameter>();
            Postconditions = new List<MethodPostcondition>();
            Preconditions = new List<MethodPrecondition>();
        }
    }
}

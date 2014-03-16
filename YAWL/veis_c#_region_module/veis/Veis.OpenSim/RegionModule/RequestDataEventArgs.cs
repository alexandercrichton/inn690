using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.OpenSim.RegionModule
{
    public class RequestDataEventArgs
    {
        public string DataType { get; set; }
        public IDictionary<string, object> DataFilter { get; set; }
        public string Destination { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Bots
{
    public class ExecutableAction
    {
        public string AssetName { get; set; }
        public string MethodName { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
    }
}

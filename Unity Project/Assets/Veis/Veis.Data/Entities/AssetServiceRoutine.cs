using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Data.Entities
{
    public class AssetServiceRoutine
    {
        public int Id { get; set; }
        public int Priority { get; set; }
        public string AssetKey { get; set; }
        public string ServiceRoutine { get; set; }
    }
}

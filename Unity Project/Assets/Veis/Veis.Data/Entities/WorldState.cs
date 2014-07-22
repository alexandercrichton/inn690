using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Data.Entities
{
    public class WorldState
    {
        public int WorldKey { get; set; }
        public string AssetName { get; set; }
        public string PredicateLabel { get; set; }
        public string Value { get; set; }
    }
}

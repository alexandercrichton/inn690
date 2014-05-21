using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Simulation.WorldState
{
    public class State
    {
        public string Asset { get; set; }
        public string Predicate { get; set; }
        public object Value { get; set; } // Could be colour/position/bool etc...

        public string StateLiteral()
        {
            return String.Format("{0}:{1};{2}", Asset, Predicate, Value);
        }
    }

    public class StateComparer : IEqualityComparer<State>
    {
        public bool Equals(State x, State y)
        {
            return x.StateLiteral().Equals(y.StateLiteral(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(State obj)
        {
            return obj.GetHashCode();
        }
    }
}

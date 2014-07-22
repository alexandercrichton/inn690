using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Simulation.WorldState
{
    public delegate void StateUpdatedHandler();
    
    public interface IStateSource
    {
        List<State> GetAll();

        List<State> Get(Func<State, bool> selector);

        event StateUpdatedHandler StateUpdated;
    }
}

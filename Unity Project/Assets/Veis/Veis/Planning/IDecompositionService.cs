using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Simulation.WorldState;

namespace Veis.Planning
{
    public interface IDecompositionService<T>
    {
        List<Goal> Decompose(T input); 
    }
}

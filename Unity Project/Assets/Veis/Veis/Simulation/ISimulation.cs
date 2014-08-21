using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Simulation
{
    /// <summary>
    /// At its most basic level, a simulation can be Initialise, Run, Reset, and Stopped.
    /// A number of typical actions can be performed on the simulation in order to do so.
    /// </summary>
    public interface ISimulation
    {
        void Start();
        void End();
        void Initialise();
        void Reset();
        void PerformSimulationAction(SimulationActions action);
    }
}

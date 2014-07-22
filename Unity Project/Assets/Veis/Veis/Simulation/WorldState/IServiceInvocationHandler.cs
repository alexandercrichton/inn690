using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Entities;

namespace Veis.Simulation.WorldState
{
    public interface IServiceInvocationHandler
    {
        Boolean CanHandle(string serviceRoutine);
        Boolean Handle(AssetServiceRoutine assetServiceRoutine);
        void Finalise();
    }
}

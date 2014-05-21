using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Simulation.WorldState.ServiceInvocationHandlers
{
    public class AbortHandler : IServiceInvocationHandler
    {
        public bool CanHandle(string serviceRoutine)
        {
            return serviceRoutine.Equals("abort", StringComparison.OrdinalIgnoreCase);
        }

        public bool Handle(Data.Entities.AssetServiceRoutine assetServiceRoutine)
        {
            return true;
        }

        public void Finalise()
        {
        }
    }
}

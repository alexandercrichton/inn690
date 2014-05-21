using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Repositories;

namespace Veis.Simulation.WorldState
{
    public class ServiceRoutineService
    {
        private const int WORLD_KEY = 1; // TODO: Add this to configuration or global state
        
        private List<IServiceInvocationHandler> _serviceHandlers;
        private WorldStateService _worldState;
        private AssetServiceRoutineRepository _assetServiceRoutines;
        
        // When world state is updated, checks if any service invocations need to be completed.
        // Checks if any of the registered handlers can take responsibilty for the service invocation
        public ServiceRoutineService(WorldStateService worldState,
            AssetServiceRoutineRepository assetServiceRoutines)
        {
            _worldState = worldState;
            _assetServiceRoutines = assetServiceRoutines;
            _worldState.WorldStateUpdated += HandleServiceInvocation;
            _serviceHandlers = new List<IServiceInvocationHandler>();
        }

        public void HandleServiceInvocation()
        {
            var serviceInvocations = _assetServiceRoutines.Find(
                new AssetServiceRoutineRepository.ByWorldSpecification(WORLD_KEY))
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.Id);

            foreach (var serviceInvocation in serviceInvocations)
            {
                Boolean handled = true;
                foreach (var handler in _serviceHandlers)
                {
                    if (handler.CanHandle(serviceInvocation.ServiceRoutine)) {
                        bool result = handler.Handle(serviceInvocation);
                        handled &= result;
                    }
                }
                if (handled) // Because it has now been handled, it does not need to remain in the queue
                    _assetServiceRoutines.Delete(serviceInvocation);
            }
        }

        public void AddServiceInvocationHandler(IServiceInvocationHandler handler)
        {
            _serviceHandlers.Add(handler);
        }

        public void RemoveServiceInvocationHandler(IServiceInvocationHandler handler)
        {
            handler.Finalise();
            _serviceHandlers.Remove(handler);
        }

        public void ClearServiceInvocationHandlers()
        {
            _serviceHandlers.ForEach(s => s.Finalise());
            _serviceHandlers.Clear();
        }

        public void Finalise()
        {
            _worldState = null;
            _worldState.WorldStateUpdated -= HandleServiceInvocation;
            _assetServiceRoutines = null;
            ClearServiceInvocationHandlers();          
        }

    }
}

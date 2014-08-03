using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Simulation.WorldState
{
    public delegate void WorldStateUpdatedHandler();
    
    /// <summary>
    /// Interface for management of states in a given world.
    /// States may be sourced from different areas (e.g. databases)
    /// When a state changes, the world might need to be updated in some way.
    /// For this reason, a WorldStateUpdated event will be fired.
    /// </summary>
    public class WorldStateService
    {
        public event WorldStateUpdatedHandler WorldStateUpdated;

        private List<IStateSource> _stateSources;
        //private readonly StateComparer _stateComparer; unused?

        public WorldStateService()
        {
            _stateSources = new List<IStateSource>();
            //_stateComparer = new StateComparer();
        }

        #region State Source Functions

        public void AddStateSource(IStateSource stateSource)
        {
            _stateSources.Add(stateSource);
            stateSource.StateUpdated += UpdateWorldState;
        }

        public void RemoveStateSource(IStateSource stateSource)
        {
            _stateSources.Remove(stateSource);
            stateSource.StateUpdated -= UpdateWorldState;
        }

        public void ClearStateSources()
        {
            _stateSources.ForEach(ss => ss.StateUpdated -= UpdateWorldState);
            _stateSources.Clear();
        }

        #endregion

        #region State Update & Retrieval

        public List<State> GetAll()
        {
            return _stateSources.SelectMany(
                ss => ss.GetAll()).ToList();
        }

        public void UpdateWorldState()
        {
            if (WorldStateUpdated != null)
                WorldStateUpdated();
        }

        public List<State> GetStates(string assetName)
        {
            return _stateSources.SelectMany(ss => ss.Get(s => s.Asset == assetName)).ToList();    
        }

        #endregion

        
    }
}

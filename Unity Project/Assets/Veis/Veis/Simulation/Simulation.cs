using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Data.Common;

using Veis.Chat;
using Veis.Bots;
using Veis.Workflow.YAWL;
using Veis.Workflow;

using Veis.Common;
using Veis.Planning;
using Veis.Services;
using Veis.Services.Interfaces;

using Veis.Simulation.WorldState;
using Veis.Simulation.WorldState.StateSources;
using Veis.Simulation.WorldState.ServiceInvocationHandlers;

using Veis.Data;
using Veis.Data.Repositories;
using Veis.Simulation.AvatarManagement;
using Veis.Data.Services;


namespace Veis.Simulation
{
    /// <summary>
    /// The simulation is the entry point to the application,
    /// and as such, needs to set up all the instance objects required
    /// Our simulation needs to:
    ///     Manage cases
    ///     Manages NPCs and Humans taking part in the cases
    /// </summary>
    public abstract class Simulation : ISimulation, ILoggable
    {
        // Avatar fields
        protected AvatarManager _avatarManager;

        // Case fields
        protected bool _isRunningCase;    // If a case is already running, another cannot be launched (to combat complexity at this stage)
        protected bool _isInitialised;

        // World state fields
        protected WorldStateService _worldStateService;
        protected SimpleWorkItemDecomposition _workItemDecomp;
        protected GoalService _goalService;
        protected ISceneService _sceneService;
        protected ServiceRoutineService _serviceRoutineService;
        protected AbortHandler _abortHandler;

        // Database access services
        protected IDataAccess _stateDataAccess;
        protected IDataAccess _knowledgeDataAccess;
        protected PolledDatabaseStateSource _polledWorldState;
        protected DbProviderFactory _databaseProvider;
        protected WorldStateRepository _worldStateRepos;
        protected AccessRecordRepository _accessRecordRepos;
        protected AssetServiceRoutineRepository _assetServiceRepos;
        protected MethodRepository _methodRepository;
        protected IActivityMethodService _activityMethodService; 


        public Simulation()
        {
            _worldStateService = new WorldStateService();
            _workItemDecomp = new SimpleWorkItemDecomposition();
            _goalService = new GoalService(_worldStateService);

            // Set up database classes
            _databaseProvider = new MySql.Data.MySqlClient.MySqlClientFactory();
            _stateDataAccess = new WorldStateDataAccess(_databaseProvider);
            _knowledgeDataAccess = new WorldKnowledgeDataAccess(_databaseProvider);

            _worldStateRepos = new WorldStateRepository(_stateDataAccess);
            _accessRecordRepos = new AccessRecordRepository(_stateDataAccess);
            _assetServiceRepos = new AssetServiceRoutineRepository(_stateDataAccess);
            _methodRepository = new MethodRepository(_knowledgeDataAccess);

            _activityMethodService = new ActivityMethodService(_methodRepository);

            // Set up service invocation classes
            _serviceRoutineService = new ServiceRoutineService(_worldStateService, _assetServiceRepos);
            _abortHandler = new AbortHandler();
            _serviceRoutineService.AddServiceInvocationHandler(_abortHandler);
        }
        
        public abstract void Run();
        public abstract void End();
        public abstract void Initialise();
        public abstract void Log(string message);
        public abstract void ResetAll();
        public abstract bool RequestLaunchCase(string specificationName);
        public abstract bool RequestCancelCase(string specificationName, int? caseNumber);
        public abstract void RequestCancelAllCases();
        public abstract void RegisterUser(UserArgs user);
        public abstract Avatar GetParticipantById(string id);

        public void PerformSimulationAction(SimulationActions action)
        {
            switch (action)
            {
                case SimulationActions.Reset:
                    ResetAll();
                    _isInitialised = false;
                    break;
                case SimulationActions.Start:
                    Initialise();
                    _isInitialised = true;
                    Run();
                    break;
            }
        }

        public bool IsInitialised()
        {
            return _isInitialised;
        }

        public bool IsCaseRunning()
        {
            return _isRunningCase;
        }
  
    }
}

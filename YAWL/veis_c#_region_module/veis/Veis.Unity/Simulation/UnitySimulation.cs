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

using Veis.Simulation;
using Veis.Simulation.WorldState;
using Veis.Simulation.WorldState.StateSources;
using Veis.Simulation.WorldState.ServiceInvocationHandlers;

using Veis.Data;
using Veis.Data.Repositories;
using Veis.Simulation.AvatarManagement;
using Veis.Data.Services;
using Veis.Unity.Bots;

namespace Veis.Unity.Simulation
{
    public class UnitySimulation : Veis.Simulation.Simulation
    {
        private YAWLWorkflowProvider workflowProvider;
        protected List<UnityHumanAvatar> humans;
        //protected List<UnityNPCAvatar> npcs;

        public UnitySimulation() : base()
        {
            workflowProvider = new YAWLWorkflowProvider();
            humans = new List<UnityHumanAvatar>();
            //npcs = new List<UnityNPCAvatar>();

            Initialise();
        }

        public override void Run() { }

        public override void End() { }
        
        public override void Initialise() 
        {
            if (!workflowProvider.IsConnected)
            {
                Log("Trying to connect to workflow provider");
                bool connected = workflowProvider.Connect();
                if (connected) // Hook up events
                {
                    //_workflowProvider.ParticipantCreated += CreateParticipant;
                    //_workflowProvider.CaseCompleted += CompleteCase;
                    //_workflowProvider.CaseStarted += StartCase;
                    //_webInterfaceModule.Initialise(_workflowProvider);
                }
                else // Mark as un-initialised
                {
                    // _workflowProvider = null;
                    Log("[YAWLSimulation]: Cannot connect to YAWL Service");
                }
            }

            if (_polledWorldState == null)
            {
                _polledWorldState = new PolledDatabaseStateSource(2000, _worldStateRepos, _accessRecordRepos);
                _worldStateService.AddStateSource(_polledWorldState);
                _polledWorldState.Start(); // TODO: put this back in StartCase
            }

            if (workflowProvider != null && workflowProvider.IsConnected)
            {
                workflowProvider.SyncAll();
            }
        }
        
        public override void Log(string message) { }

        public override void ResetAll() { }

        public override bool RequestLaunchCase(string specificationName) 
        {
            return true;
        }

        public override bool RequestCancelCase(string specificationName, int? caseNumber) 
        {
            return true;
        }
       
        public override void RequestCancelAllCases() { }

        public override void RegisterUser(UserArgs user) { }

        public override Avatar GetParticipantById(string id) 
        {
            return new Avatar();
        }

        /// <summary>
        /// This is just a redirect to this simulation's workflowprovider Send method
        /// so the user can send messages from the console window.
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            workflowProvider.Send(message);
        }
    }
}

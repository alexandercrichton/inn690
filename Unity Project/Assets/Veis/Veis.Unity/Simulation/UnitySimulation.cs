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
using Veis.Unity.Logging;
using Veis.Unity.Scene;
using UnityEngine;

namespace Veis.Unity.Simulation
{
    public class UnitySimulation : Veis.Simulation.Simulation
    {
        public YAWLWorkflowProvider _workflowProvider;
        public List<UnityHumanAvatar> _humans;
        public List<UnityNPCAvatar> _npcs;
        public SceneService _sceneService;
        public Planner<WorkItem> _npcWorkPlanner;

        public UnitySimulation()
        {
            _workflowProvider = new YAWLWorkflowProvider();
            _humans = new List<UnityHumanAvatar>();
            _npcs = new List<UnityNPCAvatar>();
            _sceneService = new SceneService();
            _npcWorkPlanner = new SmartWorkItemPlanner(
                new BasicWorkItemPlanner(_sceneService),
                new GoalBasedWorkItemPlanner(_workItemDecomp, _activityMethodService, _worldStateRepos, _sceneService));
            _serviceRoutineService.AddServiceInvocationHandler(new MoveObjectHandler(_sceneService));
            Initialise();
            _sceneService.PlaceObjectAt("s", "s");
        }

        #region Simulation Actions

        public override void Initialise()
        {
            if (!_workflowProvider.IsConnected)
            {
                Log("Trying to connect to workflow provider");
                bool connected = _workflowProvider.Connect();
                if (connected) // Hook up events
                {
                    Log("Connected to workflow provider");
                    _workflowProvider.ParticipantCreated += CreateParticipant;
                    _workflowProvider.CaseCompleted += CompleteCase;
                    _workflowProvider.CaseStarted += StartCase;
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
                _worldStateService.WorldStateUpdated += OnWorldStateUpdated;
            }

            if (_workflowProvider != null && _workflowProvider.IsConnected)
            {
                _workflowProvider.SyncAll();
            }
        }

        public override void Run() { }

        public override void ResetAll()
        {
            Log("Resetting simulation state");
            _npcs.Clear();
            _humans.Clear();
            if (_workflowProvider != null && _workflowProvider.IsConnected)
            {
                _workflowProvider.ResetAll();
                _workflowProvider.AllWorkItems.Clear();
                _workflowProvider.AllParticipants.Clear();
            }
            if (_polledWorldState != null)
            {
                _polledWorldState.Stop();
                _polledWorldState = null;
                _worldStateService.ClearStateSources();
                _goalService.ClearGoals();
            }
        }

        public override void End() 
        {
            Log("\nLogging out all bots...");
            _npcs.Clear();
            _humans.Clear();

            Log("\nClosing connection to YAWL...");
            _workflowProvider.Close();
        }

        #endregion

        #region Process Script Commands

        public void ProcessScriptCommand(string command)
        {
            string[] tokens = command.Split(new char[] { '|' }, StringSplitOptions.None);
            string function = tokens[0];
            switch (function)
            {

                case "LaunchCase":
                    //CRCMOD|LaunchCase|SpecificationName
                    string name = tokens[1];
                    if (RequestLaunchCase(name))
                    {
                        //_commsMod.DispatchReply(script, 1, "Launched case successfully. [" + name + "]", "");
                        Log("Launched case successfully. [" + name + "]");
                    }
                    else
                    {
                        //_commsMod.DispatchReply(script, 1, "Launch case failed. [" + name + "]", "");
                        Log("Launch case failed. [" + name + "]");
                    }
                    break;

                case "EndAllCases":
                    //CRCMOD|EndAllCases

                    RequestCancelAllCases();
                    //_commsMod.DispatchReply(script, 1, "Attempting to cancel all running cases", "");
                    break;

                case "Simulation": // Performs the given simulation action on listening simulators
                    if (tokens.Length > 1)
                    {
                        if (tokens[1] == "Reset")
                        {
                            PerformSimulationAction(SimulationActions.Reset);
                            //_commsMod.DispatchReply(script, 1, "Attempting to reset simulation", "");
                        }
                        else if (tokens[1] == "Start")
                        {
                            PerformSimulationAction(SimulationActions.Start);
                            //_commsMod.DispatchReply(script, 1, "Initialising simulation", "");
                        }
                    }
                    else
                    {
                        //_commsMod.DispatchReply(script, 1, "Missing argument for Simulation modCommand", "");
                    }
                    break;

                case "GetAll": // Gets all information of a given type string
                    if (tokens.Length > 1)
                    {
                        //GetDataRequested(new RequestDataEventArgs { DataType = tokens[1], DataFilter = new Dictionary<string, object>(), Destination = script.ToString() });
                    }
                    break;

                case "Get": // Gets specific information of a given type, with a filter. Filter string is of format <Field>:<Value>+<Field>:<Value>+ ..etc
                    if (tokens.Length > 2)
                    {
                        var dataFilter = new Dictionary<string, object>();
                        string[] filterTokens = tokens[2].Split(new char[] { ':', '+' }, StringSplitOptions.None);
                        for (int i = 0; i < filterTokens.Length; i += 2)
                        {
                            dataFilter.Add(filterTokens[i], filterTokens[i + 1]);
                        }

                        //GetDataRequested(new RequestDataEventArgs { DataType = tokens[1], DataFilter = dataFilter, Destination = script.ToString() });
                    }
                    break;

                case "RegisterUser": // Involves the given user in the simulation
                    if (tokens.Length > 2)
                    {
                        RegisterUser(new UserArgs { 
                            UserName = tokens[1], RoleName = tokens[1], UserId = tokens[2] });
                    }
                    break;
            }
        }

        public event WorldStateUpdatedHandler WorldStateUpdated;
        protected void OnWorldStateUpdated()
        {
            if (WorldStateUpdated != null)
            {
                WorldStateUpdated();
            }
        }

        #endregion

        #region Case Actions

        void StartCase(object sender, CaseStateEventArgs e)
        {
            // A case has been successfully launched
            _isRunningCase = true;
            Log("Running case.");
            _workflowProvider.SyncAll();
            //_polledWorldState.Start();
            //_sceneService.ShowResponse("Make sense 123!", true);
        }

        void CompleteCase(object sender, CaseStateEventArgs e)
        {
            _isRunningCase = false;
            Log("Completed case");
            foreach (var human in _humans)
            {
                human.WorkProvider.ClearCompletedGoals();
            }
            //_polledWorldState.Stop();
            // _sceneService.ShowResponse("Make sense 456!", true);
        }


        /// <summary>
        /// Starts the simulation if a case launch request comes through.
        /// It cannot start another case if one is already running.
        /// </summary>
        /// <param name="specificationName"></param>
        public override bool RequestLaunchCase(string specificationName)
        {
            if (_isRunningCase) 
            { 
                Log("Already running case"); 
                return false; 
            }

            Initialise(); // Set up the simulation inititially

            if (_workflowProvider != null)
            {
                Log("Launching case");
                _workflowProvider.LaunchCase(specificationName);
            }

            return true;
        }

        /// <summary>
        /// Cancels any currently running cases and resets the simulation state
        /// </summary>
        /// <param name="specificationName"></param>
        /// <param name="caseNumber"></param>
        public override bool RequestCancelCase(string specificationName, int? caseNumber)
        {
            if (!_isRunningCase)
            {
                return false;
            }

            if (_workflowProvider != null && _workflowProvider.IsConnected)
            {
                Log("Cancelling case");
                _workflowProvider.EndAllCases(); // TODO Cancel a specific case
                //_npcModule.RemoveAll();
                _isRunningCase = false;
            }
            else
            {
                _isRunningCase = false;
            }

            if (_polledWorldState != null)
            {
                //_polledWorldState.Stop();
            }
            return true;
        }

        public override void RequestCancelAllCases()
        {
            if (!_isRunningCase)
            {
                return;
            }

            if (_workflowProvider != null && _workflowProvider.IsConnected)
            {
                Log("Cancelling cases");
                _workflowProvider.EndAllCases(); // TODO Cancel a specific case
                //_npcModule.RemoveAll();
                _isRunningCase = false;
            }
            else
            {
                _isRunningCase = false;
            }
        }


        #endregion

        #region Human and NPC Management

        public override Veis.Bots.Avatar GetParticipantById(string id)
        {
            UUID temp = new UUID();
            if (UUID.TryParse(id, out temp))
            {
                var human = _humans.Where(h => h.UUID.Equals(temp)).FirstOrDefault();
                if (human != null) return human;

                var npc = _npcs.Where(n => n.UUID.Equals(temp)).FirstOrDefault();
                if (npc != null) return npc;
            }
            return null;
        }

        public override void RegisterUser(UserArgs e)
        {
            Log("Registering user: " + e.UserName);
            // If there is no UserName, find it using UUID
            if (String.IsNullOrEmpty(e.UserName))
            {
                Log("User name is empty");
                e.UserName = _sceneService.GetUserNameById(e.UserId);
            }

            // Check if there already exists an npc/human matching this role name (or real name if role name is blank)
            var nameQuery = String.IsNullOrEmpty(e.RoleName) ? e.UserName : e.RoleName;

            var existingHuman = _humans.FirstOrDefault(h => h.Name == nameQuery || h.ActingName == nameQuery);
            if (existingHuman != null)
            {
                Log("Cannot register because this role has already been taken");
                return;
            }

            var oldNpc = _npcs.FirstOrDefault(n => String.Format("{0} {1}", n.FirstName, n.LastName) == nameQuery);
            string yawlId = _workflowProvider.GetAgentIdByFullName(nameQuery);

            // Create new OpenSimHumanAvatar
            UnityHumanAvatar human = new UnityHumanAvatar(UUID.Parse(e.UserId), e.UserName, e.RoleName);

            if (oldNpc != null)
            {
                // Replace work enactor for this participant with the new work provider (if npc != null)
                Log("Human: " + human.RoleName + ", " + human.WorkId + ", " + human.UUID.Guid.ToString() 
                    + " replacing NPC: " + oldNpc.FirstName);
                ReplaceNpc(oldNpc, human);
            }
            else if (!String.IsNullOrEmpty(yawlId))
            {
                // If this human can be a YAWL participant, add them to the workflow provider
                AddHuman(yawlId, human);
                Log("AddHuman");
            }
        }

        private void AddHuman(string yawlId, UnityHumanAvatar newHuman)
        {
            string workerId = _workflowProvider.YawlToWorker[yawlId];
            newHuman.WorkId = workerId;

            WorkAgent workAgent = _workflowProvider.AllParticipants[yawlId];
            HumanWorkProvider workProvider = new HumanWorkProvider(newHuman, workAgent, _workflowProvider,
                _workItemDecomp, _goalService);
            newHuman.WorkProvider = workProvider;

            _workflowProvider.AddWorker(workerId, workProvider);
            _humans.Add(newHuman);
        }

        private void ReplaceNpc(UnityNPCAvatar oldNpc, UnityHumanAvatar newHuman)
        {
            //Log(oldNpc.WorkProvider.GetWorkAgent().ToString());
            //foreach (var workItem in oldNpc.WorkProvider.GetWorkAgent().ToString())
            //{
            //    Log(workItem.ToString());
            //}
            
            HumanWorkProvider workProvider = NPCToHumanMapping.MapWorkProviderFromNPC(
                oldNpc, newHuman, _goalService, _workItemDecomp);
            newHuman.WorkProvider = workProvider;
            newHuman.WorkId = oldNpc.Id;
            bool successful = _workflowProvider.ReplaceWorker(newHuman.WorkId, workProvider);
            if (successful)
            {
                //_npcModule.RemoveNPC(oldNpc.UUID);
                _npcs.Remove(oldNpc);
                _humans.Add(newHuman);
            }
            else
            {
                Log("Could not replace NPC with this user");
            }
        }

        /// <summary>
        /// When the workflow engine creates a new workflow participant, the simulation
        /// should do the same. If a human is taking the part of the given participant,
        /// a different kind of bot is created. 
        /// </summary>
        /// <param name="e">Contains information about the newly created workflow participant</param>
        public void CreateParticipant(object sender, ParticipantEventArgs e)
        {
            // Make sure that the work agent and provider are compatible with this simulation
            if (!CanCreateParticipant(e)) return;

            // TODO: Retrieve a list of currently logged in agents and check if any match the participant descriptions

            // Check if any of the registered agents match the given participant

            CreateNPC(e);
        }

        private bool CanCreateParticipant(ParticipantEventArgs e)
        {
            if (e.WorkAgent.GetType() != typeof(YAWLAgent) ||
                 e.WorkflowProvider.GetType() != typeof(YAWLWorkflowProvider))
            {
                Log("This simulation requires YAWLAgents and a YAWLWorkflowProvider.");
                return false;
            }

            return true;
        }

        private bool CreateNPC(ParticipantEventArgs e)
        {
            // Create NPC via module
            //UUID newNPCUUID = _npcModule.CreateNPC(e.FirstName, e.LastName,
            //    _npcModule.GetDefaultStartingPosition(), _npcModule.GetAppearance(e.WorkAgent.Appearance));
            UUID newNPCUUID = new UUID(Guid.NewGuid());
            Log(newNPCUUID.ToString());

            // TODO Handle concurrent executions of workflows, so don't create new NPCs each time & check for existence of the given UUID
            if (newNPCUUID != UUID.Zero)
            {
                // Create new controlled NPC
                UnityNPCAvatar newNPC = new UnityNPCAvatar(newNPCUUID, e.FirstName, e.LastName, _sceneService);
                newNPC.Id = e.Id.ToString();


                // Set up chat handlers
                //OpenSimChatHandler openSimChat = new OpenSimChatHandler(newNPC, _npcModule.GetScene());
                //WorkflowChatHandler workflowChat = new WorkflowChatHandler(e.WorkAgent, e.WorkflowProvider);
                //YAWLChatHandler yawlChat = new YAWLChatHandler(e.WorkAgent as YAWLAgent, e.WorkflowProvider as YAWLWorkflowProvider);
                //newNPC.ChatHandle.AddChatHandler(openSimChat);
                //newNPC.ChatHandle.AddChatHandler(workflowChat);
                //newNPC.ChatHandle.AddChatHandler(yawlChat);

                // Set up work enactor and register it with the workflow provider
                WorkProvider workProvider = new WorkProvider(newNPC, e.WorkflowProvider, e.WorkAgent, _npcWorkPlanner);
                newNPC.WorkProvider = workProvider;
                (e.WorkflowProvider as YAWLWorkflowProvider).AddWorker(newNPC.Id, workProvider);

                //The last uuid string will be trigger by an on-scene object to change avatar apperance, do not remove it.
                //newNPC.Say(String.Format("Hello, I am {0} {1}, {2}, {3}", newNPC.FirstName, newNPC.LastName, newNPC.Appearance, newNPCUUID));

                _npcs.Add(newNPC);
                return true;
            }
            else
            {
                Log(String.Format("Could not create NPC for {0} {1}", e.FirstName, e.LastName));
                return false;
            }
        }

        #endregion

        #region Data Requests

        void GetDataRequested(RequestDataEventArgs e)
        {
            if (e.DataType.Equals("availablespecs", StringComparison.OrdinalIgnoreCase))
            {
                if (_workflowProvider != null)
                {
                    List<string> allSpecs = _workflowProvider.AllSpecifications.Select(s => s.Value).ToList();
                    /*for (int i = 0; i < allSpecs.Count; i++ )
                    {
                        Log(i + " " + allSpecs[i]);
                    }*/
                    //_npcModule.SendData(allSpecs, true, UUID.Parse(e.Destination));
                }
            }
            if (e.DataType.Equals("availableagents", StringComparison.OrdinalIgnoreCase))
            {
                if (_workflowProvider != null)
                {
                    List<string> allAgents = _workflowProvider.AllParticipants
                        .Select(p => p.Value.FirstName + " " + p.Value.LastName).ToList();

                    //npcModule.SendData(allAgents, true, UUID.Parse(e.Destination));
                }
            }
            if (e.DataType.Equals("methodsToExecute", StringComparison.OrdinalIgnoreCase))
            {
                // <Field>:<Value>+<Field>:<Value>+ ..etc
                // npc:<npc UUID>+asset:<asset Name>
                var npcUUID = e.DataFilter["npc"];
                var assetName = e.DataFilter["asset"];

                // get the npc
                var npc = _npcs.Where(n => (string)n.UUID.ToString() == (string)npcUUID).FirstOrDefault();
                if (npc == null) return;
                // Get the executable action that the npc wants to run
                var executeAction = npc.PopExecutableAction(assetName.ToString());
                if (executeAction == null) return;

                // user_key|<user_key>|method_name|<method_name>|<param name>|<param value|...      
                //_npcModule.SendData(
                //    new List<string> { StringFormattingExtensions.EncodeExecutableActionString(executeAction, npcUUID.ToString()) },
                //    false, UUID.Parse(e.Destination));
            }
        }

        #endregion

        #region Helper Methods

        public override void Log(string message)
        {
            UnityLogger.BroadcastMesage(this, message);
        }

        /// <summary>
        /// This is just a redirect to this simulation's workflowprovider Send method
        /// so the user can easily send messages by accessing the simulation
        /// rather than the workflow provider.
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            _workflowProvider.Send(message);
        }

        #endregion
    }
}

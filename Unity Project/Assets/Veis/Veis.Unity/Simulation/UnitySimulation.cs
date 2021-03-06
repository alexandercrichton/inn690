﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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
using Veis.Bots.AvatarManagement;
using Veis.Data.Services;
using Veis.Data.Entities;

using Veis.Unity.Bots;
using Veis.Unity.Logging;
using Veis.Unity.Scene;
using UnityEngine;

namespace Veis.Unity.Simulation
{
    public class UnitySimulation : Veis.Simulation.Simulation
    {
        // These should be made non-public at some stage
        public YAWLWorkflowProvider _workflowProvider;
        //public List<UnityHumanAvatar> _humans;
        //public List<UnityBotAvatar> _npcs;
        new public UnitySceneService _sceneService;
        public Planner<WorkItem> _npcWorkPlanner;

        public UnitySimulation()
        {
            _avatarManager = new AvatarManager();
            _workflowProvider = new YAWLWorkflowProvider();
            //_humans = new List<UnityHumanAvatar>();
            //_npcs = new List<UnityBotAvatar>();
            _sceneService = new UnitySceneService();
            _npcWorkPlanner = new SmartWorkItemPlanner(
                new BasicWorkItemPlanner(_sceneService),
                new GoalBasedWorkItemPlanner(_workItemDecomp, _activityMethodService, _worldStateRepos, _sceneService));
            _serviceRoutineService.AddServiceInvocationHandler(new MoveObjectHandler(_sceneService)); 
            _polledWorldState = new PolledDatabaseStateSource(2000, _worldStateRepos, _accessRecordRepos);
			_worldStateService.AddStateSource(_polledWorldState);
			_workflowProvider.Start();
            _workflowProvider.SyncAll();

            _workflowProvider.AgentCreated += CreateBotAvatar;
            _workflowProvider.CaseCompleted += CompleteCase;
            _workflowProvider.CaseStarted += StartCase;
            _worldStateService.WorldStateUpdated += OnWorldStateUpdated;
            _worldStateRepos.ResetAssetWorldStates();

            // Test user for development purposes
            AddUser(new Veis.Simulation.AgentEventArgs
            {
                Name = "Ross Brown",
                Role = "",
                ID = "123456789123456789123456789123456789"
            });
        }

        #region Simulation Actions

        public override void Reset()
        {
            Log("Resetting simulation state");
            _isRunningCase = false;
            _avatarManager.Clear();
            _workflowProvider.ResetAll();
            _polledWorldState.Stop();
            _goalService.ClearGoals();
            _sceneService.ResetAllAssetPositions();
            _worldStateRepos.ResetAssetWorldStates();
        }

        public override void Start()
        {
            RequestLaunchCase(_workflowProvider.AllCases[0].SpecificationName);
            _polledWorldState.Start();
        }

        public override void End() 
        {
            Log("\nLogging out all bots...");
            _avatarManager.Clear();

            Log("\nClosing connection to YAWL...");
            _workflowProvider.End();
            _polledWorldState.Stop();
        }

        public event WorldStateUpdatedHandler WorldStateUpdated;
        protected void OnWorldStateUpdated()
        {
            if (WorldStateUpdated != null)
            {
                WorldStateUpdated();
            }
        }

        /// <summary>
        /// This is the entry point for the Unity application's main thread.
        /// Anything that needs to be handled on the main thread will be
        /// called starting from here.
        /// </summary>
        public void UnityMainThreadUpdate()
        {
            _sceneService.HandleAssetServiceRoutines();
			foreach (UnityBotAvatar avatar in _avatarManager.Bots) {
                //Logging.UnityLogger.BroadcastMesage(this, avatar.taskQueue.Count.ToString());
                avatar.Update();
			}

            ThreadedActionHandler.DoActions();
        }

        public void OnUnityApplicationClose()
        {

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
                            Reset();
                            //_commsMod.DispatchReply(script, 1, "Attempting to reset simulation", "");
                        }
                        else if (tokens[1] == "Start")
                        {
                            Start();
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
                        AddUser(new AgentEventArgs { 
                            Name = tokens[1], Role = tokens[1], ID = tokens[2] });
                    }
                    break;
            }
        }



        #endregion

        #region Case Actions

        void StartCase(object sender, CaseStateEventArgs e)
        {
            // A case has been successfully launched
            _isRunningCase = true;
            Log("Running case.");
            //_workflowProvider.SyncAll();
            //_polledWorldState.Start();
            //_sceneService.ShowResponse("Make sense 123!", true);
        }

        void CompleteCase(object sender, CaseStateEventArgs e)
        {
            _isRunningCase = false;
            Log("Completed case");
            foreach (var human in _avatarManager.Humans)
            {
                human.WorkEnactor.ClearCompletedGoals();
            }
            //_polledWorldState.Stop();
            // _sceneService.ShowResponse("Make sense 456!", true);
        }


        /// <summary>
        /// Starts the simulation if a case launch request comes through.
        /// It cannot start another case if one is already running.
        /// </summary>
        /// <param name="uri"></param>
        public override bool RequestLaunchCase(string uri)
        {
            if (_isRunningCase) 
            { 
                Log("Already running case"); 
                return false; 
            }

            if (_workflowProvider != null)
            {
                Log("Launching case");
                _workflowProvider.LaunchCase(uri);
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
            var human = _avatarManager.Humans.FirstOrDefault(h => h.ID == id);
            if (human != null)
            {
                return human;
            }
            var bot = _avatarManager.Bots.FirstOrDefault(b => b.ID == id);
            if (bot != null) 
            {
                return bot;
            } 
            return null;
        }

        public override void AddUser(AgentEventArgs e)
        {
            Log("Adding user: " + e.Name);
            
            UnityHumanAvatar human = new UnityHumanAvatar(e.ID, e.Name);

            string agentID = WorkAgent.WORKFLOW_IGNORE_ID;
            WorkAgent workAgent = new YAWLWorkAgent { AgentID = agentID };
            HumanWorkEnactor workEnactor = new HumanWorkEnactor(human, workAgent, _workflowProvider,
                _workItemDecomp, _goalService);
            human.WorkEnactor = workEnactor;

            _workflowProvider.AddWorkEnactor(workEnactor);
            _avatarManager.Humans.Add(human);
        }

        private void ReplaceNpc(UnityBotAvatar oldNpc, UnityHumanAvatar newHuman)
        {
            //Log(oldNpc.WorkProvider.GetWorkAgent().ToString());
            //foreach (var workItem in oldNpc.WorkProvider.GetWorkAgent().ToString())
            //{
            //    Log(workItem.ToString());
            //}
            
            //HumanWorkEnactor workProvider = NPCToHumanMapping.MapWorkProviderFromNPC(
            //    oldNpc, newHuman, _goalService, _workItemDecomp);
            //newHuman.WorkEnactor = workProvider;
            //newHuman.WorkId = oldNpc.Id;
            //bool successful = _workflowProvider.ReplaceWorker(newHuman.WorkId, workProvider);
            //if (successful)
            //{
            //    //_npcModule.RemoveNPC(oldNpc.UUID);
            //    _npcs.Remove(oldNpc);
            //    _humans.Add(newHuman);
            //}
            //else
            //{
            //    Log("Could not replace NPC with this user");
            //}
        }

        /// <summary>
        /// When the workflow engine creates a new workflow participant, the simulation
        /// should do the same. If a human is taking the part of the given participant,
        /// a different kind of bot is created. 
        /// </summary>
        /// <param name="e">Contains information about the newly created workflow participant</param>
        public void CreateBotAvatar(object sender, AgentEventArgs e)
        {
            Log("Adding bot avatar: " + e.Name);

            if (!_avatarManager.Bots.Any(b => b.ID == e.ID))
            {
                UnityBotAvatar bot = new UnityBotAvatar(_activityMethodService, _worldStateRepos, _serviceRoutineService,
                    e.ID, e.Name, e.Role, _sceneService);

                string agentID = _workflowProvider.GetAgentIdByFullName(e.Name);
                WorkAgent workAgent = _workflowProvider.AllWorkAgents.FirstOrDefault(a => a.AgentID == agentID);
                BotWorkEnactor workEnactor = new BotWorkEnactor(bot, _workflowProvider, workAgent, _npcWorkPlanner);
                bot.WorkEnactor = workEnactor;

                _workflowProvider.AddWorkEnactor(workEnactor);
                _avatarManager.Bots.Add(bot);

				ThreadedActionHandler.QueueAction(()=> 
				{
				//TODO: instantiate bot stuff
					if (e.Name == "Edwin Fahel") {
					    GameObject botAvatar = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/docPrefab"), new Vector3(-2.0f, 0.0f, -10.0f), Quaternion.identity);
						bot.botAgentMovement = botAvatar.GetComponent<navAgent>();
                        bot.botAgentMovement.BotAvatar = bot;
						botAvatar.name = "Avatar " + e.Name;
						bot.SendBotValues();
					} else if (e.Name == "Janie May") {
						GameObject botAvatar = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/nursePrefab"), new Vector3(-4.0f, 0.0f, -10.0f), Quaternion.identity);
                        bot.botAgentMovement = botAvatar.GetComponent<navAgent>();
                        bot.botAgentMovement.BotAvatar = bot;
						botAvatar.name = "Avatar " + e.Name;
						bot.SendBotValues();
					}
				});
            }
            else
            {
                Log("That bot ID already exists");
            }
        }

        public void UserClickedOnAvatar(Veis.Bots.Avatar avatar)
        {
            _avatarManager.PossessBot(_avatarManager.Humans[0], (BotAvatar)avatar);
        }

        public void UserRelinquishedCurrentAvatar()
        {
            _avatarManager.RelinquishAnyBots(_avatarManager.Humans[0]);
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

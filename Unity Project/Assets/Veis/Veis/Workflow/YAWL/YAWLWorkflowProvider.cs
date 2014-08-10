﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using Veis.Simulation;
using Veis.Data.Logging;

namespace Veis.Workflow.YAWL
{
    public delegate void AgentCreatedEventHandler(object sender, AgentEventArgs e);
    public delegate void CaseStateEventHandler(object sender, CaseStateEventArgs e);
    
    /// <summary>
    /// This will be the only interface between the workflow logic on this side and
    /// the workflow logic on the WFMS side.
    /// </summary>
    public class YAWLWorkflowProvider : WorkflowProvider
    {
        // The purpose of this event is to allow a simulation program to
        // create bots or users or some THING to handle work items.
        public event AgentCreatedEventHandler AgentCreated;
        protected virtual void OnAgentCreated(AgentEventArgs e)
        {
            if (AgentCreated != null)
                AgentCreated(this, e);
        }

        // The purpose of these event is to allow the simulation program to
        // react to the creation of new cases, and the cancelling or completion of
        // running cases
        public event CaseStateEventHandler CaseCompleted;
        public event CaseStateEventHandler CaseStarted;
        public event CaseStateEventHandler CaseCancelled;
        protected virtual void OnCaseStateChanged(CaseStateEventArgs e)
        {
            if (e.State == CaseState.COMPLETED && CaseCompleted != null)
                CaseCompleted(this, e);
            else if (e.State == CaseState.STARTED && CaseStarted != null)
                CaseStarted(this, e);
            else if (e.State == CaseState.CANCELLED && CaseCancelled != null)
                CaseCancelled(this, e);
        }

        private const string WorkflowConnectionUrl = "127.0.0.1";
        private const int WorkflowConnectionPort = 4444;
        
        private static readonly UTF8Encoding Encoding = new UTF8Encoding();

        private List<WorkEnactor> WorkEnactors { get; set; } // Things that can enact workitems
        
        private Socket _externalProcessor;
        private Thread _oThread;

        public YAWLWorkflowProvider()
        {
            _externalProcessor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            WorkEnactors = new List<WorkEnactor>();
            _oThread = new Thread(ReadMessages);
        }

        public void ResetAll()
        {
            EndAllCases();
            WorkEnactors.Clear();
            StartedCases.Clear();
        }

        public void AddWorkEnactor(WorkEnactor workEnactor)
        {
            WorkEnactors.Add(workEnactor);
            GetTaskQueuesForWorkEnactor(workEnactor);
        }

        private void GetTaskQueuesForWorkEnactor(WorkEnactor workEnactor)
        {
            if (WorkEnactors.Contains(workEnactor)) 
            {
                string agentID = workEnactor.WorkAgent.AgentID;
                Send("GetTaskQueue " + WorkAgent.OFFERED + " " + agentID);
                Send("GetTaskQueue " + WorkAgent.ALLOCATED + " " + agentID);
                Send("GetTaskQueue " + WorkAgent.STARTED + " " + agentID);
                Send("GetTaskQueue " + WorkAgent.SUSPENDED + " " + agentID);
            }
        }

        public bool ReplaceWorker(String workerId, WorkEnactor newWorker)
        {
            // Merely replace the WorkEnactor for this 'worker'
            //if (WorkEnactors.Any(w => w.WorkAgent.AgentID == ))
            //{
            //    WorkEnactors[workerId] = newWorker;
            //    GetTaskQueuesForWorker(workerId);
            //    return true;
            //}
            return false;
        }

        public bool ReplaceWorker(String oldWorkerId, String newWorkerId, WorkEnactor newWorker)
        {
            // If old worker exists, replace its work enactor entry
            //if (WorkerToYawl.ContainsKey(oldWorkerId))
            //{
            //    string yawlId = WorkerToYawl[oldWorkerId];
            //    // Remove WorkerToYawl entry and add new entry
            //    WorkerToYawl.Remove(oldWorkerId);
            //    WorkerToYawl.Add(newWorkerId, yawlId);
            //    // Replace YawlToWorker entry with the newWorkerId
            //    YawlToWorker[yawlId] = newWorkerId;
            //    // Remove the work enactor for the old worker and add the new one
            //    WorkEnactors.Remove(oldWorkerId);
            //    AddWorkEnactor(newWorkerId, newWorker);
            //    return true;
            //}
            return false; 
        } 

        public override void EndWorkItem(WorkAgent agent, WorkItem workItem)
        {
            Send("WorkItemAction Complete " + agent.AgentID + " " + workItem.taskID);
            Thread.Sleep(1000);
            // Before requesting the next task, make sure that th
            if (_oThread.ThreadState == ThreadState.Stopped) 
                _oThread.Start(); 
            else if (_oThread.ThreadState == ThreadState.Suspended) 
                _oThread.Resume(); 
            else if (_oThread.ThreadState == ThreadState.WaitSleepJoin)              
                _oThread.Interrupt(); 


            Send("GetTaskQueue " + WorkAgent.STARTED + " " + agent.AgentID);
            Send("GetTaskQueue " + WorkAgent.OFFERED + " " + agent.AgentID);
        }

        public override void Send(string msg) {
            if (!_externalProcessor.Connected) return;
            byte[] bytes = Encoding.GetBytes(msg + "\n");
            _externalProcessor.Send(bytes);
            System.Diagnostics.Debug.WriteLine("SEND: " + msg);
            Logger.BroadcastMessage(this, "SEND: " + msg);
        }

        public override void Close() {
            EndAllCases();
            Send("EndSession");
            if (_externalProcessor.Connected)
            {
                _externalProcessor.Shutdown(SocketShutdown.Both);
                _externalProcessor.Disconnect(false);
                //_externalProcessor.Close();
                _externalProcessor = null;
            }
           
            if (_oThread.ThreadState == ThreadState.Running) _oThread.Join();
            IsConnected = false;
        }

        public override bool Connect()
        {
            try
            {
                _externalProcessor.Connect(WorkflowConnectionUrl, WorkflowConnectionPort);              
                _oThread.Start();
                Console.WriteLine("connected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to YAWL Workflow service: " + e.Message);
                IsConnected = false;
                return false;
            }

            IsConnected = true;
            return true;
        }

        public override void SyncAll() {
            //Console.WriteLine("==========SyncAll=========");
            Send("GetAllAgents");
            Send("GetActiveAgents");
            Send("GetAllSpecifications");
            WorkEnactors.ForEach(w => GetTaskQueuesForWorkEnactor(w));
        }

        public override void LaunchCase(string specification)
        {
            Send("LaunchCase " + specification);
        }

        public void EndAllCases()
        {
            foreach (var startedCase in StartedCases)
            {
                Send("CancelCase " + startedCase.CaseId);
            }
            StartedCases.Clear();
        }

        private void ReadMessages() {
            char[] sep = { ' ' };

            while (_externalProcessor != null && _externalProcessor.Connected) {
                byte[] bytes = new byte[16384];

                try {
                    while (_externalProcessor != null && _externalProcessor.Available == 0) {
                        Thread.Sleep(16);
                    }

                    if (_externalProcessor != null && _externalProcessor.Available > 0) {
                        _externalProcessor.Receive(bytes);

                        string[] strings = Encoding.GetString(bytes).Split('\n');

                        foreach (string xi in strings) {
                            string x = xi.Replace("\r", "");

                            System.Diagnostics.Debug.WriteLine("RECV: " + x);
                            Logger.BroadcastMessage(this, "RECV: " + x);

                            String action = x.Split(' ')[0];
                            int totalParams = x.Split(' ').Length;

                            try
                            { // Try - catch to process string processing errors, to not break the flow of the program

                                // If a new active (bot-based) agent needs to be created, notify someone of this
                                if (action == "ACTIVEAGENT" && totalParams == 4)
                                {
                                    String first = x.Split(sep)[1];
                                    String last = x.Split(sep)[2];
                                    String agentID = x.Split(sep)[3];

                                    if (AllWorkAgents.ContainsKey(agentID))
                                    {

                                        OnAgentCreated(new AgentEventArgs
                                        {
                                            Name = first + " " + last,
                                            ID = agentID
                                        });
                                    }
                                    else
                                    {
                                        throw new Exception("No Agent Exists with this ID!");
                                    }
                                }

                                // A new non-bot agent needs to be created.
                                else if (action == "AGENT" && totalParams == 4)
                                {
                                    String agentID = x.Split(sep)[3];
                                    String first = x.Split(sep)[1];
                                    String last = x.Split(sep)[2];

                                    AddYAWLWorkAgent(agentID);

                                    AllWorkAgents[agentID].FirstName = first;
                                    AllWorkAgents[agentID].LastName = last;
                                    Logger.BroadcastMessage(this, "Updating YAWLWorkAgent: " + first + " " + last);
                                }

                                // There is a role being applied to an agent
                                else if (action == "AGENTROLE" && totalParams >= 3)
                                {
                                    String agentID = x.Split(sep)[1];
                                    String role = x.Split(sep, 3)[2];

                                    AddYAWLWorkAgent(agentID);

                                    AllWorkAgents[agentID].Appearance = role;
                                    AllWorkAgents[agentID].AddRole(role);
                                    Logger.BroadcastMessage(this, "Updating YAWLWorkAgent: " + role);
                                }

                                // There is a capabality being applied to an agent
                                else if (action == "AGENTCAPABILITY" && totalParams >= 3)
                                {
                                    String agentID = x.Split(sep)[1];
                                    String capability = x.Split(sep, 3)[2];

                                    AddYAWLWorkAgent(agentID);

                                    AllWorkAgents[agentID].AddCapability(capability);
                                    Logger.BroadcastMessage(this, "Updating YAWLWorkAgent: " + capability);
                                }

                                // A workitem is being added to a queue
                                else if (action == "WORKITEM" && totalParams >= 7)
                                {
                                    String taskQueue = x.Split(sep)[1];
                                    String agentID = x.Split(sep)[2];
                                    String taskID = x.Split(sep)[3];
                                    String tasks = x.Split(sep)[4];
                                    String taskName = x.Split(sep)[5];
                                    String goals = x.Split(sep)[6];

                                    WorkItem workItem;

                                    if (!AllWorkItems.ContainsKey(taskID))
                                    {
                                        workItem = new WorkItem();
                                        workItem.taskID = taskID;
                                        AllWorkItems.Add(taskID, workItem);
                                    }
                                    else
                                    {
                                        workItem = AllWorkItems[taskID];
                                        if (AllWorkAgents.ContainsKey(workItem.agentID))
                                        {
                                            ((YAWLWorkAgent)AllWorkAgents[workItem.agentID]).GetQueueById(taskQueue).Remove(workItem);
                                        }
                                    }
                                    workItem.taskQueue = taskQueue;
                                    workItem.agentID = agentID;
                                    workItem.taskVariables.Add("Tasks", tasks);
                                    workItem.taskVariables.Add("Goals", goals);
                                    workItem.taskName = taskName;

                                    // Add work to queue if work item doesnt exist in the queue already
                                    if (AllWorkAgents.ContainsKey(agentID) && DoesNotContainWorkItem(agentID, taskQueue, workItem))
                                    {
                                        ((YAWLWorkAgent)AllWorkAgents[agentID]).AddToQueue(taskQueue, workItem);
                                    }

                                    // Add the work if it has not been completed already
                                    if (taskQueue == WorkAgent.STARTED && DoesNotContainWorkItem(agentID, WorkAgent.COMPLETED, workItem))
                                    {
                                        WorkEnactors.FirstOrDefault(w => w.WorkAgent.AgentID == agentID).AddWork(workItem);
                                    }
                                }

                                // A work item name is being applied to a work item
                                else if (action == "WORKITEMNAME" && totalParams >= 3)
                                {
                                    String taskID = x.Split(sep)[1];
                                    String taskName = x.Split(sep, 3)[2];

                                    if (!AllWorkItems.ContainsKey(taskID))
                                    {
                                        WorkItem work = new WorkItem();
                                        work.taskID = taskID;
                                        AllWorkItems.Add(taskID, work);
                                    }

                                    AllWorkItems[taskID].taskName = taskName;
                                }

                                // A work item is being signalled to end
                                else if (action == "TASKEND" && totalParams == 3)
                                {
                                    String agentID = x.Split(sep)[1];
                                    String taskID = x.Split(sep)[2];

                                    if (AllWorkItems.ContainsKey(taskID) && AllWorkAgents.ContainsKey(agentID))
                                    {
                                        WorkItem work = AllWorkItems[taskID];
                                        if (work.agentID == agentID)
                                        {
                                            ((YAWLWorkAgent)AllWorkAgents[agentID]).GetQueueById(work.taskQueue).Remove(work);
                                            if (work.taskQueue == WorkAgent.STARTED)
                                            {
                                                WorkEnactors.FirstOrDefault(w => w.WorkAgent.AgentID == agentID).StopTaskIfStarted(work);
                                            }
                                            AllWorkItems.Remove(taskID);
                                        }
                                    }
                                }

                                // A work item is being signalled to be suspended
                                else if (action == "SUSPEND" && totalParams == 3)
                                {
                                    String agentID = x.Split(sep)[1];
                                    String taskID = x.Split(sep)[2];

                                    if (AllWorkItems.ContainsKey(taskID) && AllWorkAgents.ContainsKey(agentID))
                                    {
                                        WorkItem work = AllWorkItems[taskID];
                                        if (work.agentID == agentID)
                                        {
                                            ((YAWLWorkAgent)AllWorkAgents[agentID]).GetQueueById(work.taskQueue).Remove(work);
                                            if (work.taskQueue == WorkAgent.STARTED)
                                            {
                                                WorkEnactors.FirstOrDefault(w => w.WorkAgent.AgentID == agentID).StopTaskIfStarted(work);
                                            }
                                            ((YAWLWorkAgent)AllWorkAgents[agentID]).GetQueueById(WorkAgent.SUSPENDED).Add(work);
                                        }
                                    }
                                }

                                // A work item is being signalled to be unsuspended
                                else if (action == "UNSUSPEND" && totalParams == 3)
                                {
                                    String agentID = x.Split(sep)[1];
                                    String taskID = x.Split(sep)[2];

                                    if (AllWorkItems.ContainsKey(taskID) && AllWorkAgents.ContainsKey(agentID))
                                    {
                                        WorkItem work = AllWorkItems[taskID];
                                        if (work.agentID == agentID)
                                        {
                                            ((YAWLWorkAgent)AllWorkAgents[agentID]).GetQueueById(work.taskQueue).Remove(work);
                                            ((YAWLWorkAgent)AllWorkAgents[agentID]).GetQueueById(WorkAgent.STARTED).Add(work);
                                            WorkEnactors.FirstOrDefault(w => w.WorkAgent.AgentID == agentID).AddWork(work);
                                        }
                                    }
                                }

                                    // A new case has been launched
                                else if (action == "CASE" && totalParams == 3)
                                {
                                    String caseId = x.Split(sep)[1];
                                    String specificationId = x.Split(sep)[2];
                                    String specificationName = string.Empty;

                                    if (!StartedCases.Any(c => c.CaseId.Equals(caseId)))
                                    {
                                        // Find the name of the specification
                                        if (AllSpecifications.ContainsKey(specificationId))
                                        {
                                            specificationName = AllSpecifications[specificationId];
                                        }
                                        // Add it to the list of started cases and "begin" the simulation
                                        // with a SyncAll() call
                                        StartedCases.Add(new Case { CaseId = caseId, SpecificationId = specificationId, SpecificationName = specificationName});
                                        Thread.Sleep(1000);
                                        OnCaseStateChanged(new CaseStateEventArgs
                                        {
                                            State = CaseState.STARTED,
                                            CaseID = caseId,
                                            SpecificationID = specificationId
                                        });
                                        SyncAll(); // TODO: do this in the event handler listener
                                    }
                                }

                                else if (action == "CASEEND" && totalParams == 3)
                                {
                                    String caseId = x.Split(sep)[1];
                                    String specificationId = x.Split(sep)[2];
                                    
                                    if (StartedCases.Any(c => c.CaseId.Equals(caseId)))
                                    {
                                        StartedCases.Remove(StartedCases.FirstOrDefault(c => c.CaseId.Equals(caseId)));
                                        OnCaseStateChanged(new CaseStateEventArgs
                                        {
                                            State = CaseState.COMPLETED,
                                            CaseID = caseId,
                                            SpecificationID = specificationId
                                        });
                                    }
                                }

                                else if (action == "SPECIFICATION" && totalParams == 3)
                                {                                    
                                    String specificationId = x.Split(sep)[1];
                                    String specificationName = x.Split(sep)[2];

                                    if (!AllSpecifications.ContainsKey(specificationId))
                                    {
                                        AllSpecifications.Add(specificationId, specificationName);
                                    }
                                }


                                else if (x.Length > 1)
                                {
                                    // Console.WriteLine("Unknown command " + x);
                                }
                            }
                            catch (KeyNotFoundException e)
                            {
                                System.Diagnostics.Debug.WriteLine(e.Message);
                            }
                        }
                    }
                } catch (Exception e) {
                    //Do nothing
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        protected void AddYAWLWorkAgent(string agentID)
        {
            if (!AllWorkAgents.ContainsKey(agentID))
            {
                YAWLWorkAgent agent = new YAWLWorkAgent { AgentID = agentID };
                AllWorkAgents.Add(agentID, agent);
                Logger.BroadcastMessage(this, "Adding YAWLWorkAgent: " + agentID);
            }
        }

        private bool DoesNotContainWorkItem(string yawlID, string queueID, WorkItem workItem)
        {
            return !AllWorkAgents[yawlID].GetQueueById(queueID).Any(w => w.taskID == workItem.taskID);
        }
    }
}

using System;
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
        #region Class Members

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

        public List<WorkEnactor> WorkEnactors { get; set; } // Things that can enact workitems

        private Socket _yawlSocket;
        private Thread _yawlReceiverThread;

        public YAWLWorkflowProvider()
        {
            _yawlSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            WorkEnactors = new List<WorkEnactor>();
            _yawlReceiverThread = new Thread(ReadYAWLMessages);
        }

        public void ResetAll()
        {
            EndAllCases();
            WorkEnactors.Clear();
            StartedCases.Clear();
            AllWorkItems.Clear();
            AllWorkAgents.Clear();
            SyncAll();
        }

        #endregion

        #region Work Enactors

        public void AddWorkEnactor(WorkEnactor workEnactor)
        {
            WorkEnactors.Add(workEnactor);
            //GetTaskQueuesForWorkEnactor(workEnactor);
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

        #endregion

        #region YAWL Relay Connection

        public override bool Start()
        {
            try
            {
                _yawlSocket.Connect(WorkflowConnectionUrl, WorkflowConnectionPort);
                _yawlReceiverThread.Start();
                Logger.BroadcastMessage(this, "Connected to workflow provider");
            }
            catch (Exception e)
            {
                Logger.BroadcastMessage(this, "Could not connect to YAWL Workflow service: " + e.Message);
                IsConnected = false;
                return false;
            }

            IsConnected = true;
            return true;
        }

        public override void End()
        {
            EndAllCases();
            Send("EndSession");
            if (_yawlSocket.Connected)
            {
                _yawlSocket.Shutdown(SocketShutdown.Both);
                _yawlSocket.Disconnect(false);
                //_externalProcessor.Close();
                _yawlSocket = null;
            }

            if (_yawlReceiverThread.ThreadState == ThreadState.Running) _yawlReceiverThread.Join();
            IsConnected = false;
        }

        #endregion

        #region Send Messages to YAWL

        public override void LaunchCase(string specificationName)
        {
            Case newCase = AllCases.FirstOrDefault(c => c.SpecificationName == specificationName);
            Send(string.Format("LaunchCase {0} {1} {2}", newCase.Identifier, newCase.Version, newCase.SpecificationName));
        }

        public void EndAllCases()
        {
            foreach (var startedCase in StartedCases)
            {
                Send("CancelCase " + startedCase.CaseID);
            }
            StartedCases.Clear();
        }

        public void GetTaskQueuesForWorkEnactor(WorkEnactor workEnactor)
        {
            new Thread(() =>
            {
                Thread.Sleep(1000);
                string agentID = workEnactor.WorkAgent.AgentID;
                Send("GetTaskQueue " + WorkAgent.OFFERED + " " + agentID);
                Send("GetTaskQueue " + WorkAgent.ALLOCATED + " " + agentID);
                Send("GetTaskQueue " + WorkAgent.STARTED + " " + agentID);
                //Send("GetTaskQueue " + WorkAgent.SUSPENDED + " " + agentID);
            }).Start();

        }

        public override void EndWorkItem(WorkAgent agent, WorkItem workItem)
        {
            Send("WorkItemAction Complete " + agent.AgentID + " " + workItem.WorkItemID + " "
                + workItem.CaseID + " " + workItem.SpecificationID);
        }

        public override void SyncAll()
        {
            //Console.WriteLine("==========SyncAll=========");
            Send("GetAllAgents");
            Send("GetActiveAgents");
            Send("GetAllSpecifications");
            //WorkEnactors.ForEach(w => GetTaskQueuesForWorkEnactor(w));
        }

        public override void Send(string msg)
        {
            if (!_yawlSocket.Connected) return;
            byte[] bytes = Encoding.GetBytes(msg + "\n");
            _yawlSocket.Send(bytes);
            System.Diagnostics.Debug.WriteLine("SEND: " + msg);
            Logger.BroadcastMessage(this, "SEND: " + msg);
        }

        #endregion

        #region Receive Messages from YAWL

        private void ReadYAWLMessages()
        {
            char[] sep = { ' ' };

            while (_yawlSocket != null && _yawlSocket.Connected)
            {
                byte[] bytes = new byte[16384];

                try
                {
                    _yawlSocket.Receive(bytes);

                    string[] strings = Encoding.GetString(bytes).Split('\n');

                    foreach (string xi in strings)
                    {
                        string x = xi.Replace("\r", "");
                        String action = x.Split(' ')[0];
                        int totalParams = x.Split(' ').Length;
                        if (action.Length > 5)
                        {
                            Logger.BroadcastMessage(this, "RECV: " + x);
                        }

                        #region Agents
                        // If a new active (bot-based) agent needs to be created, notify someone of this
                        if (action == "ACTIVEAGENT")
                        {
                            String first = x.Split(sep)[1];
                            String last = x.Split(sep)[2];
                            String agentID = x.Split(sep)[3];

                            if (AllWorkAgents.Any(a => a.AgentID == agentID))
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
                        else if (action == "AGENT")
                        {
                            String agentID = x.Split(sep)[3];
                            String first = x.Split(sep)[1];
                            String last = x.Split(sep)[2];

                            AddYAWLWorkAgent(agentID);
                            WorkAgent workAgent = AllWorkAgents.FirstOrDefault(a => a.AgentID == agentID);
                            workAgent.FirstName = first;
                            workAgent.LastName = last;

                            Logger.BroadcastMessage(this, "Updating YAWLWorkAgent: " + first + " " + last);
                        }

                        // There is a role being applied to an agent
                        else if (action == "AGENTROLE")
                        {
                            String agentID = x.Split(sep)[1];
                            String role = x.Split(sep, 3)[2];

                            AddYAWLWorkAgent(agentID);
                            WorkAgent workAgent = AllWorkAgents.FirstOrDefault(a => a.AgentID == agentID);

                            workAgent.Appearance = role;
                            workAgent.AddRole(role);
                            Logger.BroadcastMessage(this, "Updating YAWLWorkAgent: " + role);
                        }

                        // There is a capabality being applied to an agent
                        else if (action == "AGENTCAPABILITY")
                        {
                            String agentID = x.Split(sep)[1];
                            String capability = x.Split(sep, 3)[2];

                            AddYAWLWorkAgent(agentID);

                            AllWorkAgents.FirstOrDefault(a => a.AgentID == agentID).AddCapability(capability);
                            Logger.BroadcastMessage(this, "Updating YAWLWorkAgent: " + capability);
                        }

                        #endregion

                        #region Work Items

                        // A workitem is being added to a queue
                        else if (action == "WORKITEM")
                        {
                            string taskID = x.Split(sep)[5];
                            string agentID = x.Split(sep)[6];
                            string taskQueue = x.Split(sep)[7];

                            if (!AllWorkItems.Any(w => w.TaskID == taskID))
                            {
                                WorkItem workItem = new WorkItem();
                                workItem.TaskID = taskID;
                                workItem.CaseID = x.Split(sep)[1];
                                workItem.SpecificationID = x.Split(sep)[2];
                                workItem.UniqueID = x.Split(sep)[3];
                                workItem.WorkItemID = x.Split(sep)[4];
                                workItem.TaskID = taskID;
                                workItem.TaskName = taskID;
                                workItem.AgentID = agentID;
                                workItem.TaskQueue = taskQueue;
                                workItem.tasksAndGoals.Add("Tasks", x.Split(sep)[8]);
                                workItem.tasksAndGoals.Add("Goals", x.Split(sep)[9]);

                                AllWorkItems.Add(workItem);
                                WorkEnactor workEnactor = WorkEnactors.FirstOrDefault(w => w.WorkAgent.AgentID == agentID);
                                if (workEnactor != null)
                                {
                                    workEnactor.AddWorkItem(workItem);
                                }
                            }
                        }

                        // A work item name is being applied to a work item
                        else if (action == "WORKITEMNAME")
                        {
                            String taskID = x.Split(sep)[1];
                            String taskName = x.Split(sep, 3)[2];

                            if (!AllWorkItems.Any(w => w.TaskID == taskID))
                            {
                                WorkItem workItem = new WorkItem();
                                workItem.TaskID = taskID;
                                AllWorkItems.Add(workItem);
                            }

                            AllWorkItems.FirstOrDefault(w => w.TaskID == taskID).TaskName = taskName;
                        }

                        // A work item is being signalled to end
                        else if (action == "TASKEND")
                        {
                            String agentID = x.Split(sep)[1];
                            String workItemID = x.Split(sep)[2];
                            WorkItem workItem = AllWorkItems.FirstOrDefault(w => w.WorkItemID == workItemID && w.AgentID == agentID);
                            WorkAgent workAgent = AllWorkAgents.FirstOrDefault(a => a.AgentID == agentID);

                            if (workItem != null && workAgent != null)
                            {
                                workAgent.GetQueueById(workItem.TaskQueue).Remove(workItem);
                                if (workItem.TaskQueue == WorkAgent.STARTED)
                                {
                                    WorkEnactors.FirstOrDefault(w => w.WorkAgent.AgentID == agentID).StopWorkItem(workItem);
                                }
                                AllWorkItems.Remove(workItem);
                            }

                            WorkEnactors.ForEach(w => GetTaskQueuesForWorkEnactor(w));
                        }

                        #endregion

                        #region Cases
                        // A new case has been launched
                        else if (action == "CASE")
                        {
                            string identifier = x.Split(sep)[1];
                            string caseID = x.Split(sep)[1];
                            string specificationID = x.Split(sep)[2];
                            string specificationName = string.Empty;

                            if (!StartedCases.Any(c => c.CaseID.Equals(caseID)))
                            {
                                // Add it to the list of started cases and "begin" the simulation
                                // with a SyncAll() call
                                CurrentCase = AllCases.FirstOrDefault(c => c.SpecificationID == specificationID);
                                CurrentCase.CaseID = caseID;
                                StartedCases.Add(CurrentCase);
                                //OnCaseStateChanged(new CaseStateEventArgs
                                //{
                                //    State = CaseState.STARTED,
                                //    CaseID = caseID,
                                //    SpecificationID = specificationID
                                //});
                                //SyncAll(); // TODO: do this in the event handler listener
                                WorkEnactors.ForEach(w => GetTaskQueuesForWorkEnactor(w));
                            }
                        }

                        else if (action == "CASEEND")
                        {
                            string caseId = x.Split(sep)[1];
                            string specificationId = x.Split(sep)[2];
                            if (StartedCases.Any(c => c.SpecificationID == specificationId))
                            {
                                StartedCases.Remove(StartedCases.FirstOrDefault(c => c.SpecificationID == specificationId));
                                OnCaseStateChanged(new CaseStateEventArgs
                                {
                                    State = CaseState.COMPLETED,
                                    CaseID = caseId,
                                    SpecificationID = specificationId
                                });
                            }
                        }

                        else if (action == "SPECIFICATION")
                        {
                            string specificationID = x.Split(sep)[3];
                            if (!AllCases.Any(c => c.SpecificationID == specificationID))
                            {
                                AllCases.Add(new Case
                                {
                                    Identifier = x.Split(sep)[1],
                                    Version = x.Split(sep)[2],
                                    SpecificationID = specificationID,
                                    SpecificationName = x.Split(sep)[4]
                                });
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception e)
                {
                    //Do nothing
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        #endregion

        protected void AddYAWLWorkAgent(string agentID)
        {
            if (!AllWorkAgents.Any(a => a.AgentID == agentID))
            {
                YAWLWorkAgent agent = new YAWLWorkAgent { AgentID = agentID };
                AllWorkAgents.Add(agent);
                Logger.BroadcastMessage(this, "Adding YAWLWorkAgent: " + agentID);
            }
        }
    }
}

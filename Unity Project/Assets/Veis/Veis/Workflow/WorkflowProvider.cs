using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Workflow
{
    /// <summary>
    /// Provides a generic abstract class for a workflow provider.
    /// A workflow provider has a collection of work items, and a list
    /// of participants in the workflow.
    /// It is designed to be subclassed by more specific workflow providers
    /// such as YAWL.
    /// </summary>
    public abstract class WorkflowProvider
    {
        public Dictionary<string, WorkItem> AllWorkItems { get; set; }
        public Dictionary<string, WorkAgent> AllParticipants { get; set; }
        public Dictionary<string, string> AllSpecifications { get; set; } // ID/Name
        public List<Case> StartedCases { get; set; } // The cases that have been started through this program
        public List<Case> CompletedCases { get; set; } // The cases that have been completed through this program

        protected WorkflowProvider()
        {
            AllWorkItems = new Dictionary<string, WorkItem>();
            AllParticipants = new Dictionary<string, WorkAgent>();
            AllSpecifications = new Dictionary<string, string>();
            StartedCases = new List<Case>();
            CompletedCases = new List<Case>();
        }

        public WorkItem GetWorkItemByName(string name) {
            foreach (KeyValuePair<string, WorkItem> workKVP in AllWorkItems) {
                if (workKVP.Value.taskName == name) {
                        return workKVP.Value;
                }
            }

            //Didn't find it
            return null;
        }

        public WorkAgent GetAgentByFirstName(string name) {
            foreach (KeyValuePair<string, WorkAgent> agentKVP in AllParticipants) {
                if (agentKVP.Value.FirstName == name) {
                    return agentKVP.Value;
                }
            }

            //Didn't find it
            return null;
        }

        public string GetAgentIdByFullName(string name)
        {
            foreach (KeyValuePair<string, WorkAgent> agentKVP in AllParticipants)
            {
                if (String.Format("{0} {1}", agentKVP.Value.FirstName, agentKVP.Value.LastName) == name)
                {
                    return agentKVP.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Should be called when a work item has been completed by one of the agents
        /// </summary>
        /// <param name="agent">The agent who completed the work item</param>
        /// <param name="workItem">The work item that was completed</param>
        public abstract void EndWorkItem(WorkAgent agent, WorkItem workItem);

        /// <summary>
        /// Sends a generic text message to the workflow provider
        /// </summary>
        /// <param name="message">The message to send</param>
        public abstract void Send(string message);

        /// <summary>
        /// Syncronises work information between this and the external
        /// provider.
        /// </summary>
        public abstract void SyncAll();

        /// <summary>
        /// Shuts down the workflow provider
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Checks if the 
        /// </summary>
        public abstract bool Connect();
        public bool IsConnected { get; protected set; }

        /// <summary>
        /// Attempts to launch a new instance of a the given workflow specification
        /// </summary>
        /// <returns></returns>
        public abstract void LaunchCase(string specification);

        

    }
}

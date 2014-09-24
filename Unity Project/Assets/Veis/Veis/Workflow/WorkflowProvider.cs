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
        public List<WorkItem> AllWorkItems { get; set; }
        public List<WorkAgent> AllWorkAgents { get; set; }
        public List<Case> AllCases { get; set; }
        public List<Case> StartedCases { get; set; } // The cases that have been started through this program
        public List<Case> CompletedCases { get; set; } // The cases that have been completed through this program
        public Case CurrentCase { get; set; }

        protected WorkflowProvider()
        {
            AllWorkItems = new List<WorkItem>();
            AllWorkAgents = new List<WorkAgent>();
            AllCases = new List<Case>();
            StartedCases = new List<Case>();
            CompletedCases = new List<Case>();
        }

        public string GetAgentIdByFullName(string name)
        {
            return AllWorkAgents.FirstOrDefault(a => (String.Format("{0} {1}", a.FirstName, a.LastName)) == name).AgentID;
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
        public abstract void End();

        /// <summary>
        /// Checks if the 
        /// </summary>
        public abstract bool Start();
        public bool IsConnected { get; protected set; }

        /// <summary>
        /// Attempts to launch a new instance of a the given workflow specification
        /// </summary>
        /// <returns></returns>
        public abstract void LaunchCase(string specification);

        

    }
}

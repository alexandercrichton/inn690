package veis.entities.workflow;

import java.util.ArrayList;

import veis.system.AgentReceiverSocketServer;
import veis.yawl.YAWLComm;

public class Agent {
	// WorkQueue constants
	public enum QueueState {
		OFFERED,
		ALLOCATED,
		STARTED,
		SUSPENDED
	}
	/*
	public static final int OFFERED = 0;
	public static final int ALLOCATED = 1;
	public static final int STARTED = 2;
	public static final int SUSPENDED = 3;
	*/
	
	// Internal data
	private String UID;
	private String userID;
	private String firstName;
	private String secondName;
	private ArrayList<String> roles;
	private ArrayList<String> capabilities;
	
	//Work queues
	private ArrayList<WorkItem> offered;
	private ArrayList<WorkItem> allocated;
	private ArrayList<WorkItem> started;
	private ArrayList<WorkItem> suspended;
	
	public Agent() {
		roles = new ArrayList<String>();
		capabilities = new ArrayList<String>();
		
		offered = new ArrayList<WorkItem>();
		allocated = new ArrayList<WorkItem>();
		started = new ArrayList<WorkItem>();
		suspended = new ArrayList<WorkItem>();
	}
	
	public Agent(String UID, String first, String last, String userID) {
		this();
		this.UID = UID;
		this.firstName = first;
		this.secondName = last;
		this.userID = userID;
	}
	
	public void SetFirstName(String firstName) {
		this.firstName = firstName;
	}
	
	public String GetFirstName() {
		return this.firstName;
	}
	
	public void SetSecondName(String secondName) {
		this.secondName = secondName;
	}
	
	public String GetSecondName() {
		return this.secondName;
	}
	
	public void SetUID(String uID) {
		this.UID = uID;
	}
	
	public String GetUID() {
		return this.UID;
	}
	
	public void SetUserID(String userID) {
		this.userID = userID;
	}
	
	public String GetUserID() {
		return this.userID;
	}
	
	//Add a role to roles arrayList if not exist
	public void AddRole(String role) {
		if(!roles.contains(role)) {
			AgentReceiverSocketServer.SendToAllClients("AGENTROLE " + this.UID + " " + role);
			roles.add(role);
		}
	}

	public ArrayList<String> GetRoles() {
		return roles;
	}
	
	//Add a capability if not exist
	public void AddCapability(String capability) {
		if(!capabilities.contains(capability)) {
			AgentReceiverSocketServer.SendToAllClients("AGENTCAPABILITY " + this.UID + " " + capability);
			capabilities.add(capability);
		}
	}
	
	public ArrayList<String> GetCapabilities() {
		return capabilities;
	}
	
	public String toString() {
		String output = firstName + " " + secondName + " (" + UID + ")";
		for(String role : roles) {
			output += "\n\t" + role;
		}
		return output;
	}
	
	/**
	 * Get a queue of current agent according to the ID: 0, 1, 2, 3
	 * @param queueID
	 * @return
	 */
	public ArrayList<WorkItem> GetQueue(QueueState queueID) {
		if(queueID == QueueState.OFFERED) {
			return offered;
		} else if(queueID == QueueState.ALLOCATED) {
			return allocated;
		} else if(queueID == QueueState.STARTED) {
			return started;
		} else if(queueID == QueueState.SUSPENDED) {
			return suspended;
		}
		
		return null;
	}
	
	/**
	 * Update all Offered, Allocated, Started, and Suspended queues.
	 * @throws Exception
	 */
	public void UpdateQueues() throws Exception {
		YAWLComm.GetInstance().ParseQueuedWorkQueue(this, QueueState.OFFERED);
		YAWLComm.GetInstance().ParseQueuedWorkQueue(this, QueueState.ALLOCATED);
		YAWLComm.GetInstance().ParseQueuedWorkQueue(this, QueueState.STARTED);
		YAWLComm.GetInstance().ParseQueuedWorkQueue(this, QueueState.SUSPENDED);
		
		/**
		 * If current queue of the agent is empty, then try to pull the next work item on the front of the allocated queue
		 */
		if(started.size() == 0 && allocated.size() > 0) {
			boolean successful = allocated.get(0).Start(this);
			if (successful) {
				for (WorkItem a : this.GetQueue(QueueState.STARTED)) {
					AgentReceiverSocketServer.SendToAllClients("WORKITEM " + QueueState.STARTED + " " + a.participant + " " + a.uniqueID + " " + a.tasks + " " + a.taskID);
				}
			}
		} else 
			/**
			 * If current queue of the agent is empty and some work items are waiting to be picked
			 */
			if(started.size() == 0 && offered.size() > 0) {
				// The agent automatically starts the next offered task
				offered.get(0).Accept(this);   
		}
	}
	
	/**  
	 * An heuristic method check if the current agent are working
	 * @return
	 */
	public boolean isActive() {
		if(started.size() > 0)  return true;
		
		//Maybe start one here?
		if(allocated.size() > 0) return true;
		
		return false;
	}
}

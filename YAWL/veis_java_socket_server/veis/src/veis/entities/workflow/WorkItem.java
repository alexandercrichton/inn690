package veis.entities.workflow;

import veis.system.SystemStorage;
import veis.yawl.YAWLComm;

public class WorkItem {
	public String workItemID = "";
	public String caseID = "";
	public String taskID = "";
	public String uniqueID = "";
	public String requiresManualResourcing = "";
	public String status = "";
	public String resourceStatus = "";
	public String specID = "";
	public String participant = "";
	public String tasks = "";
	public String goals = "";
	
	/**
	 * Force complete the current work item.
	 * @return
	 * @throws Exception
	 */
	public boolean Complete() throws Exception {
		if(YAWLComm.GetInstance().CompleteWorkItem(this)) {
			SystemStorage.GetInstance().RemoveWorkItem(this);
			return true;
		}
		return false;
	}
	
	public boolean Cancel() throws Exception {
		if(YAWLComm.GetInstance().CancelWorkItem(this)){
			SystemStorage.GetInstance().RemoveWorkItem(this);
			return true;
		}
		return false;
	}

	public boolean Delegate(Agent newAgent) throws Exception {
		return YAWLComm.GetInstance().DelegateWorkItem(this, newAgent);
	}
	
	public boolean Suspend() throws Exception {
		return YAWLComm.GetInstance().SuspendWorkItem(this);
	}
	
	public boolean Unsuspend() throws Exception {
		return YAWLComm.GetInstance().UnsuspendWorkItem(this);
	}
	
	public boolean Accept(Agent accepter) throws Exception {
		return YAWLComm.GetInstance().AcceptWorkItem(this, accepter);
	}
	
	public boolean Start(Agent accepter) throws Exception {
		return YAWLComm.GetInstance().StartWorkItem(this, accepter);
	}
}

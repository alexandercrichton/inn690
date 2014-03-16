package veis.yawl;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import main.Application;
import main.Utilities;

import org.yawlfoundation.yawl.engine.YSpecificationID;
import org.yawlfoundation.yawl.engine.interfce.interfaceA.InterfaceA_EnvironmentBasedClient;
import org.yawlfoundation.yawl.engine.interfce.interfaceB.*;
import org.yawlfoundation.yawl.engine.interfce.interfaceX.InterfaceX_ServiceSideClient;
import org.yawlfoundation.yawl.logging.YLogDataItemList;
import org.yawlfoundation.yawl.resourcing.rsInterface.*;
import org.yawlfoundation.yawl.util.XNode;
import org.yawlfoundation.yawl.util.XNodeParser;

import veis.entities.workflow.*;
import veis.system.SystemStorage;

/**
 * This is a singleton class
 *
 */
public class YAWLComm {
	public final int OFFERED = 0;
	public final int ALLOCATED = 1;
	public final int STARTED = 2;
	public final int SUSPENDED = 3;

	private InterfaceA_EnvironmentBasedClient ia;
	private InterfaceB_EnvironmentBasedClient ib;
	private ResourceGatewayClient rs;
	private WorkQueueGatewayClient wq;
	private InterfaceX_ServiceSideClient ixC;

	private String sessionIAHandle = "";
	private String sessionIBHandle = "";
	private String sessionRSHandle = "";
	private String sessionWQHandle = "";
	
	private static YAWLComm instance = null;
	
	private static XNodeParser parser = new XNodeParser();
	
	public static void DestroyLastSession(){
		instance = null;
	}
	
	public static YAWLComm GetInstance(){
		try{
			if(instance == null) {
				try {
					return new YAWLComm();
				} catch (Exception e) {
					//e.printStackTrace();
					System.err.println("Cannot connect to YAWL server.");
				}
			}
			return instance;
		}
		catch(Exception ex){
			//ex.printStackTrace();
		}
		return null;
	}
	
	public YAWLComm() throws Exception {
		if(instance != null) throw new Exception("A YAWLComm object already exists! Don't try to create one and call get instance instead!");
		instance = this;

		ia = new InterfaceA_EnvironmentBasedClient("http://localhost:8080/yawl/ia");
		ib = new InterfaceB_EnvironmentBasedClient("http://localhost:8080/yawl/ib");
		rs = new ResourceGatewayClient("http://localhost:8080/resourceService/gateway");
		wq = new WorkQueueGatewayClient("http://localhost:8080/resourceService/workqueuegateway");
		ixC = new InterfaceX_ServiceSideClient("http://localhost:8080/yawl/ix");
		
		sessionIAHandle = ia.connect(Application.YAWLAdmin, Application.YAWLPasswd);
		sessionIBHandle = ib.connect(Application.YAWLAdmin, Application.YAWLPasswd);
		sessionRSHandle = rs.connect(Application.YAWLAdmin, Application.YAWLPasswd);
		sessionWQHandle = wq.connect(Application.YAWLAdmin, Application.YAWLPasswd);
		
		System.out.println("SESSION-IX: " + ixC.addInterfaceXListener("http://localhost:" + YAWLSocketServerIX.port));
		System.out.println("SESSION-IA: " + sessionIAHandle);
		System.out.println("SESSION-IB: " + sessionIBHandle);
		System.out.println("SESSION-RS: " + sessionRSHandle);
		System.out.println("SESSION-WQ: " + sessionWQHandle);
	}
	
	public boolean isConnectionAlive(){
		try{
			ia.connect(Application.YAWLAdmin, Application.YAWLPasswd);
		} catch(IOException ex)	{
			return false;
		}
		return true;
	}
	

	/**
	 * Fetch and cache all items from YAWL to local database (SystemStorage)
	 * @throws Exception
	 */
	public void SyncAll() throws Exception {
		//RS interface
		ProcessHumanResources();
		//IB interface
		ProcessSpecifications();
		//QW interface
		ProcessWorkItems();
		//IB interface
		ProcessCases();
	}
	
	/**
	 * Fetch all participants from YAWL via RS interface,
	 * if that person is not in SystemStorage, then add in. Meanwhile, any new Roles or Capabilities also added.
	 * @throws Exception
	 */
	public void ProcessHumanResources() throws Exception {
		//System.err.println("ProcessHumanResources");
		//Query XML from RS interface
		String peopleXML = rs.getParticipants(sessionRSHandle);
		//Convert it to XML
		XNode peopleNode = parser.parse(peopleXML);
		//Output for debugging purpose
		//Utilities.OutputNodeXML(peopleNode);
		
		if (peopleNode.getName().equals("participants") && peopleNode.getChildCount() > 0) {
			for(XNode child : peopleNode.getChildren()) {
				if(child.getName().equals("participant") && child.getChildCount() > 0) {
					String uid = child.getAttributeValue("id");
					
					if(SystemStorage.GetInstance().GetAgent(uid) == null) {
						String userid = child.getChild("userid").getText();
						String first = child.getChild("firstname").getText();
						String last = child.getChild("lastname").getText();
						SystemStorage.GetInstance().AddAgent(new Agent(uid, first, last, userid));
						
						for(XNode rolesNode : child.getChild("roles").getChildren()) {
							if(rolesNode.hasChild("name")) {
								SystemStorage.GetInstance().GetAgent(uid).AddRole(rolesNode.getChild("name").getText());
							}
						}
						
						for(XNode capsNode : child.getChild("capabilities").getChildren()) {
							if(capsNode.hasChild("name")) {
								SystemStorage.GetInstance().GetAgent(uid).AddCapability(capsNode.getChild("name").getText());
							}
						}
					}
				}
			}
		}
	}
	
	/**
	 * Fetch all specifications from YAWL via IB interface,
	 * if the specification does not exist in SystemStorage, then add in.
	 * @throws Exception
	 */
	public void ProcessSpecifications() throws Exception {
		
		//System.err.println("ProcessSpecifications");
		//Query Specifications list via IB interface
		String result = YAWL_IB_Action.execute(ib.getBackEndURI(), "getSpecificationPrototypesList", sessionIBHandle);
		XNode root = parser.parse(result);
		//Utilities.OutputNodeXML(root);
		
		if(root.getName().equals("response") && root.getChild().getName().equals("specificationData")) {
			for(XNode child : root.getChildren()) {
				String uid = child.getChild("id").getText();
				if(SystemStorage.GetInstance().GetSpecification(uid) == null) {
					String version = child.getChild("specversion").getText();
					String uri = child.getChild("uri").getText();
					SystemStorage.GetInstance().AddSpecification(new Specification(uid, version, uri));
				}
			}
		}
	}
	
	/**
	 * Get all agents in the system, then each of them will be Update their queue.
	 * @throws Exception
	 */
	public void ProcessWorkItems() throws Exception {
		//System.err.println("ProcessWorkItems");
		for(Agent agent : SystemStorage.GetInstance().GetAgents()) {
			agent.UpdateQueues();
		}
	}
	
	/**
	 * Fetch work queue via WQ interface
	 * @param agent
	 * @param workQueue
	 * @throws Exception
	 */
	public void ParseQueuedWorkQueue(Agent agent, Agent.QueueState workQueue) throws Exception {
		
		System.err.println("ParseQueuedWorkQueue " + agent.GetFirstName() + " " + workQueue.toString());
		
		Map<String, WorkItem> temporaryQueue = new HashMap<String, WorkItem>();
		
		for(WorkItem tItem : agent.GetQueue(workQueue)) {
			temporaryQueue.put(tItem.uniqueID, tItem);
		}
		
		String qq = wq.getQueuedWorkItems(agent.GetUID(), workQueue.ordinal(), sessionWQHandle);
		XNode root = parser.parse(qq);
		//Utilities.OutputNodeXML(root);
		
		if(root.getChildCount() > 0) {
			for(XNode workChild : root.getChildren()) {
				if(workChild.getName().equalsIgnoreCase("workItemRecord")) {
					
					//If it is a case, not a workitem, then skip it.
					//A case does not contain a dot in its id
					String caseId = workChild.getChildText("caseid");
					if( ! caseId.contains(".")) {
						System.err.println("A case detected: " + caseId);
						//continue;
					} else {
						System.out.println("A WI detected: " + caseId);
					}
					
					String uniqueID = workChild.getChild("uniqueid").getText();
					
					if(SystemStorage.GetInstance().GetWorkItem(uniqueID) == null) {
						WorkItem work = new WorkItem();
						work.workItemID = workChild.getChild("id").getText();
						work.caseID = workChild.getChild("caseid").getText();
						work.taskID = workChild.getChild("taskid").getText();
						work.uniqueID = uniqueID;
						work.requiresManualResourcing = workChild.getChild("requiresmanualresourcing").getText();
						work.status = workChild.getChild("status").getText();
						work.resourceStatus = workChild.getChild("resourceStatus").getText();
						work.specID = workChild.getChild("specidentifier").getText();
						
						YSpecificationID ySpec = SystemStorage.GetInstance().GetSpecification(work.specID).GetYSpecID();
						XNode specData = parser.parse(ib.getTaskInformationStr(ySpec, work.taskID, sessionIBHandle));
						
						if(specData.hasChild("taskInfo")) {
							specData = specData.getChild("taskInfo");
							if(specData.hasChild("params")) {
								specData = specData.getChild("params");
								if(specData.hasChild("outputParam")) {
									for(XNode param : specData.getChildren("outputParam")) {
										if(param.getChild("name").getText().equals("Tasks")) {
											work.tasks = param.getChild("defaultValue").getText().replaceAll("&amp;lt;", "<").replaceAll("&amp;gt;", ">").replaceAll("&amp;apos;", "'");
										}
										else if (param.getChild("name").getText().equals("Goal")){
											work.goals = param.getChild("defaultValue").getText().replaceAll("&amp;lt;", "<").replaceAll("&amp;gt;", ">").replaceAll("&amp;apos;", "'");
										}
									}
								}
							}
						}
						
						SystemStorage.GetInstance().AddWorkItem(work);
						
					}
					
					SystemStorage.GetInstance().GetWorkItem(uniqueID).participant = agent.GetUID();
					
					if(temporaryQueue.containsKey(uniqueID)){
						agent.GetQueue(workQueue).remove(temporaryQueue.get(uniqueID));
						temporaryQueue.remove(uniqueID);
					}


					
						agent.GetQueue(workQueue).add(SystemStorage.GetInstance().GetWorkItem(uniqueID));
				}
			}
		} else {
			//The queue is actually empty
			//System.out.println("EMPTY");
		}
		
		for(String kvp : temporaryQueue.keySet()) {
			agent.GetQueue(workQueue).remove(temporaryQueue.get(kvp));
		}
	}
	
	/**
	 * This method fetches all running cases on YAWL via IB interface,
	 * Each specification will be query to find a running instance
	 * @throws Exception
	 */
	public void ProcessCases() throws Exception {
		//System.err.println("ProcessCases");
		for (Specification specification : SystemStorage.GetInstance().GetSpecifications()) {
			String caseXML = ib.getCases(specification.GetYSpecID(), sessionIBHandle);
			XNode caseNode = parser.parse(caseXML);
			//Utilities.OutputNodeXML(caseNode);
			
			if (caseNode.getName().equals("response") && caseNode.getChildCount() > 0) {
				for (XNode child : caseNode.getChildren()) {
					if (child.getName().equals("caseID")) {
						String caseId = child.getText();
						if (SystemStorage.GetInstance().GetCase(caseId) == null) {
							SystemStorage.GetInstance().AddCase(new Case(caseId, specification.GetUID()));
						}
					}
				}
			}
		}		
	}
	
	public boolean CompleteWorkItem(WorkItem work) throws Exception {
		String msg = wq.completeItem(work.participant, work.workItemID, sessionWQHandle);
		System.out.println("CompleteWorkItem: " + msg);
		return wq.successful(msg);
	}
	
	public boolean DelegateWorkItem(WorkItem work, Agent delegateTo) throws Exception {
		String msg = wq.delegateItem(work.participant, delegateTo.GetUID(), work.workItemID, sessionWQHandle);
		System.out.println("DelegateWorkItem: " + msg);
		return wq.successful(msg);
	}
	
	public boolean SuspendWorkItem(WorkItem work) throws Exception {
		String msg = wq.suspendItem(work.participant, work.workItemID, sessionWQHandle);
		System.out.println("SuspendWorkItem: " + msg);
		return wq.successful(msg);
	}
	
	public boolean UnsuspendWorkItem(WorkItem work) throws Exception {
		String msg = wq.unsuspendItem(work.participant, work.workItemID, sessionWQHandle);
		System.out.println("SuspendWorkItem: " + msg);
		return wq.successful(msg);
	}
	
	public boolean StartWorkItem(WorkItem work, Agent agent) throws Exception {
		String msg = wq.startItem(agent.GetUID(), work.workItemID, sessionWQHandle);
		System.out.println("Started: " + msg);
		if(wq.successful(msg)) {
			work.participant = agent.GetUID();
			agent.UpdateQueues();
			return true;
		}
		return false;
	}
	
	public boolean AcceptWorkItem(WorkItem work, Agent agent) throws Exception {
		String msg = wq.acceptOffer(agent.GetUID(), work.workItemID, sessionWQHandle);
		
		if(wq.successful(msg)) {
			System.out.println("Accepted: " + msg);
			work.participant = agent.GetUID();
			agent.UpdateQueues();
			return true;
		} else {
			System.out.println("Accepted_but_failed: " + msg);
		}
		return false;
	}

	public boolean CancelWorkItem(WorkItem workItem) throws Exception {
		String msg = wq.cancelCase(workItem.caseID, sessionWQHandle);
		SystemStorage.GetInstance().GetAgent(workItem.participant).UpdateQueues();
		System.out.println("CancelWorkItem: " + msg);
		return wq.successful(msg);
	}
	
	public String LaunchCase(Specification specification) throws Exception
	{
		String msg = ib.launchCase(specification.GetYSpecID(), null, new YLogDataItemList(), sessionIBHandle);
		if (ib.successful(msg)) {
			ProcessCases();
			SyncAll();		
			return msg;
		}
		return null;
	}
	
	public boolean CancelCase(Case cCase) throws Exception {
		
		System.err.println("Cancelcase + cancel workitem");
		
		ArrayList<WorkItem> allWorkItemsOfTheCase = SystemStorage.GetInstance().GetWorkItemsByCaseId(cCase.GetID());
		for(WorkItem wi : allWorkItemsOfTheCase){
			CancelWorkItem(wi);
		}
		
		String msg = ib.cancelCase(cCase.GetID(), sessionIBHandle);
		wq.cancelCase(cCase.GetID(), sessionWQHandle);
		System.out.println("CancelCase: " + msg);
		if (ib.successful(msg)) {
			SystemStorage.GetInstance().RemoveCase(cCase);
			SyncAll();
			return true;
		}
		return false;
	}
	
	public boolean IsCaseCompleted(Case cCase) throws Exception {
		String exists = ib.getCaseState(cCase.GetID(), sessionIBHandle);
		if (!ib.successful(exists)) {
			// It does not exist, therefore it is completed
			if (exists.contains("not found")) {
				System.out.println("CaseCompleted: " + cCase.GetID());
				return true;
			}
		}
		return false;
	}
	
}

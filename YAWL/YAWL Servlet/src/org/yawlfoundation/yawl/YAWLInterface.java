package org.yawlfoundation.yawl;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.jdom2.Element;
import org.yawlfoundation.yawl.engine.YSpecificationID;
import org.yawlfoundation.yawl.engine.interfce.SpecificationData;
import org.yawlfoundation.yawl.engine.interfce.WorkItemRecord;
import org.yawlfoundation.yawl.engine.interfce.interfaceA.InterfaceA_EnvironmentBasedClient;
import org.yawlfoundation.yawl.engine.interfce.interfaceB.InterfaceBWebsideController;
import org.yawlfoundation.yawl.logging.YLogDataItemList;
import org.yawlfoundation.yawl.resourcing.resource.Capability;
import org.yawlfoundation.yawl.resourcing.resource.Participant;
import org.yawlfoundation.yawl.resourcing.resource.Role;
import org.yawlfoundation.yawl.resourcing.rsInterface.ResourceGatewayClientAdapter;
import org.yawlfoundation.yawl.resourcing.rsInterface.ResourceGatewayException;
import org.yawlfoundation.yawl.resourcing.rsInterface.WorkQueueGatewayClientAdapter;
import org.yawlfoundation.yawl.util.JDOMUtil;
import org.yawlfoundation.yawl.util.XNode;
import org.yawlfoundation.yawl.util.XNodeParser;

public class YAWLInterface extends InterfaceBWebsideController {

	public static YAWLInterface Instance;
	
	protected final String name = "yawlRelay";
	protected final String password = "password";

	protected InterfaceA_EnvironmentBasedClient interfaceA = new InterfaceA_EnvironmentBasedClient(
			"http://localhost:8080/yawl/ia");
	protected ResourceGatewayClientAdapter resourceGateway = new ResourceGatewayClientAdapter(
			"http://localhost:8080/resourceService/gateway");
	protected WorkQueueGatewayClientAdapter workQueueGateway = new WorkQueueGatewayClientAdapter(
			"http://localhost:8080/resourceService/workqueuegateway");

	protected String handleA;
	protected String handleB;
	protected String handleRG;
	protected String handleWQG;
	
	protected XNodeParser parser = new XNodeParser();
	
	public YAWLInterface() {
		if (Instance == null) {
			Instance = this;			
		}
		try {
			ensureConnection();
		} catch (Exception e) {
			Application.Log(e.getMessage());
		}
	}

	// CASE METHODS

	public synchronized List<String> GetAllSpecifications() {
		List<String> messageList = new ArrayList<String>();
		try {
			for (SpecificationData spec : _interfaceBClient.getSpecificationList(handleB)) {
				YSpecificationID id = spec.getID();
				messageList.add("SPECIFICATION " + id.getIdentifier() + " " + 
						id.getVersionAsString() + " " + id.getKey() + " "+ id.getUri());
			}
		} catch (Exception e) {
			System.out.println(e);
		}
		return messageList;
	}

	public synchronized List<String> GetCases() {
		return null;

	}

	public synchronized List<String> LaunchCase(String inputLine) {
		List<String> messageList = new ArrayList<String>();
		System.out.println("1");
		String identifier = inputLine.split(" ")[1];
		String version = inputLine.split(" ")[2];
		String uri = inputLine.split(" ")[3];
		YSpecificationID ySpec = new YSpecificationID(identifier, version, uri);
		String response = "";
		try {
			System.out.println("2");
			response = _interfaceBClient.launchCase(ySpec, null, new YLogDataItemList(), handleB);
			System.out.println("3");
			if (_interfaceBClient.successful(response)) {
				messageList.add("CASE " + uri + " " + identifier);
				System.out.println("4");
			}
			messageList.add("CASE " + uri + " " + identifier);
		} catch (Exception e) {
			e.printStackTrace();
		}		
		
		return messageList;
	}
	
	private Boolean isCaseCompleted(String caseIdentifier) {
		try {
			String caseState = _interfaceBClient.getCaseState(caseIdentifier, handleB);
			System.out.println(caseState);
			System.out.println(_interfaceBClient.successful(caseState));
			return (caseState.contains("not found") && !_interfaceBClient.successful(caseState));
		} catch (Exception e) {
			e.printStackTrace();
		}
		return false;
	}
	
	private String completeCase(String caseIdentifier, String caseSpecificationID) {
		return "CASEEND " + caseIdentifier + " " + caseSpecificationID;
	}

	public synchronized List<String> CancelCase(String inputLine) {
		return null;

	}

	public synchronized List<String> CancelAllCases(String inputLine) {
		return null;

	}

	// AGENT METHODS

	public synchronized List<String> GetActiveAgents() {
		List<String> messageList = new ArrayList<String>();
		try {
			for (Participant participant : getAgents()) {
				String userID = participant.getID();
				messageList.add("ACTIVEAGENT " + participant.getName() + " " + userID);
			}
		} catch (Exception e) {
			System.out.println(e);
		}
		return messageList;
	}

	public synchronized List<String> GetAllAgents() {
		List<String> messageList = new ArrayList<String>();
		try {
			for (Participant participant : getAgents()) {
				String userID = participant.getID();
				messageList.add("AGENT " + participant.getName() + " " + userID);
				
				for (Capability capability : participant.getCapabilities()) {
					messageList.add("AGENTCAPABILITY " + userID + " " 
							+ capability.getCapability());
				}
				
				for (Role role : participant.getRoles()) {
					messageList.add("AGENTROLE " + userID + " " 
							+ role.getName());
				}
			}
		} catch (Exception e) {
			System.out.println(e);
		}
		return messageList;
	}
	
	private List<Participant> getAgents() throws Exception {
		return resourceGateway.getParticipants(handleRG);
	}

	// WORK ITEM METHODS

	public synchronized List<String> GetTaskQueue(String inputLine) {
		List<String> messageList = new ArrayList<String>();
		int queueID = Integer.parseInt(inputLine.split(" ")[1]);
		String participant = inputLine.split(" ")[2];
		
		try {
			for (WorkItemRecord workItemRecord : (workQueueGateway.getQueuedWorkItems(participant, queueID, handleWQG))) {
				String caseID = workItemRecord.getCaseID();
				String specificationID = workItemRecord.getSpecIdentifier();
				String uniqueID = workItemRecord.getUniqueID();
				String workItemID = workItemRecord.getID();
				String tasks = "";
				String taskID = workItemRecord.getTaskID();
				String goals = "";
				
				XNode specDataXML = parser.parse(_interfaceBClient.getTaskInformationStr(new YSpecificationID(workItemRecord), taskID, handleB));
				try {
					List<XNode> outputParams = specDataXML.getChild("taskInfo").getChild("params").getChildren("outputParam");
					for(XNode param : outputParams) {
						if (param.getChild("name").getText().equals("Tasks")) {
							tasks = param.getChild("defaultValue").getText();
						}
						else if (param.getChild("name").getText().equals("Goal")) {
							goals = param.getChild("defaultValue").getText();
						}
					}
				} catch (Exception e) {
					System.out.println("Malformed task info. " + e);
				}
				
				messageList.add(String.format("WORKITEM %s %s %s %s %s %s %s %s %s", 
						caseID, specificationID, uniqueID, workItemID, taskID, participant, queueID, tasks, goals));
			}
		} catch (Exception e) {
			System.out.println(e);
		}
		
		return messageList;
	}
	
	public synchronized void PrintAllTaskQueues() {
		try {
			String[] queues = { "0", "1", "2", "3", "4", "5" };
			for (Participant participant : getAgents()) {
				String userID = participant.getID();
				for (String queue : queues) {
					String inputLine = String.format("%s %s %s", "GetTaskQueue", queue, userID);
					for (String message : GetTaskQueue(inputLine)) {
						Application.Log(message);
					}
				}
				
			}
		} catch (Exception e) {
			Application.Log(e.getMessage());
		}
		
	}

	public synchronized List<String> GetAllWorkItems() {
		return null;

	}

	public synchronized List<String> WorkItemAction(String inputLine) {
		List<String> messageList = new ArrayList<String>();
		
		try {
			String action = inputLine.split(" ")[1];
			String agentID = inputLine.split(" ")[2];
			String workItemID = inputLine.split(" ")[3];
			String caseIdentifier = inputLine.split(" ")[4].split("\\.")[0];
			String caseSpecificationID = inputLine.split(" ")[5];
			
			if (action.equalsIgnoreCase("Complete")) {
				if (completeWorkItem(agentID, workItemID)) {
					messageList.add(EndTask(agentID, workItemID));
					if (isCaseCompleted(caseIdentifier)) {
						messageList.add(completeCase(caseIdentifier, caseSpecificationID));
					}
				}
				
			} else if (action.equalsIgnoreCase("CheckIn")) {
				
			}
			
		} catch (Exception e) {
			Application.Log(e.getMessage());
		}
		
		return messageList;
	}
	
	protected synchronized boolean CheckInWorkItem(String inputLine) {
		return true;
		
//		Application.Log(_interfaceBClient.checkInWorkItem(record.getID(), JDOMUtil.elementToString(getOutputData(record.getTaskID(), "result")), null, handleB));

	}
	
	protected Boolean completeWorkItem(String agentID, String workItemID) throws IOException, ResourceGatewayException {
		try {
			String success = workQueueGateway.completeItem(agentID, workItemID, handleWQG);
			return (success.equals("<success/>"));
		} catch (Exception e) {
			e.printStackTrace();
		}
		return false;		
	}
	
	protected static String EndTask(String agentID, String workItemID) {
		return "TASKEND " + agentID + " " + workItemID;
	}
	
	
	
	
	
	
	
	
	
	
	
	/**************************************************************************
	 * 
	 * 
	 * YAWL Custom Service override methods
	 * 
	 * 
	 **************************************************************************/
	
	
	
	public void handleStartCaseEvent(YSpecificationID specID, String caseID, String launchingService, boolean delayed) {
		Application.Log(String.format("CASE STARTED: %s, by: %s", caseID, launchingService));
		ensureConnection();
		Application.Log(handleB);
	}
	
	public void handleCancelledCaseEvent(String caseID)  {
		Application.Log("CASE CANCELLED");
	}
	
	public void handleCompleteCaseEvent(String caseID, String casedata) {
		Application.Log("CASE COMPLETED");
	}

	public void handleEnabledWorkItemEvent(WorkItemRecord record) {
		try {
			ensureConnection();
			
			
//			record = checkOut(record.getID(), handleB);

			Application.Log("Work item enabled: " + record.getTaskID());
			Application.Log(record.toXML());
//			Application.Log(_interfaceBClient.checkInWorkItem(record.getID(), JDOMUtil.elementToString(getOutputData(record.getTaskID(), "result")), null, handleB));
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	protected void ensureConnection() {
		try {
			if (handleB == null || !checkConnection(handleB)) {
				handleB = connect(name, password);
			}
			if (handleA == null || !interfaceA.checkConnection(handleA).equals("true")) {
				Application.Log("interfaceA.checkConnection() == " + interfaceA.checkConnection(handleA));
				handleA = interfaceA.connect(name, password);
			}
			if (handleRG == null || !resourceGateway.checkConnection(handleRG)) {
				Application.Log("resourceGateway.checkConnection() != true");
				handleRG = resourceGateway.connect(name, password);
			}
			if (handleWQG == null || !workQueueGateway.checkConnection(handleWQG)) {
				Application.Log("workQueueGateway.checkConnection() != true");
				handleWQG = workQueueGateway.connect(name, password);
			}
//			if (handleRG == null || !checkConnection(handleB)) {
//				handleRG = resourceGateway.connect(name, password);
//			}
//			if (handleWQG == null || !checkConnection(handleB)) {
//				handleWQG = workQueueGateway.connect(name, password);
//			}
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	protected Element getOutputData(String taskName, String data) {
		return new Element(taskName);
	}
	
	public synchronized void LaunchCase() {
		YSpecificationID ySpec = new YSpecificationID("UID_0ad53bd4-6f76-4bd5-86e3-197be62c26e5", "0.2", "TramaCentreA_NEW");
		try {
			_interfaceBClient.launchCase(ySpec, null, handleB, new YLogDataItemList(), _interfaceBClient.getBackEndURI());
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	public void handleEngineInitialisationCompletedEvent() { }

	public void handleCancelledWorkItemEvent(WorkItemRecord record) { }
}

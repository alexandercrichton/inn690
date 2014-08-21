package main;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.yawlfoundation.yawl.engine.YSpecificationID;
import org.yawlfoundation.yawl.engine.interfce.SpecificationData;
import org.yawlfoundation.yawl.engine.interfce.WorkItemRecord;
import org.yawlfoundation.yawl.engine.interfce.interfaceA.InterfaceA_EnvironmentBasedClient;
import org.yawlfoundation.yawl.engine.interfce.interfaceB.InterfaceB_EnvironmentBasedClient;
import org.yawlfoundation.yawl.logging.YLogDataItemList;
import org.yawlfoundation.yawl.resourcing.resource.Capability;
import org.yawlfoundation.yawl.resourcing.resource.Participant;
import org.yawlfoundation.yawl.resourcing.resource.Role;
import org.yawlfoundation.yawl.resourcing.rsInterface.ResourceGatewayClientAdapter;
import org.yawlfoundation.yawl.resourcing.rsInterface.ResourceGatewayException;
import org.yawlfoundation.yawl.resourcing.rsInterface.WorkQueueGatewayClientAdapter;
import org.yawlfoundation.yawl.util.XNode;
import org.yawlfoundation.yawl.util.XNodeParser;

public class YAWLInterface {

	protected static final String LOGIN = "admin";
	protected static final String PASSWORD = "YAWL";

	protected static InterfaceA_EnvironmentBasedClient interfaceA;
	protected static InterfaceB_EnvironmentBasedClient interfaceB;
	protected static ResourceGatewayClientAdapter resourceGateway;
	protected static WorkQueueGatewayClientAdapter workQueueGateway;

	protected static String handleInterfaceA;
	protected static String handleInterfaceB;
	protected static String handleResourceGateway;
	protected static String handleWorkQueueGateway;
	
	protected static XNodeParser parser = new XNodeParser();

	static {
		interfaceA = new InterfaceA_EnvironmentBasedClient(
				"http://localhost:8080/yawl/ia");
		interfaceB = new InterfaceB_EnvironmentBasedClient(
				"http://localhost:8080/yawl/ib");
		resourceGateway = new ResourceGatewayClientAdapter(
				"http://localhost:8080/resourceService/gateway");
		workQueueGateway = new WorkQueueGatewayClientAdapter(
				"http://localhost:8080/resourceService/workqueuegateway");

		try {
			handleInterfaceA = interfaceA.connect(LOGIN, PASSWORD);
			handleInterfaceB = interfaceB.connect(LOGIN, PASSWORD);
			handleResourceGateway = resourceGateway.connect(LOGIN, PASSWORD);
			handleWorkQueueGateway = workQueueGateway.connect(LOGIN, PASSWORD);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public static synchronized List<String> SyncAll() {
		return null;

	}

	// CASE METHODS

	public static synchronized List<String> GetAllSpecifications() {
		List<String> messageList = new ArrayList<String>();
		try {
			for (SpecificationData spec : interfaceB.getSpecificationList(handleInterfaceB)) {
				YSpecificationID id = spec.getID();
				messageList.add("SPECIFICATION " + id.getIdentifier() + " " + 
						id.getVersionAsString() + " " + id.getKey() + " "+ id.getUri());
			}
		} catch (Exception e) {
			System.out.println(e);
		}
		return messageList;
	}

	public static synchronized List<String> GetCases() {
		return null;

	}

	public static synchronized List<String> LaunchCase(String inputLine) {
		List<String> messageList = new ArrayList<String>();
		System.out.println("1");
		String identifier = inputLine.split(" ")[1];
		String version = inputLine.split(" ")[2];
		String uri = inputLine.split(" ")[3];
		YSpecificationID ySpec = new YSpecificationID(identifier, version, uri);
		String response = "";
		try {
			System.out.println("2");
			response = interfaceB.launchCase(ySpec, null, new YLogDataItemList(), handleInterfaceB);
			System.out.println("3");
			if (interfaceB.successful(response)) {
				messageList.add("CASE " + uri + " " + identifier);
				System.out.println("4");
			}
			messageList.add("CASE " + uri + " " + identifier);
		} catch (Exception e) {
			e.printStackTrace();
		}		
		
		return messageList;
	}
	
	private static Boolean isCaseCompleted(String caseIdentifier) {
		try {
			String caseState = interfaceB.getCaseState(caseIdentifier, handleInterfaceB);
			System.out.println(caseState);
			System.out.println(interfaceB.successful(caseState));
			return (caseState.contains("not found") && !interfaceB.successful(caseState));
		} catch (Exception e) {
			e.printStackTrace();
		}
		return false;
	}
	
	private static String completeCase(String caseIdentifier, String caseSpecificationID) {
		return "CASEEND " + caseIdentifier + " " + caseSpecificationID;
	}

	public static synchronized List<String> CancelCase(String inputLine) {
		return null;

	}

	public static synchronized List<String> CancelAllCases(String inputLine) {
		return null;

	}

	// AGENT METHODS

	public static synchronized List<String> GetActiveAgents() {
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

	public static synchronized List<String> GetAllAgents() {
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
	
	private static List<Participant> getAgents() throws IOException, ResourceGatewayException {
		return resourceGateway.getParticipants(handleResourceGateway);
	}

	// WORK ITEM METHODS

	public static synchronized List<String> GetTaskQueue(String inputLine) {
		List<String> messageList = new ArrayList<String>();
		int queueID = Integer.parseInt(inputLine.split(" ")[1]);
		String participant = inputLine.split(" ")[2];
		
		try {
			for (WorkItemRecord workItemRecord : (workQueueGateway.getQueuedWorkItems(participant, queueID, handleWorkQueueGateway))) {
				String caseID = workItemRecord.getCaseID();
				String specificationID = workItemRecord.getSpecIdentifier();
				String uniqueID = workItemRecord.getUniqueID();
				String workItemID = workItemRecord.getID();
				String tasks = "";
				String taskID = workItemRecord.getTaskID();
				String goals = "";
				
				XNode specDataXML = parser.parse(interfaceB.getTaskInformationStr(new YSpecificationID(workItemRecord), taskID, handleInterfaceB));
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

	public static synchronized List<String> GetAllWorkItems() {
		return null;

	}

	public static synchronized List<String> WorkItemAction(String inputLine) throws IOException, ResourceGatewayException {
		List<String> messageList = new ArrayList<String>();
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
		}

		
		return messageList;
	}
	
	protected static Boolean completeWorkItem(String agentID, String workItemID) throws IOException, ResourceGatewayException {
		try {
			String success = workQueueGateway.completeItem(agentID, workItemID, handleWorkQueueGateway);
			return (success.equals("<success/>"));
		} catch (Exception e) {
			e.printStackTrace();
		}
		return false;		
	}
	
	protected static String EndTask(String agentID, String workItemID) {
		return "TASKEND " + agentID + " " + workItemID;
	}
}

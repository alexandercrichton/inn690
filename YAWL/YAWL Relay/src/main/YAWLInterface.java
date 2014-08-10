package main;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.xml.sax.*;
import org.yawlfoundation.yawl.engine.YSpecificationID;
import org.yawlfoundation.yawl.engine.interfce.SpecificationData;
import org.yawlfoundation.yawl.engine.interfce.WorkItemRecord;
import org.yawlfoundation.yawl.engine.interfce.interfaceA.InterfaceA_EnvironmentBasedClient;
import org.yawlfoundation.yawl.engine.interfce.interfaceB.InterfaceB_EnvironmentBasedClient;
import org.yawlfoundation.yawl.resourcing.resource.*;
import org.yawlfoundation.yawl.resourcing.rsInterface.ResourceGatewayClient;
import org.yawlfoundation.yawl.resourcing.rsInterface.ResourceGatewayClientAdapter;
import org.yawlfoundation.yawl.resourcing.rsInterface.ResourceGatewayException;
import org.yawlfoundation.yawl.resourcing.rsInterface.WorkQueueGatewayClient;
import org.yawlfoundation.yawl.resourcing.rsInterface.WorkQueueGatewayClientAdapter;

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

	public static List<String> SyncAll() {
		return null;

	}

	// CASE METHODS

	public static List<String> GetAllSpecifications() {
		List<String> messageList = new ArrayList();
		try {
			for (SpecificationData spec : interfaceB
					.getSpecificationList(handleInterfaceB)) {
				YSpecificationID id = spec.getID();
				messageList.add("SPECIFICATION " + id.getKey() + " "
						+ id.getUri());
			}
		} catch (Exception e) {
			System.out.println(e);
		}
		return messageList;
	}

	public static List<String> GetCases() {
		return null;

	}

	public static List<String> LaunchCase(String inputLine) {
		return null;

	}

	public static List<String> CancelCase(String inputLine) {
		return null;

	}

	public static List<String> CancelAllCases(String inputLine) {
		return null;

	}

	// AGENT METHODS

	public static List<String> GetActiveAgents() {
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

	public static List<String> GetAllAgents() {
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

	public static List<String> GetTaskQueue(String inputLine) {
		List<String> messageList = new ArrayList<String>();
		int queueID = Integer.parseInt(inputLine.split(" ")[1]);
		String participant = inputLine.split(" ")[2];
		try {
			for (WorkItemRecord str : (workQueueGateway.getQueuedWorkItems(participant, queueID, handleWorkQueueGateway))) {
				System.out.println(str.toString());				
			}
		} catch (Exception e) {
			System.out.println(e);
		}
		return messageList;
	}

	public static List<String> WorkItemAction(String inputLine) {
		return null;

	}

	public static List<String> GetAllWorkItems() {
		return null;

	}
}

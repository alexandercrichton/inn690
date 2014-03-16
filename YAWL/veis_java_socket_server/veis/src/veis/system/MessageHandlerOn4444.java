package veis.system;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.util.ArrayList;
//import java.util.Collection;


import main.Application;

import veis.entities.workflow.Agent;
import veis.entities.workflow.Case;
import veis.entities.workflow.Specification;
import veis.entities.workflow.WorkItem;
import veis.system.SystemStorage;
import veis.yawl.YAWLComm;

/**
 * This thread will be created when a client connect on port 4444
 */

public class MessageHandlerOn4444 extends Thread {
	private Socket clientSocket = null;
	private PrintWriter out = null;
	
	public MessageHandlerOn4444(Socket connectedSocket) {
		clientSocket = connectedSocket;
		if (clientSocket != null) {
			start();
		} else {
			System.err.println("Given socket was null");
		}
	}
	
	public void Send(String message) {
		if(clientSocket != null && clientSocket.isConnected() && out != null) {
			out.println(message);
			Application.Log("SEND: " + message);
		}
	}
    
	public void run() {
		System.out.println("A client connected on port " + AgentReceiverSocketServer.SERVER_SOCKET + AgentReceiverSocketServer.getCurrentConnections());
    	try {
	        out = new PrintWriter(clientSocket.getOutputStream(), true);
	        BufferedReader in = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));
	        boolean shutdownByClient = false;
	        Send("Aloha, type HELP for instruction.");
	        // Reads messages received from the visualization client, until an error occurs, or it receives
	        // a message telling it to stop listening (EndSession OR EndSimulation)
	        while(!clientSocket.isInputShutdown() && !clientSocket.isOutputShutdown() || !clientSocket.isClosed() && clientSocket.isConnected()) {
	        	if(in.ready()) {
	        		String inputLine = in.readLine();
	        		
	        		//This loop will delete the character in front of #7
	        		//This will make the system easier to interact by Telnet
	        		//The loop has been removed
	        		
	        		//System.out.println("RECV: " + inputLine);
	        		Application.Log("RECV: " + inputLine);
	        		
	        		String command = inputLine.split(" ")[0];
	        		
	        		if(command.equalsIgnoreCase("Help")){
	        			SayHelp();
	        		} else if(command.equalsIgnoreCase("Stat")){
	        			ShowStat();
	        		} else if(command.equalsIgnoreCase("GetActiveAgents")) {
	        			GetActiveAgents();
	        		} else if(command.equalsIgnoreCase("GetAllAgents")) {
	        			GetAllAgents();
	        		} else if (command.equalsIgnoreCase("GetTaskQueue")) {
	        			GetTaskQueue(inputLine);
	        		} else if (command.equalsIgnoreCase("WorkItemAction")) {
	        			WorkItemAction(inputLine);
	        		} else if(command.equalsIgnoreCase("LaunchCase")) {
	        			LaunchCase(inputLine);
	        		} else if(command.equalsIgnoreCase("SyncAll")) {
	        			SyncAll();
	        		} else if(command.equalsIgnoreCase("GetCases")) {
	        			GetCases();
	        		} else if(command.equalsIgnoreCase("GetAllSpecifications")) {
	        			GetAllSpecifications();
	        		} else if(command.equalsIgnoreCase("GetAllWorkItems")){
	        			GetAllWorkItems();
	        		} else if(command.equalsIgnoreCase("CancelCase")) {
	        			CancelCase(inputLine);
	        		} else if(command.equalsIgnoreCase("CancelAllCases")) {
	        			CancelAllCases(inputLine);
	        		} else if(command.equalsIgnoreCase("Shutdown")){
	        			shutdownByClient = true; break;
	        		} else if(command.equalsIgnoreCase("EndSession") || command.equalsIgnoreCase("EndSimulation") || command.equalsIgnoreCase("exit") || command.equalsIgnoreCase("quit")) {
	        			break;
	        		} else Send("\nUnknown command: " + inputLine + ". Type HELP for instruction.");
	        	}
	        	
	        	Thread.yield();
	        }
	        
	        if(shutdownByClient){
	        	Send("Server is shutting down...");
	        	System.out.print("Server shutdown request from client.");
	        }
            out.close();
            out = null;
	        in.close();
	        clientSocket.close();
	        
	        if(shutdownByClient)
    			System.exit(0);
	        
	        
    	} catch (Exception e) {
    		System.err.println("[AClientOn4444] Agent Visualisation disconnected unexpectedly");
    		e.printStackTrace();
    		//try { clientSocket.close(); } catch (IOException e1) { }
    	}
    	
    	AgentReceiverSocketServer.allClients.remove(this);
    	System.out.println("A client has been disconnected." + AgentReceiverSocketServer.getCurrentConnections());
	}
	
	
	private void SayHelp(){
		Send("Veis socket server listen on port " + clientSocket.getLocalPort());
		Send("   Help");
		Send("   Stat");
		//Send("\tShow this screen.");
		Send("   Quit or Exit");
		//Send("\tClose the client connection, server still running.");
		Send("   Shutdown");
		Send("\tClose the client connection, then shutdown the server.");
		Send("   LaunchCase {specificationID}");
		//Send("\tLauch a specification by its ID");
		Send("   CancelCase {caseID}");
		Send("   CancelAllCases {caseID}");
		//Send("\tCancel a running case by its ID");
		Send("   SyncAll");
		Send("   GetAllSpecifications");
		Send("   GetCases");
		//Send("\tGet all running cases.");
		Send("   GetActiveAgents");
		//Send("\tGet all active agents.");
		Send("   GetAllAgents");
		Send("   GetAllWorkItems");
		//Send("\tGet all agents in the system.");
		Send("   GetTaskQueue {queueId} {participant}");
		//Send("\tGet all task in queue.");
		Send("   WorkItemAction {action} {agentId} {taskId}");
		Send("\tAction: Complete, Suspend, Unsuspend, Delegate, Accept");		
	}
	
	private void SyncAll() throws Exception{
		YAWLComm.GetInstance().SyncAll();
	}
	
	private void ShowStat() throws Exception{
		Send(SystemStorage.GetInstance().toString());
	}
	
	private void LaunchCase(String inputLine) throws Exception {
		if (inputLine.split(" ").length >= 2) {
			// Find the specification it's referring to
			String specificationID = inputLine.split(" ")[1];
			Specification specification = SystemStorage.GetInstance().GetSpecificationByName(specificationID);
			//Specification specification = SystemStorage.GetInstance().GetSpecification(specificationID);
			if (specification != null) {
				// Launch a case through the YAWLComm
				String caseId = YAWLComm.GetInstance().LaunchCase(specification);
				// Start planning and assigning work items from that task to agents
				if (caseId != null) {
					String message = "CASE " + caseId + " " + specificationID;
					AgentReceiverSocketServer.SendToAllClients(message);
					//Send(message);
				} else {
					Send("FAIL_TO_LAUCH_CASE");
				}
			} else {
				Send("NO_CASE_FOUND");
			}
		} else {
			Send("LaunchCase needs {specificationID}.");
		}
	}
	
	private void GetAllSpecifications() throws Exception{
		
		//System.out.println("GetAllSpecifications Requested.");
		
		ArrayList<Specification> specs = SystemStorage.GetInstance().GetSpecifications();
		
		if(specs.size() == 0) Send("NO_SPEC_FOUND");
		else 
		for(int i = 0; i < specs.size(); i ++){
			Send("SPECIFICATION " + specs.get(i).GetUID()+ " " + specs.get(i).GetURI());
		}
	}
	
	private void GetCases() throws Exception{
		ArrayList<Case> cases = SystemStorage.GetInstance().GetCases();
		
		if(cases.size() == 0) Send("NO_CASE_FOUND");
		else
		for(int i = 0; i < cases.size(); i ++){
			Send("RUNNINGCASE " + cases.get(i).GetID() + " " + SystemStorage.GetInstance().GetSpecification(cases.get(i).GetSpecificationID()).GetURI() );
		}
	}
	
	private void CancelCase(String inputLine) throws Exception{
		if (inputLine.split(" ").length >=2) {
			String caseID = inputLine.split(" ")[1];
			Case cCase = SystemStorage.GetInstance().GetCase(caseID);
			if (cCase != null) {
				YAWLComm.GetInstance().CancelCase(cCase);
			} else {
				Send("NO_CASE_FOUND");
			}
		}
	}
	
	private void CancelAllCases(String inputLine) throws Exception{
		
		ArrayList<Case> cases = SystemStorage.GetInstance().GetCases();
		if(cases.size() == 0) { Send("NO_CASE_FOUND"); }
		else
		for(int i = 0; i < cases.size(); i++){
			Case c = cases.get(i);
			YAWLComm.GetInstance().CancelCase(c);
		}
		YAWLComm.GetInstance().SyncAll();
	}
	
	private void GetActiveAgents() throws Exception {
		
		//System.out.println("GeActiveAgents Requested.");
		
		ArrayList<Agent> agents = SystemStorage.GetInstance().GetAgents();
		
		if(agents.size() == 0) Send("NO_AGENT_FOUND");
		else
		for(Agent a : agents) {
		//for(Agent a : SystemStorage.GetInstance().GetActiveAgents()) {
			String reply = "ACTIVEAGENT " + a.GetFirstName() + " " + a.GetSecondName() + " " + a.GetUID();
			Send(reply);
		}
	}
	
	private void GetAllAgents() throws Exception {
		
		//System.out.println("GetAllAgents Requested.");
		
		ArrayList<Agent> agents = SystemStorage.GetInstance().GetAgents();
		if(agents.size() == 0) Send("NO_AGENT_FOUND");
		else
		for(Agent a : agents) {
			String reply = "AGENT " + a.GetFirstName() + " " + a.GetSecondName() + " " + a.GetUID();
			Send(reply);

			for(String cap : a.GetCapabilities()) {
				Send("AGENTCAPABILITY " + a.GetUID() + " " + cap);
			}

			for(String role : a.GetRoles()) {
				Send("AGENTROLE " + a.GetUID() + " " + role);
			}
		}
	}
	
	private void GetTaskQueue(String inputLine) throws Exception{
		if(inputLine.split(" ").length == 3) {
			YAWLComm.GetInstance().ProcessWorkItems(); // Make sure this application is synched with YAWL
			String queueID = inputLine.split(" ")[1];
			String participant = inputLine.split(" ")[2];
    		if(SystemStorage.GetInstance().GetAgent(participant) != null) {
				for (WorkItem a : SystemStorage.GetInstance().GetAgent(participant).GetQueue(Agent.QueueState.valueOf(queueID))) {
					Send("WORKITEM " + queueID + " " + a.participant + " " + a.uniqueID + " " + a.tasks + " " + a.taskID + " " + a.goals);
					// Send("WORKITEMNAME " + a.uniqueID + " " + a.taskID);
				}
			} else Send("NO_TASK_FOUND");
		}
	
	}
	
	private void GetAllWorkItems() throws Exception{
		ArrayList<WorkItem> allWorkItems = SystemStorage.GetInstance().GetWorkItems();
		
		if(allWorkItems.size() > 0){
			for(WorkItem wi : allWorkItems){
				Send("WORKITEM " + wi.goals + " " + wi.caseID + " " +wi.status );
			}
		} else Send("NO_WORKITEM_FOUND");
	}
	
	/**
	 * Process work item by client's message (Opensim maybe). Possible messages:
	 * Complete
	 * Suspend
	 * Un suspend
	 * Delegate
	 * Accept
	 * @param inputLine
	 * @throws Exception
	 */
	private void WorkItemAction(String inputLine) throws Exception {
		if(inputLine.split(" ").length >= 4) {
			String action = inputLine.split(" ")[1];
			String agentID = inputLine.split(" ")[2];
			String taskID = inputLine.split(" ")[3];
			
			Agent agent = SystemStorage.GetInstance().GetAgent(agentID);
			WorkItem work = SystemStorage.GetInstance().GetWorkItem(taskID);
			
			if (agent != null && work != null) {
				if (work.participant.equals(agentID)){
					if (action.equals("Complete")) {
    					if(work.Complete()) {
    						YAWLComm.GetInstance().ProcessWorkItems();
	        				Send("TASKEND " + agentID + " " + taskID);
	        				// Check if the case has been completed with the completion of this task
	        				if (YAWLComm.GetInstance().IsCaseCompleted(SystemStorage.GetInstance().GetCase(work.caseID.split("\\.")[0]))){
	        					Send("CASEEND " + work.caseID.split("\\.")[0] + " " + work.specID);
	        				}
    					}
					} else if (action.equals("Suspend")) {
						if(work.Suspend()) {
							YAWLComm.GetInstance().ProcessWorkItems();
	        				Send("SUSPEND " + agentID + " " + taskID);				        				
    					}
					} else if (action.equals("Unsuspend")) {
						if(work.Unsuspend()) {
							YAWLComm.GetInstance().ProcessWorkItems();
							Send("UNSUSPEND " + agentID + " " + taskID);					        				
    					}
					} else if (action.equals("Delegate") && inputLine.split(" ").length >= 5) {
						if(work.Suspend()) {
							String newAgentID = inputLine.split(" ")[4];
							Agent newAgent = SystemStorage.GetInstance().GetAgent(newAgentID);
	        				
	        				if(newAgent != null) {
	        					if(work.Delegate(newAgent)) {
	        						YAWLComm.GetInstance().ProcessWorkItems();
	        						Send("TASKEND " + agentID + " " + taskID);
			        				Send("WORKITEM " + newAgent + " " + taskID + " " + work.tasks + " " + work.taskID);							        				
	        					}
	        				}
    					}
					}
				} else if (action.equals("Accept") && work.participant == "") {
					work.Accept(agent);
				}
			}
		}
	}
}

package veis.system;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;
import java.util.Map.Entry;

import veis.entities.workflow.Agent;
import veis.entities.workflow.Case;
import veis.entities.workflow.Specification;
import veis.entities.workflow.WorkItem;
import veis.system.SystemStorage;

/**
 * This class works as a internal database server which store data from YAWL server
 */

public class SystemStorage {
	private Map<String, Specification> specs;
	private Map<String, Agent> agents;
	private Map<String, WorkItem> workitems;
	private ArrayList<Case> cases;
	
	private static SystemStorage instance = null;
	public static SystemStorage GetInstance() throws Exception {
		if (instance == null) instance = new SystemStorage();
		return instance;
	}
	
	private SystemStorage() throws Exception {
		if (instance != null) throw new Exception("Cannot have more than one SystemStorage!");
		
		specs = new HashMap<String, Specification>();
		cases = new ArrayList<Case>();
		agents = new HashMap<String, Agent>();
		workitems = new HashMap<String, WorkItem>();
	}
	
	public void AddSpecification(Specification spec) {
		specs.put(spec.GetUID(), spec);
	}
	
	public Specification GetSpecification(String UID) {
		return specs.get(UID);
	}
	
	public Specification GetSpecificationByName(String uri) {
		for (Specification spec : specs.values()) {
			if (spec.GetURI().equalsIgnoreCase(uri)) {
				return spec;
			}
		}
		return null;
	}
	
	public ArrayList<Specification> GetSpecifications() {
		ArrayList<Specification> retval = new ArrayList<Specification>();
		
		Set<Entry<String, Specification>> kkk = specs.entrySet();
		
		for(Entry<String, Specification> ggg : kkk) {
			retval.add(ggg.getValue());
		}
				
		return retval;
	}
	
	public String GetSpecificationNames(String seperator){
		ArrayList<Specification> specs = GetSpecifications();
		
		String output = "";
		
		for(int i = 0; i < specs.size(); i ++){
			output += specs.get(i).GetURI();
			if(i < specs.size() - 1) output += seperator;
		}
		
		return "(" + output + ")";
	}
	
	public void AddCase(Case iCase) {
		cases.add(iCase);
	}
	
	public Case GetCase(String caseID) {
		for(Case cCase : cases) {
			if(cCase.GetID().equals(caseID)) {
				return cCase;
			}
		}
		
		return null;
	}
	
	public void RemoveCase(Case cCase) {
		if (cases.contains(cCase)) {
			cases.remove(cCase);
		}
	}
	
	public Case GetLatestCase(Specification specification) {
		Integer latest = -1;
		for (Case cCase : cases) {
			if (cCase.GetSpecificationID().equals(specification.GetUID())) {
				if (Integer.parseInt(cCase.GetID()) > latest) {
					latest = Integer.parseInt(cCase.GetID());
				}
			}
		}
		return GetCase(latest.toString());
	}
	
	public ArrayList<Case> GetCases() {
		return cases;
	}
	
	public String GetCaseNames(String seperator){
		ArrayList<Case> cases = GetCases();
		
		String output = "";
		
		for(int i = 0; i < cases.size(); i ++){
			output += GetSpecification(cases.get(i).GetSpecificationID()).GetURI();
			if(i < cases.size() - 1) output += seperator;
		}
		
		return "(" + output + ")";
	}
	
	public void AddWorkItem(WorkItem spec) {
		workitems.put(spec.uniqueID, spec);
	}
	
	public WorkItem GetWorkItem(String UID) {
		return workitems.get(UID);
	}
	
	public void RemoveWorkItem(WorkItem work) {
		if(workitems.containsKey(work.uniqueID)) {
			workitems.remove(work.uniqueID);
		}
	}
	
	public ArrayList<WorkItem> GetWorkItems() {
		ArrayList<WorkItem> retval = new ArrayList<WorkItem>();
		
		Set<Entry<String, WorkItem>> kkk = workitems.entrySet();
		
		for(Entry<String, WorkItem> ggg : kkk) {
			retval.add(ggg.getValue());
		}
				
		return retval;
	}
	
	public ArrayList<WorkItem> GetWorkItemsByCaseId(String caseId){
		 ArrayList<WorkItem> workitems = GetWorkItems();
		 
		 ArrayList<WorkItem> output = new ArrayList<>();
		 
		 for(WorkItem wi : workitems){
			 if(wi.caseID == caseId){
				 output.add(wi);
			 }
		 }
		 
		 return output;
	}
	
	public String GetWorkItemNames(String seperator){
	
		ArrayList<WorkItem> workItem = GetWorkItems();
		
		String output = "";
		
		for(int i = 0; i < workItem.size(); i ++){
			output += workItem.get(i).taskID;
			if(i < workItem.size() - 1) output += seperator;
		}
		
		return "(" + output + ")";
		
	}
	
	public void AddAgent(Agent spec) {
		agents.put(spec.GetUID(), spec);
	}
	
	public Agent GetAgent(String UID) {
		return agents.get(UID);
	}
	
	public Agent GetAgent(String first, String last) {
		for(Agent agent : GetAgents()) {
			if(agent.GetFirstName().equals(first) && agent.GetSecondName().equals(last)) {
				return agent;
			}
		}
		
		return null;
	}
	
	public ArrayList<Agent> GetActiveAgents() {
		ArrayList<Agent> retval = new ArrayList<Agent>();
		
		Set<Entry<String, Agent>> kkk = agents.entrySet();
		
		for(Entry<String, Agent> ggg : kkk) {
			if(ggg.getValue().isActive())
				retval.add(ggg.getValue());
		}
				
		return retval;
	}
	
	public String GetActiveAgentNames(String seperator){
		
		ArrayList<Agent> activeAgents = GetActiveAgents();
		
		String output = "";
		
		for(int i = 0; i < activeAgents.size(); i ++){
			output += activeAgents.get(i).GetFirstName() + " " + activeAgents.get(i).GetSecondName();
			if(i < activeAgents.size() - 1) output += seperator;
		}
		
		return "(" + output + ")";
	}
	
	public ArrayList<Agent> GetAgents() {
		ArrayList<Agent> retval = new ArrayList<Agent>();
		
		Set<Entry<String, Agent>> kkk = agents.entrySet();
		
		for(Entry<String, Agent> ggg : kkk) {
			retval.add(ggg.getValue());
		}
				
		return retval;
	}
	
	public String GetAgentNames(String seperator){
		ArrayList<Agent> activeAgents = GetAgents();
		
		String output = "";
		
		for(int i = 0; i < activeAgents.size(); i ++){
			output += activeAgents.get(i).GetFirstName() + " " + activeAgents.get(i).GetSecondName();
			if(i < activeAgents.size() - 1) output += seperator;
		}
		
		return "(" + output + ")";
	}
	
	
	
	public String toString(){
		String output = "Veis interface internal cache: " + "\r\n";
		output += "\tActive agents:      " + GetActiveAgents().size() + GetActiveAgentNames(",") + "\r\n"; 
		output += "\tAgents:             " + GetAgents().size() + GetAgentNames(",") + "\r\n";
		output += "\tRunning cases:      " + GetCases().size() + GetCaseNames(",") + "\r\n";
		output += "\tAll specifications: " + GetSpecifications().size() + GetSpecificationNames(",") + "\r\n";
		output += "\tAll work items:     " + GetWorkItems().size() + GetWorkItemNames(",") + "\r\n";
		output += "\tLive connections:   " + AgentReceiverSocketServer.allClients.size() + "\r\n";
		return output;
	}
}

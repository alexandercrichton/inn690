package veis.entities.workflow;

public class Case {
	private String id = "-1";
	private String specificationID = "-1";

	public Case(String id, String specificationID) {
		this.id = id;
		this.specificationID = specificationID;
	}
	
	public void SetID(String id) {
		this.id = id;
	}

	public String GetID() {
		return this.id;
	}
	
	public void SetSpecificationID(String id) {
		this.specificationID = id; 
	}
	
	public String GetSpecificationID() {
		return this.specificationID;
	}
	
	
}

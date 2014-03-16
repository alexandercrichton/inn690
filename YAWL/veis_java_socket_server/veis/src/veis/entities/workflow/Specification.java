package veis.entities.workflow;

import org.yawlfoundation.yawl.engine.YSpecificationID;

public class Specification {
	
	private String UID = "";
	private String version = "";
	private String URI = "";
	
	public Specification(String uid, String version, String uri) {
		this.UID = uid;
		this.version = version;
		this.URI = uri;
	}
	
	public void SetUID(String uid) {
		UID = uid;
	}
	
	public String GetUID() {
		return UID;
	}
	
	public void SetVersion(String version) {
		this.version = version;
	}
	
	public String GetVersion() {
		return version;
	}
	
	public void SetURI(String uRI) {
		URI = uRI;
	}
	
	public String GetURI() {
		return URI;
	}
	
	public YSpecificationID GetYSpecID() {
		return new YSpecificationID(UID, version, URI);
	}
	
}

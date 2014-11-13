using UnityEngine;
using System.Collections;
using Veis.Unity.Bots;
using System.Collections.Generic;

public class navAgent : MonoBehaviour {
	
	public enum AgentControl {
		Bot,
		Human,
		Idle
	}
	
	//TODO: Add more states for Bots to act
	public enum BotState
	{
		Walking, 
		Working,
		Idle
	}
	
	float restSpeed = 0.0f;
	float restAngle = 0.0f;
	
	public GameObject targetPoint;
	public GUIText textObject;
	
	public GameObject pinPoint;
	public NavMeshAgent agent;
	
	public string taskQueue;
	public string botTask;
	
	//Bot Information
	public string ID;
	public string name;
	public string role;
	
	//Animation And Mecanim stuff
	protected Animator animator;
	
	protected Locomotion locomotion;
	protected Object pointClone;
	
	public UnityBotAvatar BotAvatar;
	GUIStyle labelStyle;
	
	
	public AgentControl controlStatus;
	public BotState botState;
	
	
	// Use this for initialization
	void Start () {
		
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		//agent.speed = 1.0f;
		
		animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);
		pointClone = null;
		
		//taskQueue = new List<string>();
		string taskQueue;
		
		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;
		labelStyle.fontSize = 20;
		
		botState = BotState.Idle;
		
		
		
	}
	
	public UnityBotAvatar GetAvatar()
	{
		return BotAvatar;
	}
	
	//
	protected void SetDestination () {
		
		//agent.destination = targetPoint.transform.position;
		
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		
		if (Physics.Raycast(ray, out hit)) {
			
			Debug.Log (hit.ToString());
			if (hit.transform.gameObject.tag != "floor") {
			} else {
				
				if (pointClone != null) {
					GameObject.Destroy(pointClone);
					pointClone = null;
				}
				
				Quaternion q = new Quaternion();
				q.SetLookRotation(hit.normal, Vector3.forward);
				pointClone = Instantiate(targetPoint, hit.point, q);
				agent.destination = hit.point;
				
			}
		}
		
	}
	protected void SetupAgentLocomotion() {
		
		if (AgentDone ()) {
			locomotion.Do (0, 0);
			if (pointClone != null) {
				GameObject.Destroy (pointClone);
				pointClone = null;
			}
		} else {
			float speed = agent.desiredVelocity.magnitude;
			
			Vector3 velocity = Quaternion.Inverse (transform.rotation) * agent.desiredVelocity;
			
			float angle = Mathf.Atan2 (velocity.x, velocity.z) * 180.0f / 3.14159f;
			if (botState == BotState.Walking) {
				locomotion.Do (speed, angle);
			} else if (botState == BotState.Idle){
				locomotion.Do(restSpeed, restAngle);
			}
		}
		
	}
	
	void OnAnimatorMove() {
		agent.velocity = animator.deltaPosition / Time.deltaTime;
		transform.rotation = animator.rootRotation;
	}
	
	//Moves the Agent to the target location
	public void SetTarget(GameObject location ) {
		pinPoint = location;
		agent.destination = location.transform.position;
	}
	
	//Exposes the current task the bot needs to do
	public void SetTask(string currentTask) {
		
		taskQueue = currentTask;
		
	}
	
	//Initialises the Bot specifications when instantiated 
	//See: UnitySimulation CreateBotAvatar() and UnityBotAvatar SendBotValues()
	public void SetBotInfo(UnityBotAvatar botAvatar) {
		name = botAvatar.Name;
		role = botAvatar.Role;
		ID = botAvatar.ID;
		controlStatus = AgentControl.Bot;
		
		
	}
	
	protected bool AgentDone() {
		return !agent.pathPending && AgentStopping();
	}
	
	protected bool AgentStopping() {
		return agent.remainingDistance <= agent.stoppingDistance;
	}
	
	private void SetUpGUI() {
		Vector3 objectScreenPosition = Camera.main.WorldToScreenPoint(this.transform.position);
		string objectText = name;
		float objectLabelSize = 200f;
		Rect objectLabelRect = new Rect(
			objectScreenPosition.x - objectLabelSize / 2,
			Screen.height - objectScreenPosition.y - objectLabelSize / 2,
			objectLabelSize,
			objectLabelSize);
		GUI.Label(objectLabelRect, objectText, labelStyle);
	}
	
	void Update() {
		
		if (controlStatus == AgentControl.Human) {
			if (Input.GetMouseButtonDown(0))
				SetDestination();
		}
		SetupAgentLocomotion();
		
		CheckState ();
		
	}
	
	//TODO: More states dpeending on how much action the bot requires
	void CheckState() {
		string currentState = taskQueue.Split(':')[0];
		
		if (currentState == "WALKTO") {
			botState = BotState.Walking;
			
		} else {
			botState = BotState.Idle;
		}
	}
	
	void OnGUI () {
		SetUpGUI();
	}
	
}

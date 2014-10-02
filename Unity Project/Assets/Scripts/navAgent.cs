using UnityEngine;
using System.Collections;
using Veis.Unity.Bots;
using System.Collections.Generic;

public class navAgent : MonoBehaviour {

	public GameObject targetPoint;
	public GUIText textObject;

	public GameObject pinPoint;
	public NavMeshAgent agent;

	public List<string> taskQueue;
	public string botTask;

	//Bot Information
	public string ID;
	public string name;
	public string role;
	
	//Animation And Mecanim stuff
	protected Animator animator;

	protected Locomotion locomotion;
	protected Object pointClone;

	UnityBotAvatar botAvatar;
	GUIStyle labelStyle;

	// Use this for initialization
	void Start () {

		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.speed = 1.0f;

		animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);
		pointClone = null;

		taskQueue = new List<string>();

		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;
		labelStyle.fontSize = 20;

		
	}
	
	protected void SetDestination () {

		//agent.destination = targetPoint.transform.position;

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();

		if (Physics.Raycast(ray, out hit)) {
			if (pointClone != null) {
				GameObject.Destroy(pointClone);
				pointClone = null;
			}

			Quaternion q = new Quaternion();
			q.SetLookRotation(hit.normal, Vector3.forward);
			pointClone = Instantiate(pinPoint, hit.point, q);
			agent.destination = hit.point;

		}

	}
	protected void SetupAgentLocomotion() {
		if (AgentDone()) {
			locomotion.Do (0,0);
			if (pointClone != null) {
				GameObject.Destroy(pointClone);
				pointClone = null;
			}
		} else {
			float speed = agent.desiredVelocity.magnitude;

			Vector3 velocity = Quaternion.Inverse(transform.rotation) * agent.desiredVelocity;

			float angle = Mathf.Atan2(velocity.x, velocity.z) * 180.0f/ 3.14159f;

			locomotion.Do(speed, angle);
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

		if(currentTask[0] == 'A') {
			taskQueue.Add(currentTask);
		}
	}

	public void SetBotInfo(UnityBotAvatar botAvatar) {
		name = botAvatar.Name;
		role = botAvatar.Role;
		ID = botAvatar.ID;



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

		SetupAgentLocomotion();
		
	}

	void OnGUI () {
		SetUpGUI();
	}
}

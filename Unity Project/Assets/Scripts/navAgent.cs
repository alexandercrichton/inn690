using UnityEngine;
using System.Collections;

public class navAgent : MonoBehaviour {

	public GameObject targetPoint;

	public GameObject pinPoint;
	public NavMeshAgent agent;
	protected Animator animator;

	protected Locomotion locomotion;
	protected Object pointClone;

	// Use this for initialization
	void Start () {
	
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;

		animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);
		pointClone = null;

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

	void SetTarget(GameObject location ) {
		agent.destination = location.transform.position;
	}

	protected bool AgentDone() {
		return !agent.pathPending && AgentStopping();
	}

	protected bool AgentStopping() {
		return agent.remainingDistance <= agent.stoppingDistance;
	}

	void Update() {

		/*if (Input.GetButtonDown ("Fire1")) {
			SetDestination();
		}*/
		SetTarget (targetPoint);

		SetupAgentLocomotion();


	}

}

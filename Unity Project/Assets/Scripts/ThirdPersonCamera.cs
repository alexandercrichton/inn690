using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThirdPersonCamera : MonoBehaviour
{
	public float smooth = 3f;		// a public variable to adjust smoothing of camera motion
	Transform standardPos;			// the usual position for the camera, specified by a transform in the game
	Transform lookAtPos;			// the position to move the camera to when using head look
	public GameObject avatarObject;
	UserInteraction userInteractionScript;
	
	void Start()
	{
		if(GameObject.Find ("LookAtPos"))
			lookAtPos = GameObject.Find ("LookAtPos").transform;
		userInteractionScript = GameObject.FindGameObjectWithTag("Simulator").GetComponent<UserInteraction>();
	}
	
	void FixedUpdate ()
	{
		
		/*
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			avatarObject = getObjectUserClickedOn();


			if (avatarObject != null && avatarObject.tag == "Avatar")
			{
				standardPos = avatarObject.transform.FindChild("CamPos").transform;
			}

		}
			*/


		// if we hold Alt
		if(Input.GetButton("Fire2") && lookAtPos)
		{
			// lerp the camera position to the look at position, and lerp its forward direction to match 
			transform.position = Vector3.Lerp(transform.position, lookAtPos.position, Time.deltaTime * smooth);
			transform.forward = Vector3.Lerp(transform.forward, lookAtPos.forward, Time.deltaTime * smooth);
		}
		else
		{	
			// return the camera to standard position and direction
			transform.position = Vector3.Lerp(transform.position, userInteractionScript.camTransform.position, Time.deltaTime * smooth);	
			transform.forward = Vector3.Lerp(transform.forward, userInteractionScript.camTransform.forward, Time.deltaTime * smooth);
		}
		
	}



}

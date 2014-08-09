using UnityEngine;
using System.Collections;

public class BotMovingScript : MonoBehaviour {


	public AnimationClip walkAnimation;
	public AnimationClip idleAnimation;
	public AnimationClip runAnimation;

	public float updateTime;

	public Vector3 newPosition;

	public float interval = 2.0f;

	void Start() {
		InvokeRepeating("GetPosition",0,1);
	}
	
	// Update is called once per frame
	void Update () {

		if(newPosition != this.transform.position) {
			animation.CrossFade(walkAnimation.name);
		} else {
			animation.CrossFade(idleAnimation.name);
		}

	}

	void GetPosition() {
		newPosition = this.transform.position;
	}
}

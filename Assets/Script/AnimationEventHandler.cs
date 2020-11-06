using UnityEngine;
using System.Collections;

public class AnimationEventHandler : MonoBehaviour {
	
	public GameManager gameManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnAnimationEvent(string tag) {
		if(tag == "move_to_task")
			gameManager.playTaskAnimation();
	}
}

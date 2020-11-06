using UnityEngine;
using System.Collections;

public class MainCameraFollower : MonoBehaviour {

	public PlayerFollower follower;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = follower.originalPos;
	}
}

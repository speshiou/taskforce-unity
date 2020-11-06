using UnityEngine;
using System.Collections;

/*
 * give the child a kinematic rigidbody. 
 * This will separate it from the parent collision, but still allow it to be moved by the parent's transform instead of physics.
 */
public class Bone : MonoBehaviour {

	public Skeleton skeleton;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCollisionEnter(Collision col) {
		skeleton.OnBoneCollisionEnter (this, col);
	}
	
	public void OnTriggerEnter(Collider col) {
		skeleton.OnBoneTriggerEnter (this, col);
	}
	
	public void OnTriggerEnter2D(Collider2D col) {
		skeleton.OnBoneTriggerEnter2D (this, col);
	}
}

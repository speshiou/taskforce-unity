using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour {
	
	/// <summary>
    /// A reference to the transform to follow (offset from the camera)
    /// </summary>
	public Transform target;
	public Vector3 offset;
	
	/// <summary>
    /// The position of the camera
    /// </summary>
	private Vector3 pos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		pos = target.position;
		//if(target.position.x > pos.x) {
		//pos.x = Vector3.Lerp(pos, target.position, 1).x;
		transform.position = pos + offset;
		//}
	}
}

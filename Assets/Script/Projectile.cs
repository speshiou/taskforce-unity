using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public float damage;
	public Vector3 direction;
	public string target;

	// Use this for initialization
	void Start () {
		Vector3 rotation = Vector3.zero;
		rotation.z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.eulerAngles = rotation;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class Grenade : Projectile {
	public float liveTime = 3;
	
	public Transform prefExplosion;

	// Use this for initialization
	void Start () {
		Vector3 rotation = Vector3.zero;
		rotation.z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.eulerAngles = rotation;
	}
	
	// Update is called once per frame
	void Update () {
		liveTime -= Time.deltaTime;
		if(liveTime <= 0) {
			
			Instantiate(prefExplosion, transform.position, Quaternion.identity);
			
			Destroy(gameObject);
		}
	}
}

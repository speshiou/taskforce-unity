using UnityEngine;
using System.Collections;

public class Bullet4Gun3 : Projectile {

	public Explosion prefExplostion;
	private float offset = 3;

	
	// Use this for initialization
	void Start () {
		Vector3 rotation = Vector3.zero;
		rotation.z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.eulerAngles = rotation;
		direction.z = 0;
		direction = direction.normalized * offset;
//		Explosion e = (Explosion)Instantiate (prefExplostion, transform.position + direction, Quaternion.identity);
//		e.damage = damage;
//		e.target = target;
	}
	
	// Update is called once per frame
	void Update () {

	}
	

}

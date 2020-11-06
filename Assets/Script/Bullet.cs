using UnityEngine;
using System.Collections;

public class Bullet : Projectile {
	
	public bool piercable = false;
	private float speed = 100;
	public bool available = true;
	public bool shieldable = true;
	public float force = -1;
	public float distance = -1;
	private Vector3 _startPos;

	// Use this for initialization
	void Start () {
		Vector3 rotation = Vector3.zero;
		rotation.z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.eulerAngles = rotation;
		direction.z = 0;
		direction = direction.normalized * speed;
		_startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(direction != Vector3.zero)
			rigidbody.MovePosition(transform.position + direction * Time.deltaTime);

		if(distance != -1) {
			float d = Vector3.Distance(_startPos, transform.position);
			if(d > distance)
				Destroy(gameObject);
		}
	}
	
	public bool isAvailable() {
		return available;
	}
	
	public void onShield() {
		if (!shieldable)
			return;
		available = false;
		if(collider.isTrigger) {
			direction = Vector3.zero;
			collider.isTrigger = false;
			rigidbody.useGravity = true;
			gameObject.layer = LayerMask.NameToLayer("Item");
		}
	}
	
	public void onHit(Soldier soldier) {
		if (force != -1) {
			float d = Vector3.Distance(_startPos, soldier.transform.position);
			if(d > distance)
				d = distance;
			float rate = (distance - d) / distance;
			float f = Mathf.Lerp(500, force, rate);
			soldier.rigidbody.AddForce(f * direction.normalized);
		}
		if(!piercable) {
			available = false;
			Destroy(gameObject);
		}
	}
	
	public void OnTriggerEnter(Collider other) {
		if(other.tag == "bumper" || other.tag == "tile")
			Destroy(gameObject);
	}
	
	public void OnCollisionEnter(Collision other) {
		string t = other.gameObject.tag;
		if (t == "bumper") {
			Destroy(gameObject);
		}
	}
}

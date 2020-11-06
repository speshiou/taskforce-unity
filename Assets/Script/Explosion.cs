using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public float power = 3000;
	public float radius = 10;

	public AudioClip sound;
	public float damage;
	public bool shakeEffect = false;
	
	void Awake () {
	}

	// Use this for initialization
	void Start () {
		audio.PlayOneShot(sound);

		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere (explosionPos, radius);
		foreach (Collider hit in colliders) {
			if (!hit || hit.Equals(this.collider))
				continue;
			
			if (hit.rigidbody && hit.tag != "unavailable" && (hit.tag == "player" || hit.tag == "enemy"))
				hit.rigidbody.AddExplosionForce(power, explosionPos, radius, 3.0f);
		}

		if (shakeEffect) {
			PlayerFollower f = Camera.main.GetComponent<PlayerFollower> ();
			f.playShakeEffect ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnTriggerEnter(Collider col) {
		string other = col.tag;
		if(other == "player" || other == "enemy") {
			Soldier s = col.GetComponent<Soldier>();
			if(s == null) return;
			s.hurt(damage, 0);
		}
	}
}

using UnityEngine;
using System.Collections;

public class Parachute : MonoBehaviour {

	private Animator _anim;

	void Awake() {
		_anim = GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void open() {
		if (rigidbody != null) {
			rigidbody.drag = 3;
			_anim.SetBool("falling", true);
		}
	}

	public void close() {
		if (rigidbody != null) {
			rigidbody.drag = 0;
			_anim.SetBool("falling", false);
		}
	}
}

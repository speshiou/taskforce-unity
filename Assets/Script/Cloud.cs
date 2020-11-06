using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

	public float speed;
	private Vector3 defaultPos;

	void Awake() {
		defaultPos = transform.position;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.left * speed * Time.deltaTime);
	}

	public void reset() {
		transform.position = defaultPos;
	}
}

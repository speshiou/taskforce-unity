using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {

	public Cloud[] clouds;
	public Transform target;
	public int range;

	private Vector3 _d = Vector3.zero;

	// Use this for initialization
	void Start () {
		clouds = gameObject.GetComponentsInChildren<Cloud> ();
		reset ();
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Cloud c in clouds) {
			Transform t = c.transform;
			if((target.position.x - t.position.x) > range) {
				Vector3 pos = new Vector3(target.position.x, t.position.y, t.position.z);
				t.position = pos + Vector3.right * range * Random.Range(1.3f, 1.6f);
			}
		}
	}
	
	public void offset(Vector3 d) {
		foreach (Cloud c in clouds) {
			Transform t = c.transform;
			t.position = t.position + d;
		}
	}

	public void reset() {
		foreach (Cloud c in clouds) {
			c.reset();
		}
	}
}

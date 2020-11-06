using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {
	
	public AudioClip soundCase;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnCollisionEnter(Collision other) {
		string t = other.gameObject.tag;
		if(t == "tile") {
			if(soundCase != null) {
				audio.PlayOneShot(soundCase);
				soundCase = null;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class WaitForClear : MonoBehaviour {
	
	public float liveTime = 5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		liveTime -= Time.deltaTime;
		if(liveTime < 0)
			Destroy(gameObject);
	}
}

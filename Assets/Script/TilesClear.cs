using UnityEngine;
using System.Collections;

public class TilesClear : MonoBehaviour {

	public TileMapManager tileManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnTriggerEnter(Collider other) {

	}
	
	public void OnTriggerExit(Collider other) {
		if(other.tag == "tiles") {
			//Destroy(other.gameObject);
			other.transform.position = new Vector3(0, -2000, 0);

			int count = tileManager.getAliveTilesCount();
			if(count <= 0)
				tileManager.notifyUpdateTilemap();
		}
	}
}

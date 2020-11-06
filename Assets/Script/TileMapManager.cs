using UnityEngine;
using System;
using System.Collections;

public class TileMapManager : MonoBehaviour {
	
	public GameManager gameManager;
	public Transform tilemap;
	public Transform prefTilesContainer;
	public Transform prefTile;
	private static string[] _maps = new string[] {
		"123456788888888888887654321",
		"12344444444322222211111",
		"123456666667777654321",
		"1222222222222222221",
		"12344444321",
		"12344455566665544332222222222222222222223344556666555444321",
		"111222211",
		"12344455567765544332223333444445554444433333222222223344556666555444321",
		"11122223333444445555666777888"
	};

	private static Transform[] _tilemaps = new Transform[_maps.Length];

	// Use this for initialization
	void Awake () {
		Vector3 pos = Vector3.zero;
		pos.y = -2000;
		for (int i = 0; i < _maps.Length; i++) {
			generateTile(pos, i);
		}
	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnTriggerExit(Collider other) {
		if (other.tag == "tiles") {
			Debug.Log ("OnTriggerExit tiles t=" + Time.time);
			notifyUpdateTilemap ();
		}
	}
	
	public void notifyUpdateTilemap() {
		Debug.Log ("notifyUpdateTilemap");
		generateTiles(new Vector3(transform.position.x + UnityEngine.Random.Range(5, 10), 0, 0));
	}

	public void generateTiles(Vector3 pos) {
		Debug.Log ("+generateTiles=" + pos);
		Transform container = null;
		int retry = 3;
		while (container == null && retry > 0) {
			int i = UnityEngine.Random.Range (0, _maps.Length);
			container = getTilesIfAvailable(i);
			--retry;
		}

		if (container != null) {
			container.localPosition = pos;
			Debug.Log ("-generateTiles=" + pos);
		}
	}

	private Transform getTilesIfAvailable(int type) {
		Transform container = _tilemaps [type];
		if (container.position.y < -1000)
			return container;
		return null;
	}

	public void generateTile(Vector3 pos, int type) {
		Vector3 baseScale = new Vector3(1, 1, 1);
		string map = _maps[type];
		Transform container = (Transform)Instantiate(prefTilesContainer, pos, Quaternion.identity);
		container.parent = tilemap;
		//pos.y = 0;
		pos.z = 0;
		container.localPosition = pos;
		container.localScale = baseScale;
		Vector3 tilePos = Vector3.zero;
		tilePos.x = 0;
		for(int i = 0; i < map.Length; i++) {
			int h = Convert.ToInt32(map[i].ToString());
			tilePos.y = 0;
			for(int j = 0; j < h; j++) {
				Transform t = (Transform)Instantiate(prefTile);
				t.parent = container;
				t.localPosition = tilePos;
				t.localScale = baseScale;
				tilePos.y += 0.857f;
			}
			tilePos.x += 1.714f;
		}
		BoxCollider boxCollider = container.GetComponent<BoxCollider>();
		boxCollider.center = new Vector3(tilePos.x / 2, 0, 0);
		boxCollider.size = new Vector3(tilePos.x, 3, 1);

		_tilemaps [type] = container;
	}

	public int getAliveTilesCount() {
		int count = 0;
		for (int i = 0; i < _tilemaps.Length; i++) {
			if(_tilemaps[i].position.y > -1000)
				++count;
		}
		return count;
	}

	public void clear () {
		Debug.Log ("clear");
		GameObject[] objs = GameObject.FindGameObjectsWithTag("tiles");
		foreach (GameObject obj in objs) {
				//Destroy (obj);
			obj.transform.position = new Vector3(0, -2000, 0);
		}
	}
}

using UnityEngine;
using System;
using System.Collections;

public class Map : MonoBehaviour {
	
	public GameManager gameManager;
	public TileMapManager tilemapManager;
	public Transform target;
	public Transform bg_1;
	public Transform bg_1_2;
	public Transform bg_2;
	public Transform bg_3;
	public Transform ground;
	public Transform g2;
	public Clouds clouds;
	
	public float speedBg1 = 0.3f;
	public float speedBg2 = 0.5f;
	public float speedBg3 = 0.7f;

	//cache
	private float bgOffsetX;
	private float gOffsetX;
	private Vector3 bg1DefaultPos;
	private Vector3 gDefaultPos;

	private Vector3 direction;
	private float totalDx;
	
	private Vector3 nextTilesPos;

	void Awake() {
		bg1DefaultPos = bg_1.position;
		gDefaultPos = ground.position;
		bgOffsetX = bg_1_2.position.x;
		gOffsetX = g2.position.x;
	}

	// Use this for initialization
	void Start () {
		//reset();
	}
	
	// Update is called once per frame
	void Update () {
		resetBgPosIfNecessary(bg_1);
		resetBgPosIfNecessary(bg_2);
		resetBgPosIfNecessary(bg_3);
		resetGroundPosIfNecessary(ground);
	}
	
	public void offset(Vector3 d) {
		Vector3 pos =  bg_1.position;
		pos += d * 0.5f;
		bg_1.position = pos;
		clouds.offset (d * 0.5f);

		if (bg_2 != null) {
			pos = bg_2.position;
			pos += d * 0.3f;
			bg_2.position = pos;
		}
		totalDx += Mathf.Abs(d.x) * 2;
		gameManager.distance = (int)(totalDx / gameManager.gameSpeed);
	}

	public void move() {
		//direction = Vector3.right;
	}
	
	public void stop() {
		//direction = Vector3.zero;
	}
	
	public void reset() {
		totalDx = 0;
		bg_1.position = bg1DefaultPos;
		//resetBgPos (bg_2);
		//resetBgPos (bg_3);
		ground.position = gDefaultPos;
		clouds.reset ();
		tilemapManager.clear ();
		tilemapManager.notifyUpdateTilemap ();
	}

	private void resetBgPos(Transform obj) {
		if (obj == null)
			return;
		obj.position = new Vector3(0, obj.position.y, obj.position.z);
	}
	
	private void resetBgPosIfNecessary(Transform obj) {
		if (obj == null)
			return;
		if((target.position.x -  obj.position.x) >= bgOffsetX) {
			Vector3 v = new Vector3(bgOffsetX,0,0);
			obj.position += v;
		}
	}
	
	private void resetGroundPosIfNecessary(Transform obj) {
		if((target.position.x - obj.position.x) >= gOffsetX) {
			Vector3 v = new Vector3(gOffsetX,0,0);
			obj.position += v;
		}
	}
}

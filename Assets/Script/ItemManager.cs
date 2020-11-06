using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {
	
	public GameManager gameManager;
	public Transform refPos;
	public Box prefWeapon;
	public Box prefAidKit;
	
	public int weapon2BoxCount;
	public int weapon3BoxCount;
	public int weapon4BoxCount;
	public int weapon5BoxCount;
	public int weaponAllBoxCount;
	public int aidkitBoxCount;
	
	private Box.TYPE[] types = new Box.TYPE[] { Box.TYPE.Aidkit, 
												Box.TYPE.Aidkit, 
												//Box.TYPE.AllWeapon, 
												Box.TYPE.Weapon2,
												Box.TYPE.Weapon3, 
												Box.TYPE.Weapon4, 
												Box.TYPE.Weapon5 };
	
	private List<Box> _items = new List<Box>();
	private int _nextPutItemX;
	
	// Use this for initialization
	void Start () {
		Box box;

        /*for (int i = 0; i < weapon2BoxCount; i++) {
            // create the object and place it under this manager
            box = (Box)Instantiate(prefWeapon2Box, Vector3.zero, Quaternion.identity);
            box.transform.parent = this.transform;
			box.gameObject.SetActive(false);
			box.gameManager = gameManager;
            _items.Add(box);
        }

        for (int i = 0; i < weapon3BoxCount; i++) {
            // create the object and place it under this manager
            box = (Box)Instantiate(prefWeapon3Box, Vector3.zero, Quaternion.identity);
            box.transform.parent = this.transform;
			box.gameObject.SetActive(false);
			box.gameManager = gameManager;
            _items.Add(box);
        }

        for (int i = 0; i < weapon4BoxCount; i++) {
            // create the object and place it under this manager
            box = (Box)Instantiate(prefWeapon4Box, Vector3.zero, Quaternion.identity);
            box.transform.parent = this.transform;
			box.gameObject.SetActive(false);
			box.gameManager = gameManager;
            _items.Add(box);
        }
		
		for (int i = 0; i < weapon5BoxCount; i++) {
            // create the object and place it under this manager
            box = (Box)Instantiate(prefWeapon5Box, Vector3.zero, Quaternion.identity);
            box.transform.parent = this.transform;
			box.gameObject.SetActive(false);
			box.gameManager = gameManager;
            _items.Add(box);
        }
		
		for (int i = 0; i < weaponAllBoxCount; i++) {
            // create the object and place it under this manager
            box = (Box)Instantiate(prefWeaponAllBox, Vector3.zero, Quaternion.identity);
            box.transform.parent = this.transform;
			box.gameObject.SetActive(false);
			box.gameManager = gameManager;
            _items.Add(box);
        }
		
		for (int i = 0; i < aidkitBoxCount; i++) {
            // create the object and place it under this manager
            box = (Box)Instantiate(prefAidKit, Vector3.zero, Quaternion.identity);
            box.transform.parent = this.transform;
			box.gameObject.SetActive(false);
			box.gameManager = gameManager;
            _items.Add(box);
        }*/
		reset();
	}
	
	// Update is called once per frame
	void Update () {
		if(gameManager.gameState == GameManager.GameState.RUNNING) {
			putItem();
		}
	}
	
	private void putItem() {
		if(gameManager.distance > _nextPutItemX) {
			int N = 1;
			for (int i = 0; i < N; i++) {
				Vector3 pos = refPos.position;
				pos = new Vector3(pos.x + 3, 30, 0);
				int idx = Random.Range(0, types.Length);
				putItem(types[idx], pos);
			}
			generateNextPos();
		}
	}
	
	public void putItem(Box.TYPE type, Vector3 pos) {
		/*foreach (Box box in _items) {
			if(!box.gameObject.activeSelf && box.type == type) {
				box.gameObject.SetActive(true);
				box.transform.position = pos;
			}
		}*/

		Box box;
		if (type == Box.TYPE.Aidkit) {
			box = (Box)Instantiate (prefAidKit, pos, Quaternion.identity);
		} else {
			box = (Box)Instantiate (prefWeapon, pos, Quaternion.identity);
		}
		box.type = type;
		box.gameManager = gameManager;
		if (Random.Range (0, 5) > 3) {
			box.parachute.open();
		}
	}
	
	private void generateNextPos() {
		_nextPutItemX += Random.Range(40, 50);
	}
	
	public void reset() {
		_nextPutItemX = 0;
		generateNextPos();
		foreach (Box box in _items) {
			//box.gameObject.SetActive(false);
		}

		GameObject[] objs = GameObject.FindGameObjectsWithTag ("item");
		foreach (GameObject obj in objs) {
			Destroy(obj);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
	
	public Soldier prefSoldier;
	public Helicopter prefHelicopter;
	public Transform prefNotice;
	public GameManager gameManager;

	public int maxEnemyCount;
	
	private List<Soldier> _enemies = new List<Soldier>();
	private int _nextSpawnDistance = 5;

	private int _nextSpawnHelicopterDistance = 200;

	// Use this for initialization
	void Start () {
		/*for (int i = 0; i < maxEnemyCount; i++) {
			Soldier e = (Soldier)Instantiate(prefSoldier, Vector3.zero, Quaternion.identity);
			e.gameManager = gameManager;
			e.gameObject.SetActive(false);
			e.transform.parent = transform;
			_enemies.Add(e);
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		if(gameManager.gameState == GameManager.GameState.RUNNING) {
			spawn();
		}
	}
	
	private void spawn() {
		Vector3 refPos = gameManager.map.tilemapManager.transform.position;
		if(_nextSpawnDistance < gameManager.distance ) {
			int max = 1 + gameManager.distance / 50;
			max = max <= 0 ? 1 : max;
			max = max >= 5 ? 5 : max;
			int min = gameManager.distance / 200;
			min = min <= 0 ? 1 : min;
			int N = Random.Range(min, max);
			bool enableParachute = gameManager.distance > 100;
			for (int i = 0; i < N; i++) {
				Vector3 pos = refPos;
				bool withParachute = false;
				if(enableParachute) {
					withParachute = Random.Range(0, 5) > 3;
				}

				pos = new Vector3(pos.x + Random.Range(3.0f, 8.0f), 30, gameManager.player.transform.position.z);
				Soldier e = getAvaliableSoldier();
				if(withParachute)
					e.parachute.open();
				if(e != null) {
					Weapon[] weapons = e.GetComponentsInChildren<Weapon>(true);
					int iWeapon = Random.Range(0, weapons.Length);
					Weapon w = weapons[iWeapon];
					w.fill();
					e.reset();
					e.transform.position = pos;
					e.equipWeapon(w);
					
				//w.bullet = (Bullet)Resources.LoadAssetAtPath("Assets/Sprite/bullet_for_enemy.prefab", typeof(Bullet));
				}
			}
			_nextSpawnDistance += Random.Range(3, 10);
		}

		if (_nextSpawnHelicopterDistance < gameManager.distance) {
			Vector3 pos = refPos;
			pos = new Vector3(pos.x + 50, pos.y, 0);
			Helicopter e = (Helicopter)Instantiate(prefHelicopter, pos, Quaternion.identity);
			e.target = gameManager.player.transform;
			_nextSpawnHelicopterDistance += Random.Range(130, 180);

			//notice player
			pos = gameManager.player.transform.position;
			Transform t = (Transform)Instantiate(prefNotice, pos, Quaternion.identity);
			Follower f = t.GetComponent<Follower>();
			f.target = gameManager.player.transform;
		}
	}
	
	private Soldier getAvaliableSoldier() {
		/*int N = _enemies.Count;
		for (int i = 0; i < N; i++) {
			Soldier e = _enemies[i];
			if(!e.gameObject.activeSelf)
				return e;
		}
		return null;*/
		Soldier e = (Soldier)Instantiate(prefSoldier, Vector3.zero, Quaternion.identity);
		e.gameManager = gameManager;
		e.transform.parent = transform;
		return e;
	}
	
	public void reset() {
		_nextSpawnDistance = 5;
		_nextSpawnHelicopterDistance = 100;

		foreach(Soldier e in _enemies) {
			e.reset();
			e.gameObject.SetActive(false);
		}

		GameObject[] objs = GameObject.FindGameObjectsWithTag ("enemy");
		foreach(GameObject obj in objs) {
			Destroy(obj);
		}
	}
}

using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour {
	
	public enum TYPE {
		Aidkit,
		Weapon2,
		Weapon3,
		Weapon4,
		Weapon5,
		AllWeapon
	}
	
	public GameManager gameManager;

	public TYPE type;
	public Parachute parachute;
	private Animator _anim;
	private bool _isAvailable = true;


	void Awake() {
		parachute = GetComponent<Parachute> ();
		_anim = GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -100)
			Destroy (gameObject);
	}

	public void OnCollisionEnter(Collision col) {
		switch (col.gameObject.tag) {
		case "tile":
			if(parachute != null) {
				parachute.close();
			}
			break;
		}
	}
	
	public void OnTriggerEnter(Collider col) {
		if(col.tag == "player") {
			if(!_isAvailable)
				return;
			_isAvailable = false;
			_anim.SetTrigger("open");
			if(audio != null)
				audio.Play();
			switch(type) {
			case TYPE.Aidkit:
				gameManager.player.setHp(gameManager.soldierMaxHp);
				break;
			case TYPE.Weapon2:
				gameManager.playerWeapon2.fill();
				break;
			case TYPE.Weapon3:
				gameManager.playerWeapon3.fill();
				break;
			case TYPE.Weapon4:
				gameManager.playerWeapon4.fill();
				break;
			case TYPE.Weapon5:
				gameManager.playerWeapon5.fill();
				break;
			case TYPE.AllWeapon:
				gameManager.playerWeapon2.fill();
				gameManager.playerWeapon3.fill();
				gameManager.playerWeapon4.fill();
				gameManager.playerWeapon5.fill();
				break;
			}
		}
	}

}

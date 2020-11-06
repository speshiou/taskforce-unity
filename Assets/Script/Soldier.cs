using UnityEngine;
using System.Collections;

public class Soldier : Skeleton {
	
	public enum STATE {
		Standing,
		Running,
		Attacking,
		Shielding,
        Dying,
		Unavailable
	}
	
	public enum Role {
		Player,
		Enemy
	}
	
	//transform
	public GameManager gameManager;
	public ProgressBar healthBar;
	public string target;
	public Animator prefBlood;
	public Animator prefHeadShot;
	public Parachute parachute;
	
	private Transform _boneHands;
	private Transform _boneMuzzle;
	private Transform _boneEjection;
	private Transform _boneHead;
	private Vector3 _boneHandsRotation;
	private Animator _anim;
	private SpriteRenderer _spriteHead;
	private SpriteRenderer _spriteBody;
	private SpriteRenderer _spriteFeet;
	
	//settings
	public float defenseHead = -10;
	public float defenseBody = 0.5f;
	public float defenseFeet = 0.7f;
	public float clearBodyTime = 5;
	public Role role;
	
	private float _speed;
	public float moveForce = 400f;			// Amount of force added to move the player left and right.
	
	//status
	public float health = 100;
	private STATE _state;
	private float _clearBodyTimeRemaining;
	
	//weapon
	private Weapon _currentWeapon = null;
	private bool _isAttacking = false;
	private bool _isReloading = false;
	private bool _isShield = false;
	private Vector3 _fireDirection;
	private Vector3 _moveDirection;
	
	//ai
	public float attckingMaxRange = 700;
	public float attckingMinRange = 300;
	public bool ai = false;
	private float _nextCommandTimeRemaining;
	
	//sound
	public AudioClip soundShield;
	public AudioClip soundThrowGrenade;
	public AudioClip soundHit1;
	public AudioClip soundHit2;

	public AudioSource soundDanger;
	
	void Awake() {

		_anim = GetComponent<Animator> ();

		_boneHands = transform.Find("body/hand_right");
		_boneHead = transform.FindChild("body/head");
		_boneMuzzle = transform.FindChild("body/hand_right/muzzle");
		_boneEjection = transform.FindChild("body/hand_right/ejection");

		_spriteHead = transform.Find("body/head").GetComponent<SpriteRenderer>();
		_spriteBody = transform.Find("body").GetComponent<SpriteRenderer>();
		_spriteFeet = transform.Find("body/feet").GetComponent<SpriteRenderer>();

		if (_currentWeapon == null) {
			_currentWeapon = gameObject.GetComponentInChildren<Weapon>();
		}

		if (gameManager == null)
			gameManager = GameObject.Find ("Game Manager").GetComponent<GameManager>();

		
		/*if(role == Role.Enemy) {
			boneAnimation.SwapTexture("player", "player_body", "player", "enemy_body");
			boneAnimation.SwapTexture("player", "player_head", "player", "enemy_head");
			boneAnimation.SwapTexture("player", "player_run_1", "player", "enemy_run_1");
			boneAnimation.SwapTexture("player", "player_run_2", "player", "enemy_run_2");
			boneAnimation.SwapTexture("player", "player_run_3", "player", "enemy_run_3");
			boneAnimation.SwapTexture("player", "player_stand", "player", "enemy_stand");
		}*/

		parachute = GetComponent<Parachute> ();

		healthBar = GetComponentInChildren<ProgressBar> ();
		healthBar.hide ();
	}
	
	// Use this for initialization
	void Start () {
		//boneAnimation.RegisterUserTriggerDelegate(UserTrigger);
		//boneAnimation.RegisterColliderTriggerDelegate(ColliderTrigger);
		_speed = gameManager.gameSpeed;
		_moveDirection = Vector3.zero;
		_boneHandsRotation = Vector3.zero;

		reset();
	}
	
	// Update is called once per frame
	void Update () {
		if(_state == STATE.Dying || _state == STATE.Unavailable) {
			return;
		}

		checkState();

		if(_state == STATE.Running)
			rigidbody.MovePosition(transform.position + _moveDirection * _speed * Time.deltaTime);

		if(ai && isAlive())
			doAI();
	}
	
	// bone processing goes here, after Unity has made its changes using its animation processes	
	void LateUpdate() {

		if (_state == STATE.Unavailable && tag == "enemy")
			Destroy (gameObject);

		if(_state == STATE.Dying || _state == STATE.Unavailable) {
			return;
		}

		if(_state != STATE.Shielding && _state != STATE.Dying && !_isReloading) {
			if(!_currentWeapon.isTriggerable()) {
				faceDirection(_fireDirection);
				setArm();
			} else if(_isAttacking) {
				faceDirection(_fireDirection);
				shoot();
			} else
				faceDirection(_moveDirection);
		} else if(_state == STATE.Running || _state == STATE.Shielding)
			faceDirection(_moveDirection);

		if (role == Role.Enemy) {
			_spriteHead.sprite = gameManager.spriteEnemyHead;
			_spriteBody.sprite = gameManager.spriteEnemyBody;
			if(_spriteFeet.sprite.Equals(gameManager.spritePlayerFeetStand))
				_spriteFeet.sprite = gameManager.spriteEnemyFeetStand;
			else if(_spriteFeet.sprite.Equals(gameManager.spritePlayerFeetRun1))
				_spriteFeet.sprite = gameManager.spriteEnemyFeetRun1;
			else if(_spriteFeet.sprite.Equals(gameManager.spritePlayerFeetRun2))
				_spriteFeet.sprite = gameManager.spriteEnemyFeetRun2;
			else if(_spriteFeet.sprite.Equals(gameManager.spritePlayerFeetRun3))
				_spriteFeet.sprite = gameManager.spriteEnemyFeetRun3;
		}
	}
	
	private void checkState() {
		if (transform.position.y < -200) {
			_state = STATE.Unavailable;
			return;
		}
		if (_state == STATE.Dying && isAlive ())
			_state = STATE.Standing;
		if(_state == STATE.Dying) {
			return;
		} else if(!isAlive()) {
			_state = STATE.Dying;
			_anim.SetTrigger("dying");
			_anim.SetLayerWeight (1, 0);
			parachute.close();
			StartCoroutine(ClearBody());
			return;
		}
		
		if(_isShield) {
			if(_anim.GetBool("falling"))
				return;
			if(_state != STATE.Shielding) {
				_anim.SetTrigger("shield");
			}
			_state = STATE.Shielding;
		} else if(_moveDirection != Vector3.zero) {
			_state = STATE.Running;
			_anim.SetFloat("speed", 1);
		} else if(_moveDirection == Vector3.zero) {
			_state = STATE.Standing;
			_anim.SetFloat("speed", 0);
		}
	}

	public bool isAvailable() {
		return _state != STATE.Unavailable;
	}

	IEnumerator ClearBody() {
		yield return new WaitForSeconds(5);
		_state = STATE.Unavailable;
	}
	
	private void doAI() {
		_nextCommandTimeRemaining -= Time.deltaTime;
		if(_nextCommandTimeRemaining <= 0) {
			_nextCommandTimeRemaining = Random.Range(1, 2);
			GameObject[] objs = GameObject.FindGameObjectsWithTag(target);
			GameObject obj = null;
			float targetX = 0;
			float selfX = transform.position.x;
			float distance = 0;
			//TODO temp solution for multiple obj with tag 'player'
			foreach(GameObject o in objs) {
				targetX = o.transform.position.x;
				float d = Mathf.Abs(targetX - selfX);
				if(d < distance || obj == null) {
					obj = o;
					distance = d;
				} 
				//Debug.Log("name=" + o.name + " objs=" + objs.Length + " pos=" + o.transform.position);
			}
			if(obj != null) {


				if(Random.Range(0, 1.0f) < 0.5) {
					stand();
					if(targetX < selfX)
						faceDirection(Vector3.left);
					else
						faceDirection(Vector3.right);
				} else if (Random.Range(0, 1.0f) < 0.5) {
					if(distance < 20) {

						_fireDirection = obj.transform.position;
						//_fireDirection.y += obj.collider.bounds.size.y / 2;
						shoot(_fireDirection);
					}
					stand();
				} else if(distance > Random.Range(30, 40)) {
					if(targetX < selfX)
						moveLeft();
					else
						moveRight();
				} else if(distance < Random.Range(5, 10)) {
					if(targetX < selfX)
						moveRight();
					else
						moveLeft();
				}
			}
		}
		
	}
	
	private void setArm() {
		
        // calculate the z rotation angle using the tan of y/x
		float angle = Mathf.Atan2(_fireDirection.y, _fireDirection.x) * Mathf.Rad2Deg;
        //_boneHandsRotation.y = boneAnimation.mLocalTransform.localEulerAngles.y;
		if(transform.localScale.x > 0) {
		} else {
			if(angle >= 90)
				angle = 180 - angle;
			if(angle <= -90)
				angle = -180 - angle;
			
		}
		
		_boneHandsRotation.z = angle;
		if(!_currentWeapon.isThrower()) {
	        // set the arm's rotation
	        _boneHands.eulerAngles = _boneHandsRotation;
		}
	}
	
	private void shoot() {
		setArm();
		if(!_currentWeapon.isThrower()) {
			if(_currentWeapon.shoot(_boneMuzzle.position, _fireDirection, target, _boneEjection.position))
				_anim.SetTrigger("attack");
			reloadIfNecessary();
		} else {
			if(((Thrower)_currentWeapon).preShoot())
				_anim.SetTrigger("attack");
		}
	}
	
	private void reloadIfNecessary() {
		if(!_isReloading && _currentWeapon.numberOfProjectile <= 0 && _currentWeapon.isReloadable()) {
			_anim.SetTrigger("reload");
			_isReloading = true;
		}
	}
	
	public void shoot(Vector3 direction) {
		_isAttacking = true;
		// get the screen position of the arm bone (relative to the camera)
        //Vector3 _transformScreenPosition = gameManager.cameraMain.WorldToScreenPoint(_boneHands.position);
		//Vector3 _transformScreenPosition = gameManager.cameraMain.WorldToScreenPoint(_boneHands.position);

        // get the offset vector from the arm's screen position to the mouse position
        _fireDirection = (direction - _boneHands.position);
	}
	
	public void stopAttack() {
		_isAttacking = false;
	}
	
	public void shootFlare() {
		//boneAnimation.Play("Flare");
	}
	
	public void shield() {
		_isShield = true;
	}
	
	public void unshield() {
		_isShield = false;
		equipWeapon(_currentWeapon);
	}
	
	public void stand() {
		_moveDirection = Vector3.zero;
	}
	
	public void moveLeft() {
		_moveDirection = new Vector3(-1, 0, 0);
	}
	
	public void moveRight() {
		_moveDirection = new Vector3(1, 0, 0);
	}
	
	public void faceDirection(Vector3 direction) {
		if (direction.x == 0)
			return;
		if (Mathf.Sign (transform.localScale.x) == Mathf.Sign (direction.x)) {
			//workaround - setting localScale will cause rigitbody.movePosition not working
			//so here to set face direction when sign changed
			return;
		}

		Vector3 theScale = transform.localScale;
		theScale.x = Mathf.Sign (direction.x) * Mathf.Abs(theScale.x);
		transform.localScale = theScale;
	}
	
	public void equipWeapon(Weapon weapon) {
		_currentWeapon = weapon;
		if(_currentWeapon != null) {
			_anim.SetTrigger(_currentWeapon.textureName);
			_isReloading = false;
		}
	}
	
	public bool isAlive() {
		return health > 0;
	}

	public void setHp(float hp) {
		health = hp;
		if (health < gameManager.soldierMaxHp) {
			healthBar.show ();
		} else {
			healthBar.hide ();
		}
		healthBar.progress = Mathf.Clamp01(health / gameManager.soldierMaxHp);
						
		if (soundDanger == null) return;
		if (health < 30 && !soundDanger.isPlaying) {
			soundDanger.Play ();
		} else if(health >= 30 && soundDanger.isPlaying) {
			soundDanger.Stop();
		}
	}
	
	public void hurt(float damage, float defense) {
		damage = damage * (1 - defense);
		setHp(health - damage);
		if(!isAlive() && role == Role.Enemy)
			gameManager.incKill();
	}
	
	public void OnCollisionEnter(Collision col) {
		switch (col.gameObject.tag) {
		case "tile":
			if(_anim.GetBool("falling")) {
				parachute.close();
			}
			break;
		}
	}

/*	public override void OnBoneCollisionEnter(Bone bone, Collision other) {
		if (other.transform.tag == "bullet") {
			string hit = bone.name;
	
			Bullet b = other.transform.GetComponent<Bullet> ();
			onHitByBullet(hit, b);
		}
	}*/

	public override void OnBoneTriggerEnter(Bone bone, Collider other) {
		if (other.tag == "bullet") {
			string hit = bone.name;

			Bullet b = other.GetComponent<Bullet>();
			onHitByBullet(hit, b);
		}
	}

	private void onHitByBullet(string hit , Bullet b) {
		if(isAlive() && !b.isAvailable() || b.target != tag || !isAlive())
			return;
		// check to see what object we collided with
		if (hit == "head") {
			hurt(b.damage, defenseHead);
			// play the sound of hitting the fridge
			if(!isAlive()) {
				gameManager.incHeadShot();
				Instantiate(prefHeadShot, _boneHead.position, Quaternion.identity);
			}
			audio.PlayOneShot(soundHit2);
			b.onHit(this);
		} else if(hit == "body") {
			hurt(b.damage, defenseBody);
			audio.PlayOneShot(soundHit1);
			b.onHit(this);
		} else if(hit == "feet") {
			hurt(b.damage, defenseFeet);
			audio.PlayOneShot(soundHit1);
			b.onHit(this);
		} else if(hit == "hand_right") {
			//shield
			b.onShield();
			audio.PlayOneShot(soundShield);
		}
		
		if(hit == "head" || hit == "body" || hit == "feet") {
			Vector3 hitPos = b.transform.position;
			Animator anim = (Animator)Instantiate(prefBlood, hitPos, Quaternion.identity);
			Vector3 scale = anim.transform.localScale;
			scale.y = hitPos.x > transform.position.x ? -scale.y : scale.y;
			anim.transform.localScale = scale;
			anim.SetTrigger("fx_" + Random.Range(1, 4));
		}
	}

	public void OnAnimationEvent(string e) {
		switch (e) {
		case "reload":
			_currentWeapon.reload();
			_isReloading = false;
			break;
		case "throw_grenade":
			audio.PlayOneShot(soundThrowGrenade);
			break;
		case "grenade":
			((Thrower)_currentWeapon).shoot(_boneMuzzle.position, _fireDirection);
			reloadIfNecessary();
			break;
		}
	}

	public void reset() {
		setHp(gameManager.soldierMaxHp);
		healthBar.hide ();
		_moveDirection = Vector3.zero;
		faceDirection(Vector3.right);
		_state = Soldier.STATE.Standing;
		unshield();
		stopAttack();
		Vector3 v = rigidbody.velocity;
		v.y = 0;
		rigidbody.velocity = v;
		_anim.SetTrigger ("reset");
		_anim.SetLayerWeight (1, 1);
		if (tag == "player") {
			parachute.open();
		}
	}
}

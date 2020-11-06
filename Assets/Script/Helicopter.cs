using UnityEngine;
using System.Collections;

public class Helicopter : Skeleton {
	
	public enum STATE {
		Moving,
		Standing,
		Unavailable
	}

	public Transform target;
	public Transform prefExplosion;

	public float speed;
	public int hp;
	public Weapon currentWeapon;
	private Transform _boneMuzzle;
	private Transform _boneGun;
	private float _smoothTime;
    private float _xVelocity = 0.0F;
	private float _yVelocity = 0.0F;
	private float _startTime;
	private float _journeyLength;
	private Vector3 _posFrom;
	private Vector3 _posTo;
	private STATE _state = STATE.Standing;
	private bool _isAttacking = false;
	private Vector3 _fireDirection;
	private Animator _anim;

	// Use this for initialization
	void Start () {
		_anim = GetComponent<Animator> ();

		_boneMuzzle = transform.Find ("body/gun/muzzle");
		_boneGun = transform.Find ("body/gun");
		reset ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_state == STATE.Unavailable)
			return;

		doAI();

		if(_state == STATE.Moving) {

			float x = Mathf.SmoothDamp(transform.position.x, _posTo.x, ref _xVelocity, _smoothTime);
			float y = Mathf.SmoothDamp(transform.position.y, _posTo.y, ref _yVelocity, _smoothTime);

			transform.position = new Vector3(x, y, 0);
			float t = Mathf.Abs(transform.position.x - _posTo.x) / 5.0f;
			t = t > 1 ? 1 : t;
			float speed = Mathf.Lerp(0, 1, t);
			_anim.SetFloat("speed", speed);
			//if(Mathf.Abs( x - _posTo.x) <= 10)

			/*float distCovered = (Time.time - _startTime) * speed;
			float fracJourney = distCovered / _journeyLength;
        	transform.position = Vector3.Lerp(_posFrom, _posTo, fracJourney);
			if(fracJourney > 1)
				_state = STATE.Standing;*/
		}
		
		if(_isAttacking && target != null) {

			Vector3 offset = Random.insideUnitSphere * 1f;
			offset.z = 0;
			faceDirection(_fireDirection);
			currentWeapon.shoot(_boneMuzzle.position + offset, _fireDirection, "player", _boneGun.position);
		}

		if (transform.position.y > 250 || transform.position.y < -100) {
			Destroy(gameObject);
		}
	}

	void LateUpdate() {
		if (_isAttacking) {
			// calculate the z rotation angle using the tan of y/x
			float angle = Mathf.Atan2 (_fireDirection.y, _fireDirection.x) * Mathf.Rad2Deg;
			//_boneHandsRotation.y = boneAnimation.mLocalTransform.localEulerAngles.y;
			if (transform.localScale.x > 0) {
			} else {
					if (angle >= 90)
							angle = 180 - angle;
					if (angle <= -90)
							angle = -180 - angle;

			}

			Vector3 v = Vector3.zero;
			v.z = angle;
			_boneGun.eulerAngles = v;
		}
	}
	
	private void doAI() {
		Vector3 pos = transform.position;
		Vector3 targetPos = target.position;
		float distance = Mathf.Abs(pos.x - target.position.x);
		if(distance < 42 && currentWeapon.numberOfProjectile > 0 && Random.Range(0, 10) < 10) {
			attack();
			_fireDirection = (target.position - _boneMuzzle.position);
		} else
			stopAttack();

		if(currentWeapon.numberOfProjectile <= 0) {

			pos.x -= 20;
			pos.y = 300;
			moveTo(pos);
		} else if (distance > 40) {
			targetPos.y = 9;
			targetPos.x = pos.x > targetPos.x ? targetPos.x + 40 : targetPos.x - 40;
			moveTo(targetPos);
		}
	}
	
	public void attack() {
		_isAttacking = true;
		_anim.SetLayerWeight (1, 1);
	}
	
	public void stopAttack() {
		_isAttacking = false;
		_anim.SetLayerWeight (1, 0);
	}
	
	public void moveTo(Vector3 pos) {

		_state = STATE.Moving;
		_posFrom = transform.position;
		_posTo = pos;

		_journeyLength = Vector3.Distance(_posFrom, _posTo);
		_startTime = Time.time;
		_smoothTime = _journeyLength / (speed * 2);
		faceDirection(_posTo - _posFrom);

	}
	
	public void faceDirection(Vector3 direction) {
        // set the local Y angle to 180 degrees if the direction is greater than zero, else keep the Y angle at zero.
		Vector3 localEulerAngles = transform.localEulerAngles;
        localEulerAngles.y = (direction.x > 0 ? 0 : 180.0f);
		transform.localEulerAngles = localEulerAngles;
	}

	public void OnCollisionEnter(Collision col) {
		if (_state != STATE.Unavailable)
			return;

		if (col.transform.tag == "tile") {
			_anim.SetTrigger("fall_n_break");
			Instantiate(prefExplosion, col.transform.position, Quaternion.identity);
			audio.Stop();
		}
	}

	public void OnTriggerEnter(Collider col) {
		if (_state == STATE.Unavailable)
			return;

		if (col.tag == "bullet") {
			Bullet b = (Bullet) col.GetComponent<Bullet>();
			b.available = false;
		}
	}

	public override void OnBoneTriggerEnter(Bone bone, Collider other) {
		if (_state == STATE.Unavailable)
			return;

		if (bone.name == "body" && other.tag == "bullet") {
			Bullet b = (Bullet) other.GetComponent<Bullet>();
			if(b.available && tag == b.target && b.piercable) {
				_anim.SetTrigger("broken");
				_state = STATE.Unavailable;
				stopAttack();
				gameObject.layer = LayerMask.NameToLayer("Item");

				//temp solution
				transform.tag = "unavailable";
			}
		}
	}

	public void OnAnimationEvent(string e) {
		if (e == "falling") {
			rigidbody.useGravity = true;
		}
	}

	public void reset() {
		_state = STATE.Standing;
		currentWeapon.fill ();
/*
		GameObject go = GameObject.Find ("Player");
		if(go != null)
			_target = go.transform;*/
	}
}

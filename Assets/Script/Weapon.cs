using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	
	public Projectile proj;
	public Rigidbody shell;
	public string textureName;
	public float fireRate;
	public int maxNumberOfProjectile;
	public int numberOfProjectileOneMagazine;
	public int numberOfProjectile;
	public int totalNumberOfProjectile;
	public int fillProjCount;
	public float damage = 30;
	
	public AudioClip soundFire;
	public AudioClip soundFill;
	public AudioClip soundReload;
	public AudioClip soundEmpty;
	
	private int numberOfProjectileOneShot = 1;
	protected float nextShotRemaining;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(nextShotRemaining > 0) {
			nextShotRemaining -= Time.deltaTime;
		}
	}
	
	public bool shoot(Vector3 pos, Vector3 direction, string target, Vector3 ejectionPos) {
		if(!isTriggerable())
			return false;
		nextShotRemaining = fireRate;
		
		if (obtainProjectiles () > 0) {
			//Quaternion.identity means no rotation
			Projectile p = (Projectile)Instantiate (proj, pos, Quaternion.identity);
			p.damage = damage;
			p.direction = direction;
			p.target = target;

			audio.PlayOneShot (soundFire);

			if (shell != null) {
					pos.z = 6;
					Rigidbody s = (Rigidbody)Instantiate (shell, ejectionPos, Quaternion.identity);
					float force = Random.Range (100, 200);
					s.AddForce ((direction.x > 0 ? Vector3.left : Vector3.right) * force + Vector3.up * force);
			}
			return true;
		} else {
			if(soundEmpty != null && !audio.isPlaying)
				audio.PlayOneShot (soundEmpty);
		}
		return false;
	}
	
	protected int obtainProjectiles() {
		if(numberOfProjectile > 0) {
			numberOfProjectile -= numberOfProjectileOneShot;
			return numberOfProjectileOneShot;
		}
		return 0;
	}
	
	public void fill() {
		totalNumberOfProjectile += fillProjCount;
		totalNumberOfProjectile = Mathf.Min(maxNumberOfProjectile, totalNumberOfProjectile);
		numberOfProjectile = numberOfProjectileOneMagazine;
		if(soundFill != null)
			audio.PlayOneShot(soundFill);
	}
	
	public void reload() {
		int count = totalNumberOfProjectile != -1 ? Mathf.Min(numberOfProjectileOneMagazine - numberOfProjectile, totalNumberOfProjectile) : numberOfProjectileOneMagazine;
		if(totalNumberOfProjectile != -1)
			totalNumberOfProjectile -= count;
		numberOfProjectile += count;
		numberOfProjectile = Mathf.Min(numberOfProjectile, numberOfProjectileOneMagazine);
		if(soundReload != null)
			audio.PlayOneShot(soundReload);

		nextShotRemaining = fireRate;
	}
	
	public bool isReloadable() {
		return (totalNumberOfProjectile > 0  || totalNumberOfProjectile == -1);
	}
	
	public bool isTriggerable() {
		return nextShotRemaining <= 0;
	}
	
	public bool isThrower() {
		return this is Thrower;
	}
}

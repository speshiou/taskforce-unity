using UnityEngine;
using System.Collections;

public class Thrower : Weapon {
	
	private float forceRatio = 60;
	
	public Thrower() {
		maxNumberOfProjectile = 5;
		numberOfProjectileOneMagazine = 1;
		numberOfProjectile = 1;
		totalNumberOfProjectile = 2;
		damage = 30;
	}

	public bool preShoot() {
		if(!isTriggerable())
			return false;
		nextShotRemaining = fireRate;
		return true;
	}
	
	public bool shoot(Vector3 pos, Vector3 direction) {
		if(obtainProjectiles() > 0) {
			direction.z = 0;
			
			//Quaternion.identity means no rotation
			Projectile p = (Projectile)Instantiate(proj, pos, Quaternion.identity);
			p.damage = damage;
			p.direction = direction;
			float d = Mathf.Sqrt(direction.sqrMagnitude);

			Debug.Log("d=" + d);
			p.rigidbody.AddForce(p.direction.normalized * d * forceRatio);

			if(shell != null) {
				pos.z = 6;
				Rigidbody s = (Rigidbody)Instantiate(shell, pos, Quaternion.identity);
				s.AddForce((direction.x > 0 ? Vector3.left : Vector3.right) * 5000 + Vector3.up * 5000);
			}
			return true;
		}
		return false;
	}
}

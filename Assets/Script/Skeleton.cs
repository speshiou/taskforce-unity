using UnityEngine;
using System.Collections;

public abstract class Skeleton : MonoBehaviour {

	public virtual void OnBoneCollisionEnter (Bone bone, Collision other) {
	}
	
	public virtual void OnBoneTriggerEnter (Bone bone, Collider other) {
	}
	
	public virtual void OnBoneTriggerEnter2D (Bone bone, Collider2D other) {
	}
}

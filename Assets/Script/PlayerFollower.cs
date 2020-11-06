using UnityEngine;
using System.Collections;

public class PlayerFollower : MonoBehaviour {
	
	/// <summary>
    /// A reference to the transform to follow (offset from the camera)
    /// </summary>
	public Transform target;
	public GameManager gameManager;
	
    /// <summary>
    /// The speed with which to follow the camera
    /// </summary>
	public float cameraFollowSpeed;

    /// <summary>
    /// The screen bounds that contain the camera
    /// </summary>
	public Rect cameraBounds;
	
	public float playerOffsetX;
	
	//shake effect
	private bool shaking = false;

	public float explosionEffectDuration = 1f;
	public float shakeDistance = 50;
	public Vector3 originalPos;
	private float explosionEffectEndTime;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!shaking)
			originalPos = transform.position;

		if(target.position.x + playerOffsetX > originalPos.x) {
		// make the camera follow smoothly with a slight delay in the player's movement
			//originalPos.x = Vector3.Lerp(originalPos, target.position, cameraFollowSpeed).x;
			Vector3 d = originalPos;
			originalPos.x = target.position.x + playerOffsetX;
			d = originalPos - d;
		// make sure the camera doesn't stray too far left or right
		//_position.x = Mathf.Max (_position.x, cameraBounds.xMin);
		//_position.x = Mathf.Min (_position.x, cameraBounds.xMax);
		//_position.y = Mathf.Max (_position.y, cameraBounds.yMin);
		//_position.y = Mathf.Min (_position.y, cameraBounds.yMax);
			gameManager.map.offset(d);
		}
		
		transform.position = originalPos;
		
		if(shaking)
			updateExplosionEffect();
	}
	
	private void updateExplosionEffect() {
		if(Time.time < explosionEffectEndTime) {
			Vector3 offset = Random.insideUnitSphere * shakeDistance;
			offset.z = 0;
			transform.position += offset;
		} else {
			shaking = false;
			transform.position = originalPos;
		}
	}
	
	public void playShakeEffect() {
		shaking = true;
	    explosionEffectEndTime = Time.time + explosionEffectDuration;
		originalPos = transform.position;
	}
}

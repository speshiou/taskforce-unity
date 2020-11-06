using UnityEngine;
using System.Collections;

public class ButtonHelper : MonoBehaviour {

	public Sprite spriteUp;
	public Sprite spriteDown;
	public Sprite spriteDisable;
	public SpriteRenderer spriteRenderer;

	private bool _enabled = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool handleTouchEvent(TouchPhase action) {
		if (spriteRenderer == null)
			return false;
		if (_enabled || spriteDisable == null) {
			if (action == TouchPhase.Began) {
					spriteRenderer.sprite = spriteDown;
			} else if (action == TouchPhase.Ended) {
					spriteRenderer.sprite = spriteUp;
					return true;
			} else if (action == TouchPhase.Canceled) {
					spriteRenderer.sprite = spriteUp;
			}
		} else if(spriteDisable != null) {

			if (action == TouchPhase.Began) {
				spriteRenderer.sprite = spriteDisable;
			} else if (action == TouchPhase.Ended) {
				spriteRenderer.sprite = spriteDisable;
				return true;
			} else if (action == TouchPhase.Canceled) {
				spriteRenderer.sprite = spriteDisable;
			}
		}
		return false;
	}

	public void setEnabled(bool enabled) {
		_enabled = enabled;
		handleTouchEvent (TouchPhase.Canceled);
	}
}

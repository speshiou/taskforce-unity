using UnityEngine;
using System.Collections;

public class Rank : MonoBehaviour {

	private Animator _anim;
	private SpriteRenderer _spriteRenderer;
	private Sprite _sprite;

	// Use this for initialization
	void Awake () {
		_anim = GetComponent<Animator> ();
		_spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setFlash(bool enable) {
		_anim.SetBool ("flash", enable);
	}

	public void setSprite(Sprite sprite) {
		if (sprite != null) {
			_sprite = sprite;
			_spriteRenderer.sprite = _sprite;
		}
	}
}

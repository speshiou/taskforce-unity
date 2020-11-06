using UnityEngine;
using System.Collections;

/// <summary>
/// This class uses an animation to simulate a GUI progress bar. This shows how you can use
/// the normalized time to set the progress value of an animation.
/// 
/// CAUTION: this is not the most efficient way to handle a progress bar. Use a GUI or 2D
/// package for this to reduce your draw calls.
/// </summary>
public class ProgressBar : MonoBehaviour {

	private SpriteRenderer progressBar;
	private Vector3 scale;

	private bool _visible = true;

	void Awake() {
		progressBar = GetComponent<SpriteRenderer> ();
		scale = progressBar.transform.localScale;
	}

    public float progress {
		set {
			float v = value;
			v = Mathf.Min (1, v);
			v = Mathf.Max (0, v);
			// Set the health bar's colour to proportion of the way between green and red based on the player's health.
			Color color = Color.Lerp(Color.green, Color.red, 1 - v);
			color.a = _visible ? 255:0;
			progressBar.material.color = color;
			// Set the scale of the health bar to be proportional to the player's health.
			progressBar.transform.localScale = new Vector3(scale.x * v, 1, 1);
		}
	}

	public void show() {
		_visible = true;
	}

	public void hide() {
		_visible = false;
	}

}

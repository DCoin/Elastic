using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {

	public Sprite pauseSplashSprite;

	private GameObject splash;
	private float timescale;

	private bool paused = false;

	void Start() {
		// grab and remember timescale
		this.timescale = Time.timeScale;

		splash = new GameObject("Pause Splash");
		var sr = splash.AddComponent<SpriteRenderer>();
		sr.sprite = pauseSplashSprite;
		splash.renderer.enabled = false;
	}

	void Update() {
		// Paused was pressed, do stuff
		if (ControllerManager.GetPauseButtonInput()) {
			if (paused) Unpause();
			else Pause();
		}
	}

	private void Pause() {
		Time.timeScale = 0.0f;

		var pos = Camera.main.transform.position;
		pos.z = -3.0f; // TODO Hardcoded distance to screen, eek
		splash.transform.position = pos; 

		splash.renderer.enabled = true;

		paused = true;
	}

	private void Unpause() {
		Time.timeScale = timescale;

		splash.renderer.enabled = false;

		paused = false;
	}
}

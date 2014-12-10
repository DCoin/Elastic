using UnityEngine;
using System.Collections;
using System.Linq;

public class BackgroundMusicWub : MonoBehaviour {
	// A "wub" is the wub you hear when the music goes wubwub

	public Sprite backgroundSprite;
	public Color baseColor = Color.blue;
	public Color wubColor = Color.green;
	public int audioFidelity = 512;

	private GameObject bg;
	private SpriteRenderer sr;

	void Start() {
		bg = new GameObject("Background");
		sr = bg.AddComponent<SpriteRenderer>();
		sr.sprite = backgroundSprite;

	}

	void Update () {
		// Translate background
		var pos = Camera.main.transform.position;
		pos.z = 5;
		bg.transform.position = pos;

		// Scale background
		var size = backgroundSprite.bounds.extents;
		var newScale = 
			new Vector3 (
				((Camera.main.orthographicSize * Camera.main.aspect) / size.x),
				(Camera.main.orthographicSize / size.y),
				1f);
		bg.transform.localScale = newScale;

		// Color background
		float[] data = new float[audioFidelity];
		AudioListener.GetOutputData(data, 0); // channel 0 ok?
		
		var intensity = data.Select(x => Mathf.Abs(x)).Average();

		sr.color = Color.Lerp(baseColor, wubColor, intensity);

	}
}

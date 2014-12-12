using UnityEngine;
using System.Collections;

public class RespawnGhost : MonoBehaviour {

	private SpriteRenderer sr;

	private float travelTime;
	private float lingerTime;
	private Vector2 start;
	private Vector2 destination;

	private Vector3 velocity = Vector3.zero;

	private float clock = 0.0f;

	public static GameObject CreateGhost(
			Vector2 start, 
			Vector2 finish, 
			float travelTime, 
			float lingerTime, 
			Sprite sprite) 
	{
		var go = new GameObject("Ghost");

		var sr = go.AddComponent<SpriteRenderer>();
		sr.sprite = sprite;

		var rg = go.AddComponent<RespawnGhost>();
		rg.start = start;
		rg.destination = finish;
		rg.travelTime = travelTime;
		rg.lingerTime = lingerTime;

		rg.sr = sr;

		go.transform.position = start;

		return go;
	}

	void Update () {
		// account for pauses
		if (Time.timeScale > 0)
			clock += Time.deltaTime;

		if (clock < travelTime) {
			transform.position = Vector2.Lerp(start, destination, clock / travelTime);
		}
		else if (clock < travelTime + lingerTime) {
			var c = sr.color;
			c.a = Mathf.SmoothStep(1.0f, 0.0f, (clock - travelTime) / lingerTime); 
			sr.color = c;
		}
		else {
			// We have waited enough, kill ourselves!
			Destroy(gameObject);
		}
	}
}

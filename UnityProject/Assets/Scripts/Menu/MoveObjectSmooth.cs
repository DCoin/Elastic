using UnityEngine;

public class MoveObjectSmooth : MonoBehaviour {
	

	private bool move = true;
	private Vector3 newPos;
	private Vector3 velocity = Vector3.zero;
	public float smoothTime = 0.3F;
	public Vector3 deltaTarget;
	
	// Use this for initialization
	void Start () {
		newPos = new Vector3 (transform.position.x + deltaTarget.x, transform.position.y + deltaTarget.y, transform.position.z + deltaTarget.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (move) {
			if (transform.position != newPos){
				// Smoothly move the camera towards that target position
				transform.position = Vector3.SmoothDamp (transform.position, newPos, ref velocity, smoothTime);
			} else {
				move = false;
				velocity = Vector3.zero;
			}
		}
	}
}

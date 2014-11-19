using UnityEngine;
using System.Collections;

public class MoveCameraScript : MonoBehaviour {

	[HideInInspector]
	public Vector3 targetPosition = Vector3.zero;
	[HideInInspector]
	public bool moveCamera = false;

	public float smoothTime = 0.3F;
	public float camerasize = 25.0F;
	private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		var LevelScript = GetComponent<DuelMode>(); 
		Camera.main.orthographicSize = camerasize;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (moveCamera) {
				// Smoothly move the camera towards that target position
				Camera.main.transform.position = Vector3.SmoothDamp (Camera.main.transform.position, targetPosition, ref velocity, smoothTime);
				if (Camera.main.transform.position == targetPosition) {
					moveCamera = false;
				}
			}
	}
}

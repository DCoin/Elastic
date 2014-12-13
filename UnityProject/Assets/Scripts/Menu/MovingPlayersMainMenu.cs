using UnityEngine;
using System.Collections;

public class MovingPlayersMainMenu : MonoBehaviour {

	private float moveDelay = 2.0f;
	private float lastMove;
	private bool move = true;
	private bool ready_to_anim = true;
	private Vector3 newPos;
	private Vector3 velocity = Vector3.zero;
	public float smoothTime = 0.3F;

	// Use this for initialization
	void Start () {
		
		moveDelay = Random.Range (0.1f, 1.5f);
		Invoke ("Move", moveDelay);
		newPos = new Vector3 (Random.Range (-8.0f, 6.0f), Random.Range (-6.0f, 6.0f), transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (move) {
			if (transform.position != newPos){
					// Smoothly move the camera towards that target position
					transform.position = Vector3.SmoothDamp (transform.position, newPos, ref velocity, smoothTime);
						} else {
							move = false;
				newPos = new Vector3 (Random.Range (-8.0f, 6.0f), Random.Range (-6.0f, 3.0f), transform.position.z);
							if (newPos.x + transform.position.x > 3 || newPos.x + transform.position.x < -8) {
					newPos = new Vector3 (newPos.x*-.5f, newPos.y, newPos.z);
							} 
							if (newPos.y + transform.position.y > 3 || newPos.y + transform.position.y < -6) {
					newPos = new Vector3 (newPos.x, newPos.y*-.5f, newPos.z);
							}
							Invoke ("Move", moveDelay);
							velocity = Vector3.zero;
							moveDelay = Random.Range (0.1f, 1.5f);
						}
				}
	}

	void Move() 
	{
			move = true;
	}
}

using UnityEngine;
using System.Collections;
using InControl;

public class PlayerController : MonoBehaviour {
	public float moveSpeed = 10f;
	public float jumpSpeed = 3f;

	public int controller = 0;
	public bool leftSide = true;

	private bool onGround = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Horizontal movement
		rigidbody2D.AddForce(Vector2.right * ControllerManager.GetHorizontalInput(controller, leftSide) * moveSpeed);

		// Jumping
		if (onGround){
			if (ControllerManager.GetJumpInputBool(controller, leftSide)) {
				rigidbody2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		// TODO check if it's platform, not just any object
		onGround = true;
	}

	void OnTriggerExit2D(Collider2D col) {
		// TODO check if it's platform, not just any object
		onGround = false;
	}
}

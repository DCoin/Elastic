using UnityEngine;
using System.Collections;
using InControl;

public class PlayerController : MonoBehaviour {

	public Collider2D jumpCollider;

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
		//if (ControllerManager.GetJumpInput(controller, leftSide)) print ("Left stick jump!");
		//if (ControllerManager.GetJumpInput(controller, !leftSide)) print ("Right stick jump!");

		// Horizontal movement
		rigidbody2D.AddForce(Vector2.right * ControllerManager.GetHorizontalInput(controller, leftSide) * moveSpeed);

		// Jumping
		if (onGround){
			if (ControllerManager.GetJumpInputBool(controller, leftSide)) {
				rigidbody2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
			}
		}

		/*
		var inputDevice = (InputManager.Devices.Count > controller) ? InputManager.Devices[controller] : null;
		if (inputDevice == null) {
			//TODO Do something appropriate
		} else {
			transform.Translate(Vector2.right * speed * Time.deltaTime * inputDevice.RightStickX.Value); // THIS IS SHIT AND MUST GO AWAY!!!
		}
		*/
	}

	void OnTriggerEnter2D(Collider2D col) {
		// TODO check if it's platform, not just any object
		Debug.Log("On ground");
		onGround = true;
	}

	void OnTriggerExit2D(Collider2D col) {
		// TODO check if it's platform, not just any object
		Debug.Log("Leaving ground");
		onGround = false;
	}
}

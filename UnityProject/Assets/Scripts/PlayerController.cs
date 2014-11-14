using UnityEngine;
using System.Collections;
using InControl;

public class PlayerController : MonoBehaviour {
	public float moveSpeed = 10f;
	public float acceleration = 10f;
	public bool impulse = false;
	public float jumpSpeed = 7f;
	public float jumpDelay = 0.5f;

	public float heavyMultiplier = 5;

	public int controller = 0;
	public bool leftSide = true;

	private bool onGround = false;
	private float baseMass;
	// We store the state of the heavy key here because inputControl.wasPressed() is not reliable
	private bool isHeavy = false;
	private float lastJump = 0f;

	// Use this for initialization
	void Start () {
		baseMass = rigidbody2D.mass;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Horizontal movement
		print("Velo: " + rigidbody2D.velocity.x);
		// TODO add if that checks if there is any input at all.
		var dir = ControllerManager.GetRightInputBool(controller, leftSide) ? 1 : -1; // Will always show left on no input but that doesnt matter as its 0.
		rigidbody2D.AddForce(Vector2.right * ControllerManager.GetHorizontalInput(controller, leftSide) * GetAdjustedAcceleration(rigidbody2D.velocity.x * dir), impulse?ForceMode2D.Impulse:ForceMode2D.Force);

		// Jumping
		// TODO varied jump
		if (onGround){
			if (ControllerManager.GetJumpInputBool(controller, leftSide)) {
				if (lastJump + jumpDelay < Time.fixedTime) { // TODO Unnest ifs?
					rigidbody2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
					lastJump = Time.fixedTime;
				}
			}
		}

		//Heavy
		if (ControllerManager.GetHeavyInputBool(controller, leftSide)) {
			if (!isHeavy) {
				rigidbody2D.mass = baseMass * heavyMultiplier;
				isHeavy = true;
			}
		} else {
			if (isHeavy) {
				rigidbody2D.mass = baseMass;
				isHeavy = false;
			}
		}
	}

	private float GetAdjustedAcceleration(float speed) {
		return (1 - speed / moveSpeed) * acceleration;
	}

	void OnTriggerEnter2D(Collider2D col) {
		// TODO check if it's platform, not just any object
		// Or rather have a non jump list? should we be able to jump on other players? others elasitic? jump of lethal platforms before dying to them?(no)
		onGround = true;
	}

	void OnTriggerExit2D(Collider2D col) {
		// TODO check if it's platform, not just any object
		onGround = false;
	}
}

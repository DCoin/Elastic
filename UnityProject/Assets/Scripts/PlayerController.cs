using UnityEngine;
using System.Collections;
using InControl;

public class PlayerController : MonoBehaviour {
	public float moveSpeed = 10f;
	public float jumpSpeed = 7f;
	public float jumpDelay = 0.5f;

	public float heavyMultiplier = 5;

	public int controller = 0;
	public bool leftSide = true;

	private bool onGround = false;
	private float baseMass;
	private bool isHeavy = false;
	private float lastJump = 0f;

	// Use this for initialization
	void Start () {
		baseMass = rigidbody2D.mass;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Horizontal movement
		rigidbody2D.AddForce(Vector2.right * ControllerManager.GetHorizontalInput(controller, leftSide) * moveSpeed);

		// Jumping TODO We still need a jump delay
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

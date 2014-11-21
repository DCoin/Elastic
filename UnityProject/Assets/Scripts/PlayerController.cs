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
	public float heavyDrag = 0.02f;
	public float heavyGScaleMult = 2;

	public int controller = 0;
	public bool leftSide = true;

	public Sprite stoneSprite;

	private bool isRolling; // TODO remove this and the part not used when we decide on a control scheme

	private Sprite normalSprite;

	//public PhysicsMaterial2D stoneMat;

	private bool onGround = false;
	private float baseMass;
	private float baseGScale;

	// We store the state of the heavy key here because inputControl.wasPressed() is not reliable
	private bool isHeavy = false;
	private float lastJump = 0f;

	// And adjustment to acceleration to make impulse and non impulse similar.
	private float impulseAdjust = 1 / 40;

	// The child used for rolling
	private GameObject roller;

	// Use this for initialization
	void Start () {
		var t = transform.Find ("Collider");
		roller = t == null ? null : transform.Find ("Collider").gameObject;
		isRolling = roller != null;
		if (isRolling) {
			baseMass = roller.rigidbody2D.mass;
			baseGScale = roller.rigidbody2D.gravityScale;
		} else {
			baseMass = rigidbody2D.mass;
			baseGScale = rigidbody2D.gravityScale;
		}
		normalSprite = GetComponent<SpriteRenderer> ().sprite;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Horizontal movement
		// TODO Is the Mathf check that checks if there is any input woth it?
		// TODO Should be based on the speed of the platform you are on.
		// TODO Add different air control?
		if (!isHeavy && !Mathf.Approximately(ControllerManager.GetHorizontalInput(controller, leftSide), 0f)) {
			var dir = ControllerManager.GetRightInputBool(controller, leftSide) ? 1 : -1; // Will always show left on no input but that doesnt matter as its 0.
			rigidbody2D.AddForce(Vector2.right * ControllerManager.GetHorizontalInput(controller, leftSide) * GetAdjustedAcceleration(rigidbody2D.velocity.x * dir), impulse?ForceMode2D.Impulse:ForceMode2D.Force);
		}

		// Jumping
		// TODO varied jump
		if (onGround){
			if (ControllerManager.GetJumpInputBool(controller, leftSide)) {
				if (lastJump + jumpDelay < Time.fixedTime) { // TODO Unnest ifs?
					rigidbody2D.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
					lastJump = Time.fixedTime;
				}
			}
			// Slow down if there is no input and we are on the ground
			if (isHeavy || Mathf.Approximately(ControllerManager.GetHorizontalInput(controller, leftSide), 0f)) {
				// We either need to always or never slow down more than heavy is already doing when heavy on the ground since heavy ignores input
				if (isRolling) {
					roller.rigidbody2D.velocity = new Vector2(roller.rigidbody2D.velocity.x / 1.2F, roller.rigidbody2D.velocity.y);
				} else {
					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x / 1.2F, rigidbody2D.velocity.y);
				}
			}
		}

		//Heavy
		if (ControllerManager.GetHeavyInputBool(controller, leftSide)) {
			if (!isHeavy) {
				if (isRolling) {
					roller.rigidbody2D.mass = baseMass * heavyMultiplier;
					roller.rigidbody2D.gravityScale = baseGScale * heavyGScaleMult;
				} else {	
					rigidbody2D.mass = baseMass * heavyMultiplier;
					rigidbody2D.gravityScale = baseGScale * heavyGScaleMult;
				}
				//collider2D.sharedMaterial = stoneMat;
				GetComponent<SpriteRenderer>().sprite = stoneSprite == null ? normalSprite : stoneSprite; // TODO change this to animation?
				isHeavy = true;
			}
		} else {
			if (isHeavy) {
				if(isRolling) {
					roller.rigidbody2D.mass = baseMass;
					roller.rigidbody2D.gravityScale = baseGScale;
				} else {
					rigidbody2D.mass = baseMass;
				    rigidbody2D.gravityScale = baseGScale;
				}
				//collider2D.sharedMaterial = null;
				GetComponent<SpriteRenderer>().sprite = normalSprite; // TODO change this to animation?
				isHeavy = false;
			}
		}
		if (isHeavy) {
			if(isRolling) {
				var vel = roller.rigidbody2D.velocity;
				vel.x *= 1 - heavyDrag;
				roller.rigidbody2D.velocity = vel;
			} else {
				var vel = rigidbody2D.velocity;
				vel.x *= 1 - heavyDrag;
				rigidbody2D.velocity = vel;
			}
		}
	}

	private float GetAdjustedAcceleration(float speed) {
		return (1 - speed / moveSpeed) * acceleration * (impulse?impulseAdjust:1);
	}

	void OnTriggerEnter2D(Collider2D col) {
		// TODO check if it's platform, not just any object
		// Or rather have a non jump list? should we be able to jump on other players? others elasitic? jump of lethal platforms before dying to them?(no)
		// Don't jump on own elastic and possibly not on coplayer
		if (col.CompareTag("Platform")){
			onGround = true;
		}
	}

	void OnTriggerStay2D(Collider2D col) {
		// TODO Do ground detection here to avoid bugging when hitting multiple platforms
	}

	void OnTriggerExit2D(Collider2D col) {
		// TODO check if it's platform, not just any object
		if (col.CompareTag("Platform")) {
			onGround = false;
			// TODO Check if still on another platform
		}
	}
}

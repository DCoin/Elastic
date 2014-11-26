﻿using UnityEngine;
using System.Collections;

// TODO this class does not handle cleanup of instantiated gameobjects yet

// TODO iris color set programatically

// TODO HATS!

public class EyeAnimator : MonoBehaviour {

	public Sprite eyeballSprite;
	public Sprite irisSprite;
	public Color irisColor = Color.blue;
	public HatManager.HatNames hat;

	private Sprite hatSprite;

	private int controllerID;
	private bool leftSide;

	private GameObject go_eyeball;
	private GameObject go_iris;
	private GameObject go_hat;

	private SpriteRenderer sr_eyeball;
	private SpriteRenderer sr_iris;
	private SpriteRenderer sr_hat;

	// how should velocity be scaled before moving hats accordingly?
	private const float HAT_MOVE_RATIO_X = 0.05f;
	private const float HAT_MOVE_RATIO_Y = 0.02f;

	// Use this for initialization
	void Start () {
		if (!eyeballSprite || !irisSprite) {
			Debug.LogError("Not all sprites are assigned.");
			enabled = false;
			return;
		}

		var playerController = gameObject.GetComponent<PlayerController>();

		// TODO use the built in unity things to check if there's a playercontroller
		if (!playerController) {
			Debug.LogError("No PlayerController on this GameObject.");
			enabled = false;
			return;
		}

		// Get info for what controller to listen for
		controllerID = playerController.controller;
		leftSide = playerController.leftSide;

		// Create objects for sprites to be attached to
		go_eyeball  = new GameObject();
		go_iris 	= new GameObject();
		go_hat		= new GameObject();

		// Names
		go_eyeball.name = "EyeballSprite";
		go_iris.name = "IrisSprite";
		go_hat.name = "HatSprite";

		// Child them to this gameobject
		go_eyeball.transform.parent = transform;
		go_iris.transform.parent = transform;
		go_hat.transform.parent = transform;

		// Zero them according to parents
		go_eyeball.transform.localPosition = Vector2.zero;
		go_iris.transform.localPosition = Vector2.zero;
		go_hat.transform.localPosition = Vector2.zero;

		// Spriterenderers
		sr_eyeball = go_eyeball.AddComponent<SpriteRenderer>();
		sr_iris = go_iris.AddComponent<SpriteRenderer>();
		sr_hat = go_hat.AddComponent<SpriteRenderer>();

		// Get sprite for hat from Hat manager and assign to spriterenderer
		ChangeHat(hat);

		// Assign remaining sprites
		sr_eyeball.sprite = eyeballSprite;
		sr_iris.sprite = irisSprite;

		// Set color of iris
		SetIrisColor(irisColor);

		// Sort in proper order
		sr_eyeball.sortingOrder = 1;
		sr_iris.sortingOrder = 2;
		sr_hat.sortingOrder = 3;
	}
	
	// We move the iris based on analog movement
	void LateUpdate () {
		// Split into several functions to declutter
		MoveIris();
		MoveHat();
	}

	// Move iris based on controller input
	private void MoveIris() {
		// TODO add an offset value, to prevent going to the edge
		
		// TODO squeeze Iris based on direction looking
		
		// Find value for max translation distance, mx and my
		var eBounds = sr_eyeball.bounds;
		var iBounds = sr_iris.bounds;
		
		float mx = eBounds.extents.x - iBounds.extents.x;
		float my = eBounds.extents.y - iBounds.extents.y;
		
		// Get controlstick axis values
		float x = ControllerManager.GetHorizontalInput(controllerID, leftSide);
		float y = ControllerManager.GetVerticalInput(controllerID, leftSide);
		
		// Translate eye according to values
		go_iris.transform.localPosition = new Vector2((x * mx), (y * my));
	}

	// move hat based on current velocity
	private void MoveHat() {
		// Find radius for hat origin
		float radius = sr_eyeball.bounds.extents.y;

		// Get velocity
		Vector2 velocity = rigidbody2D.velocity;

		// TODO simple naive implementation

		// Use velocity to find distance away from eyeball
		float xDist = -velocity.x * HAT_MOVE_RATIO_X;
		float yDist = -velocity.y * HAT_MOVE_RATIO_Y;

		// Find new point to put now, related to circumference
		Vector2 newPos = Vector2.ClampMagnitude(
			new Vector2(xDist, radius),
			radius+yDist);

		// Translate hat according to values
		go_hat.transform.localPosition = newPos;

		// Find rotation
		float newAngle = Vector2.Angle(Vector2.up, newPos);

		// flip rotation depending on direction
		newAngle *= (velocity.x > 0) ? 1 : -1;

		// Rotate hat according to values
		go_hat.transform.rotation = Quaternion.identity;
		go_hat.transform.Rotate(Vector3.forward, newAngle);

		// TODO squeeze hat based on y movement
	}

	// TODO the following two methods set two seperate values, seems prone to error

	/// <summary>
	/// Changes the hat worn by this eye.
	/// </summary>
	/// <param name="hatName">Hat name.</param>
	public void ChangeHat(HatManager.HatNames hatName) {
		this.hatSprite = HatManager.Instance.GetHatSprite(hatName);
		sr_hat.sprite = hatSprite;
	}

	/// <summary>
	/// Sets the color of the iris.
	/// </summary>
	/// <param name="color">Color for the iris.</param>
	public void SetIrisColor(Color color) {
		this.irisColor = color;
		sr_iris.color = color;
	}

	// Drawing in the scene view, for convenience
	void OnDrawGizmos() {
		Vector2 pos = transform.position;

		// Draw circles for the player
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(pos, eyeballSprite.bounds.extents.x);
	}
}

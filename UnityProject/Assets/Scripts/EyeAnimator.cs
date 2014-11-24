using UnityEngine;
using System.Collections;

// TODO this class does not handle cleanup of instantiated gameobjects yet

// TODO iris color set programatically

public class EyeAnimator : MonoBehaviour {

	public Sprite eyeballSprite;
	public Sprite irisSprite;

	private int controllerID;
	private bool leftSide;

	private GameObject go_eyeball;
	private GameObject go_iris;

	private SpriteRenderer sr_eyeball;
	private SpriteRenderer sr_iris;

	// Use this for initialization
	void Start () {
		if (!eyeballSprite || !irisSprite) {
			Debug.LogError("Not all sprites are assigned.");
			enabled = false;
			return;
		}

		var playerController = gameObject.GetComponent<PlayerController>();

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

		// Names
		go_eyeball.name = "EyeballSprite";
		go_iris.name = "IrisSprite";

		// Child them to this gameobject
		go_eyeball.transform.parent = transform;
		go_iris.transform.parent = transform;

		// Zero them according to parents
		go_eyeball.transform.localPosition = Vector2.zero;
		go_iris.transform.localPosition = Vector2.zero;

		// Spriterenderers
		sr_eyeball = go_eyeball.AddComponent<SpriteRenderer>();
		sr_iris = go_iris.AddComponent<SpriteRenderer>();

		// Assign sprites
		sr_eyeball.sprite = eyeballSprite;
		sr_iris.sprite = irisSprite;

		// Sort to proper layers
		sr_eyeball.sortingOrder = 1;
		sr_iris.sortingOrder = 2;


	}
	
	// We move the iris based on analog movement
	void LateUpdate () {
		// Find value for max translation distance, mx and my
		var eBounds = sr_eyeball.bounds;
		var iBounds = sr_iris.bounds;

		float mx = eBounds.extents.x - iBounds.extents.x;
		float my = eBounds.extents.y - iBounds.extents.y;

		// Get controlstick axis values
		float x = ControllerManager.GetHorizontalInput(controllerID, leftSide);
		float y = ControllerManager.GetVerticalInput(controllerID, leftSide);

		// Translate eye according to values
		go_iris.transform.localPosition = new Vector2((x * eBounds.extents.x), (y * eBounds.extents.y));
	}

	// Drawing in the scene view, for convenience
	void OnDrawGizmos() {
		Vector2 pos = transform.position;

		// Draw circles for the player
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(pos, 0.5f);
	}
}

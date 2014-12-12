using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RopeCasting : MonoBehaviour {

	public GameObject p1;
	public GameObject p2;
	// The distance that the rope should be from the colliders
	public float ropeOffset = 0.1f;
	public float minRopeDist = 2;
	public float acceleration = 0.1f;
	public bool squaredPower = false;
	public Material ropeMaterial;
	public float ropeWidth = 0.15f;
	private float softCap = 10;
	public bool destroyOnFailedRopeCast = true;

	public delegate void KillAction ();
	public KillAction killActions;

	private float speedMult = 15f;
	private float cornerDrag = 0f;
	private float length = 0f;
	private int segCount = 0;
	private HashSet<Collider2D> crossColliders;
	private bool isInitialized = false;
	private RopeCastingSegment ropePath;

	void Start () {
		// We don't want to initialize if the rope is not properly setup
		if (p1 == null || p2 == null) {
			gameObject.SetActive(false);
		} else {
			InitializeSegments ();
		}
	}

	private void InitializeSegments() {
		if (isInitialized) {
			Debug.LogError("Rope was already initialized");
			return;
		}
		if (gameObject.GetComponentsInChildren<RopeCastingSegment> ().Length != 0) {
			Debug.LogError("InitializeSegments was called while it still has segments.");
			DestroySegments();
		}

		// TODO Fix hack to find the real player object (Which doen't roll)
		var p1c = p1.GetComponent<PlayerController> ();
		var p2c = p2.GetComponent<PlayerController> ();
		var p1col = p1c != null ? p1c.roller.collider2D : p1.collider2D;
		var p2col = p2c != null ? p2c.roller.collider2D : p2.collider2D;
		var lastEnd = RopeCastingSegment.NewEndSeg (this, p2.transform.position, p2col); //TODO change the child order? We could do this to order the update order if needed.
		ropePath = RopeCastingSegment.NewSeg (this, p1.transform.position, lastEnd, p1col, 0);
		crossColliders = null;
		GetCrossColliders ();
		isInitialized = true;
	}

	public void Activate () {
		gameObject.SetActive (true);
		InitializeSegments ();
	}

	public void ResetPosition() {
		// TODO Check if there is any platforms on the ropes path
		DestroySegments (); // TODO The objects are not destroyed imidialtly and initialize will see them.
		InitializeSegments ();
	}

	private void DestroySegments() {
		foreach (var o in GetComponentsInChildren<RopeCastingSegment>()) {
			Destroy (o.gameObject);
		}
		isInitialized = false;
	}

	void FixedUpdate () {
		if (!gameObject.activeSelf) Debug.LogError("Mother FixedUpdate while inactive");

		// Check if any dynamic colliders on ropePath has moved and update those + check if affected points have changed.
		// Check if end points are dissolved
		// Calculate force based on length of ropePath. Use constant force on the players (colliders) rigidbody.
		var lastSeg = UpdateCounts ();

		// Apply forces
		var forceP1 = CalcForce2 (length, segCount - 2, GetDirectionalSpeed(p1.rigidbody2D.velocity, ropePath.Vector().normalized));
		var forceP2 = CalcForce2 (length, segCount - 2, GetDirectionalSpeed(p2.rigidbody2D.velocity, ropePath.Vector().normalized));
		p1.rigidbody2D.AddForceAtPosition(ropePath.Vector().normalized * forceP1 * forceP1, p1.transform.position, ForceMode2D.Impulse);
		p2.rigidbody2D.AddForceAtPosition(lastSeg.Vector().normalized * -forceP2 * forceP2, p2.transform.position, ForceMode2D.Impulse); // This vector goes the wrong way thus -force
	}

	// Bad name i know...
	private RopeCastingSegment UpdateCounts() {
		// Count segments and length
		var segment = ropePath;
		float len = 0;
		int i = 1;
		var lastSeg = ropePath;
		while (!segment.IsEnd()) {
			len += segment.Length();
			i++;
			lastSeg = segment;
			segment = segment.end;
		}
		length = len;
		segCount = i;
		return lastSeg;
	}

	public HashSet<Collider2D> GetCrossColliders ()
	{
		if (crossColliders != null) return crossColliders;
		crossColliders = new HashSet<Collider2D> ();
		var squad = p1.GetComponentInParent<Squad> ();
		if (squad != null) {
			foreach (var col in squad.GetComponentsInChildren<Collider2D> ()) {
				if (col.gameObject.layer == 11) crossColliders.Add(col);
			}
		}
		squad = p2.GetComponentInParent<Squad> ();
		if (squad != null) {
			foreach (var col in squad.GetComponentsInChildren<Collider2D> ()) {
				if (col.gameObject.layer == 11) crossColliders.Add(col);
			}
		}
		return crossColliders;
	}

	public void KillRope ()
	{
		// We don't want to kill if the rope is already dead.
		if (!gameObject.activeSelf) return;
		DestroySegments ();
		if (killActions != null) {
			killActions ();
		}
		gameObject.SetActive (false);
	}

	private float GetDirectionalSpeed(Vector2 velocity, Vector2 direction) {
		return Vector3.Project(velocity, velocity).magnitude;
	}
	
	private float TargetSpeed (float ropeLength, int corners) {
		return (ropeLength - minRopeDist) * speedMult * Mathf.Pow(1 - cornerDrag, corners);
	}
	
	private float CalcForce (float ropeLength, int corners, float directionalSpeed) {
		var target = TargetSpeed (ropeLength, corners);
		if (directionalSpeed > target) return 0;
		return (target - directionalSpeed) * acceleration;
	}
	
	private float CalcForce2 (float ropeLength, int corners, float directionalSpeed) {
		return GetRopeStretch(ropeLength) * acceleration * (squaredPower ? GetRopeStretch(ropeLength) : 1);
	}
	
	private float CalcForce3 (float ropeLength, int corners, float directionalSpeed) {
		var stretch = GetRopeStretch (ropeLength);
		if (stretch < softCap)
			return stretch * acceleration;
		else
			return (stretch + (stretch - softCap) * (stretch - softCap) ) * acceleration;
	}

	private float GetRopeStretch(float ropeLength) {
		var rope = ropeLength - minRopeDist;
		return rope < 0 ? 0 : rope;
	}

	void OnValidate() { // OnEnable or both???
		// TODO Check that there is no collider between p1 and p2
	}
}

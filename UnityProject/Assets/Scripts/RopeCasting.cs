using UnityEngine;
using System.Collections;
using System;

//[RequireComponent (typeof (LineRenderer))]
public class RopeCasting : MonoBehaviour {

	public GameObject p1;
	public GameObject p2;
	// The distance that the rope should be from the colliders
	public float ropeOffset = 0.1f;
	public float minRopeDist = 2;
	public float acceleration = 0.1f;
	public Material ropeMaterial;
	public float ropeWidth = 0.15f;
	private float speedMult = 15f;
	private float cornerDrag = 0f;
	private float length = 0f;
	private int segCount = 0;

	private RopeCastingSegment ropePath;

	void Start () { // TODO is awake better?
		InitializeSegments ();
	}

	private void InitializeSegments() {
		// TODO Fix hack to find the real player object (Which doen't roll)
		var lastEnd = RopeCastingSegment.NewEndSeg (this, p2.transform.position, p2.transform.parent.collider2D); //TODO change the child order? We could do this to order the update order if needed.
		ropePath = RopeCastingSegment.NewSeg (this, p1.transform.position, lastEnd, p1.transform.parent.collider2D, 0);
	}

	public void ResetPosition() {
		// TODO Check if there is any platforms on the ropes path
		DestroySegments ();
		InitializeSegments ();
	}

	private void DestroySegments() {
		foreach (var o in GetComponentsInChildren<RopeCastingSegment>()) {
			Destroy (o.gameObject);
		}
	}

	void FixedUpdate () {
		// Check if any dynamic colliders on ropePath has moved and update those + check if affected points have changed.
		// Check if end points are dissolved
		// Calculate force based on length of ropePath. Use constant force on the players (colliders) rigidbody.

		var lastSeg = UpdateCounts ();

		// Apply forces
		var forceP1 = CalcForce2 (length, segCount - 2, GetDirectionalSpeed(p1.rigidbody2D.velocity, ropePath.Vector().normalized));
		var forceP2 = CalcForce2 (length, segCount - 2, GetDirectionalSpeed(p2.rigidbody2D.velocity, ropePath.Vector().normalized));
		p1.rigidbody2D.AddForce(ropePath.Vector().normalized * forceP1 * forceP1, ForceMode2D.Impulse);
		p2.rigidbody2D.AddForce(lastSeg.Vector().normalized * -forceP2 * forceP2, ForceMode2D.Impulse); // This vector goes the wrong way thus -force
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
		return GetRopeStretch(ropeLength) * acceleration;
	}

	private float GetRopeStretch(float ropeLength) {
		var rope = ropeLength - minRopeDist;
		return rope < 0 ? 0 : rope;
	}

	void OnValidate() { // OnEnable or both???
		// TODO Check that there is no collider between p1 and p2
	}
}

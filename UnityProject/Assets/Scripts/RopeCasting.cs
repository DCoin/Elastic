using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (LineRenderer))]
public class RopeCasting : MonoBehaviour {

	public GameObject p1;
	public GameObject p2;
	// The distance that the rope should be from the colliders
	public float ropeOffset = 0.1f;
	public float minRopeDist = 2;
	public float acceleration = 0.1f;
	public float speedMult = 0.1f;
	public float cornerDrag = 0f;

	private RopeCastingSegment ropePath;
	private LineRenderer lRen;

	void Start () { // TODO is awake better?
		var lastEnd = RopeCastingSegment.NewEndSeg (this, p2.transform.position); //TODO change the child order? We could do this to order the update order if needed.
		ropePath = RopeCastingSegment.NewSeg (this, p1.transform.position, lastEnd, null, 0);
		lRen = GetComponent<LineRenderer> ();
	}

	void FixedUpdate () {
		// Check if any dynamic colliders on ropePath has moved and update those + check if affected points have changed.
		// Check if end points are dissolved
		// Calculate force based on length of ropePath. Use constant force on the players (colliders) rigidbody.

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

		// Apply forces
		var forceP1 = CalcForce (len, i - 2, GetDirectionalSpeed(p1.rigidbody2D.velocity, ropePath.Vector().normalized));
		var forceP2 = CalcForce (len, i - 2, GetDirectionalSpeed(p2.rigidbody2D.velocity, ropePath.Vector().normalized));
		p1.rigidbody2D.AddForce(ropePath.Vector().normalized * forceP1 * forceP1, ForceMode2D.Impulse);
		p2.rigidbody2D.AddForce(lastSeg.Vector().normalized * -forceP2 * forceP2, ForceMode2D.Impulse); // This vector goes the wrong way thus -force
		
		// Update the line renderer
		lRen.SetVertexCount (i);
		segment = ropePath;
		i = 0;
		while (segment != null) {
			lRen.SetPosition(i, segment.start);
			segment = segment.end;
			i++;
		}
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

	void OnValidate() { // OnEnable or both???
		// Check that there is no collider between p1 and p2
	}
}

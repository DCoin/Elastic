using UnityEngine;
using System.Collections;

public class RopeCastingSegment : MonoBehaviour {

	private RopeCasting mother;
	// The point the rope goes through. Should be offset slightly compared to the corner of the collider

	private Vector2 startOffset;// Use this to account for moving platforms AND PLAYERS TATADADADADAD
	//TODO TODO TODO!!!

	//public Vector2 start;
	// The next segment on the rope. Will be null if it is the last.
	public RopeCastingSegment end;
	private bool isEnd;
	// The collider near start that initiated the creation of this segment. Will be null if it is the first og last element.
	private Collider2D col;
	// TODO col has lost its meaning fix!

	// Did the bend between this and end's vector go right?
	private float bendsCross;
	// The collider that defines this segment.
	private EdgeCollider2D eCol;

	public static RopeCastingSegment NewSeg(RopeCasting mother, Vector2 start, RopeCastingSegment end, Collider2D col, float bendsCross) {
		var gObj = new GameObject("RopeSegment");
		gObj.transform.parent = mother.transform;

		var nxt = gObj.AddComponent<RopeCastingSegment>();
		nxt.mother = mother;
		nxt.end = end;
		nxt.col = col;
		nxt.startOffset = nxt.col.transform.InverseTransformPoint(start);
		nxt.bendsCross = bendsCross;
		nxt.eCol = gObj.AddComponent<EdgeCollider2D>();
		nxt.eCol.isTrigger = true;
		nxt.IgnoreCollisions ();
		nxt.isEnd = false;

		// TODO Hacked to always add rigidbody so that it will detect when moving platforms hits it.
		// TODO RopeCast instead when you break instead? !?!?!? its not actualy because of moving platforms!!!
		if (col == null || end.isEnd || true) { // in this case this is the first or second to last segment
			// TODO add a seperate signifier for col == null
			var rig = gObj.AddComponent<Rigidbody2D> ();
			rig.gravityScale = 0; // TODO fix this hack to make it collide ? (it will not detect collision if there is no non kinematic rigidbodies involved).
		}
		nxt.UpdateECol();
		return nxt;
	}

	public static RopeCastingSegment NewEndSeg(RopeCasting mother, Vector2 start, Collider2D col) { // TODO make it an enum instead of this? This is not functional though :S
		var gObj = new GameObject("RopeSegmentEnd");
		gObj.transform.parent = mother.transform;

		var nxt = gObj.AddComponent<RopeCastingSegment>();
		nxt.mother = mother;
		nxt.bendsCross = 0; // This value should never be used on ends
		nxt.col = col;
		nxt.startOffset =  nxt.col.transform.InverseTransformPoint(start);
		nxt.isEnd = true;
		return nxt;
	}

	// TODO Move this to its own function and let ropecasting control when they are updated?
	void FixedUpdate () {
		// Check if the link at the end of this segment is in an illegal position.
		if (!isEnd && !end.isEnd) {
			var lc = Physics2D.Linecast (end.GetStart(), end.GetStart(), 1 << 9); // TODO This test might be excesive
			if (lc.collider != null) {
				Debug.LogError("Start is inside a collider");
				DestroyBend();
			}
		}
		// TODO Check if start is inside a collider?
		/* This should not be needed anymore
		// Update ends that follow players.
		if (isEnd) {
			start = mother.p2.transform.position;
		} else if (col == null) { // It is the first segment TODO add a seperate signifier for this?
			start = mother.p1.transform.position;
		}
		*/
		UpdateECol ();

		CheckBend();
	}

	// Check the bend at the end of this segment eg. the bend with a corner in end.start
	private void CheckBend() {
		// Check if the rope went back over 180 deg.
		if (!isEnd && !end.isEnd) { // Don't check if its the end or pointing at the end.
			var cross = Vector3.Cross(Vector(),end.Vector());
			// Do they have a different cross and non of them are zero?
			if (cross.z * bendsCross < 0 - Mathf.Epsilon) {
				DestroyBend();
			}
		}
	}

	// Destroy the bend at the end of this segment eg. the bend with a corner in end.start
	private void DestroyBend() {
		// I assume i dont have to touch: mother col eCol start isEnd
		bendsCross = end.bendsCross;
		var oldEnd = end;
		end = end.end;
		Destroy(oldEnd.gameObject);
		
		// If we just deleted the segment with a rigidbody we need to add it to ourself
		if (end.isEnd && rigidbody2D == null) {
			var rig = gameObject.AddComponent<Rigidbody2D> ();
			rig.gravityScale = 0; // TODO This hack again...
		}
		
		UpdateECol();
		
		// TODO Check if any of the other ends are good?
	}

	void UpdateECol() {
		// TODO this could be done once for the segments that don't move
		// TODO Do we need the inverser transforms?
		if (!isEnd) 
		eCol.points = new Vector2[] {GetStart(), end.GetStart()};
	}

	void IgnoreCollisions ()
	{
		Physics2D.IgnoreCollision (eCol, mother.p1.collider2D);
		var p1 = mother.p1.transform.parent.GetComponent<PlayerController> (); // TODO avoid this hack.
		if (p1 != null) Physics2D.IgnoreCollision (eCol, p1.collider2D);

		Physics2D.IgnoreCollision (eCol, mother.p2.collider2D);
		var p2 = mother.p2.transform.parent.GetComponent<PlayerController> (); // TODO avoid this hack
		if (p2 != null) Physics2D.IgnoreCollision (eCol, p2.collider2D);
	}

	public bool IsEnd() {
		return isEnd;
	}

	// Dont call this on the last segment!
	public Vector2 Vector() {
		return end.GetStart() - GetStart();
	}

	public float Length() {
		return isEnd ? 0 : Vector2.Distance (GetStart(), end.GetStart());
	}
	
	void OnTriggerStay2D (Collider2D hitCol) {
		if (hitCol.gameObject.layer != 9) return; // Hack to ignore all but platforms
		ropeCast ();
	}

	private void ropeCast () {
		// TODO Is there a way to avoid doing this twice?
		// We could save all colliders hit and then do this in FixedUpdate on a limited layer if it takes to much time otherwise.
		var hits = Physics2D.LinecastAll (GetStart(), end.GetStart(), 1 << 9); // 1 << 9 means layer 9 which is platforms
		var hits_ = Physics2D.LinecastAll (end.GetStart(), GetStart(), 1 << 9); // 1 << 9 means layer 9 which is platforms

		if (hits.Length == 0) {
			// This happens when we are checking if a new segment is ok
			// TODO Find out why this also happens on a line that triggered the trigger. Is it just a corner?
			return;
		}

		if (hits.Length != hits_.Length) {
			Debug.LogError("The ropeCast did not hit the same amount of colliders.");
			return;
		}

		if (Mathf.Approximately(hits[0].fraction, 0)) {
			Debug.LogError("Illegal RopeCast, " + hits[0].point + " is inside a collider");
			return;
		}

		if (Mathf.Approximately(hits_[0].fraction, 0)) {
			Debug.LogError("Illegal RopeCast, " + hits_[0].point + " is inside a collider");
			return;
		}

		// TODO Find a square
		var hit1 = hits [0];
		//Find the other side of the collider of hit1
		RaycastHit2D hit2 = new RaycastHit2D();
		foreach (var hit in hits_) {
			if (hit1.collider == hit.collider) hit2 = hit;
		}

		if (hit1.collider != hit2.collider) {
			Debug.LogError("Did not find the other side of the collider");
			return;
		}

		// Split by the first collider hit. It will check and make sure the others are ok too
		DefineSplit(hit1, hit2); // We let split segment check if the segments it created are ok.
		// TODO Check that the bends are still OK Sould not be needed?!?!?
	}

	// This function takes to points on a single collider checks if they are parallel and if they are finds appropriate sides to find corners with.
	// hit1 and hit2 should be on the same collider!
	private void DefineSplit (RaycastHit2D hit1, RaycastHit2D hit2) {
		if (hit1.collider != hit2.collider) {
			Debug.LogError("DefineSplit should only be called with the same collider");
		}

		// TODO Do something else for polygon circle etc.
		if (!(hit1.collider is BoxCollider2D)) {
			Debug.LogError("DUDE I DID ONLY SUPPORT BOX COLLIDERS WTF MAN!?!?!");
			return;
		}
		// The angle between the normals of the surfaces hit on the collider
		var ang = Vector2.Angle(hit1.normal, hit2.normal); // TODO use dot/cross prod?
		if (Mathf.Approximately(ang, 180) || Mathf.Approximately(ang, 0)) {
			// The lines are parallel so we need to find the middle line
			Vector3 mid = (hit1.point + hit2.point) / 2;
			var normal = Quaternion.AngleAxis(90,new Vector3(0,0,-1)) * (hit2.point-hit1.point); // This wil fail if we use edge colliders
			var ray0 = mid + (normal*1000);
			var ray0_ = mid + (normal*-1000);

			// TODO Limit to one collider if performance is an issue
			// TODO MAKE A SINGLE LAYER !!!!!

			var oldLayer = hit1.collider.gameObject.layer;
			hit1.collider.gameObject.layer = 10; // TODO Change to signifie RopeCast Layer
			var hits = Physics2D.LinecastAll(ray0, mid, 1 << 10); // TODO Change all 1<<10 / 1<<9 to look up platform / RopeCast layer
			var hits_ = Physics2D.LinecastAll(ray0_, mid, 1 << 10);
			hit1.collider.gameObject.layer = oldLayer;

			// Split the segments by the line that is closest and one of the corners.
			// splitSegment will check if the split is ok and figure out that the other corner needs a split too.
			if (hits[hits.Length-1].fraction > hits_[hits_.Length-1].fraction) { // We use fraction because distance doesn't work
				SplitSegment(hits[hits.Length-1], hit1);
			} else {
				SplitSegment(hits_[hits_.Length-1], hit1);
			}
		} else {
			SplitSegment (hit1, hit2);
		}
	}
	
	// hit1 and hit2 should be on the same collider!
	private void SplitSegment (RaycastHit2D hit1, RaycastHit2D hit2) {
		if (hit1.collider != hit2.collider) {
			Debug.LogError("SplitSegment should only be called with the same collider");
		}

		var ang = Vector2.Angle(hit1.normal, hit2.normal);
		// Find which normal is the first in a clockwise rotation so that the angle is the smallest
		// and use this to calculate a vector that is exactly between the two normals.
		var cross = Vector3.Cross(hit1.normal, hit2.normal);
		Vector3 dir = Quaternion.AngleAxis(ang/2, new Vector3(0,0,-1)) * ((cross.z < 0) ? hit1.normal : hit2.normal);
		
		// Calculate the corner of the collider by intersect the rays from the points with vectors perpendicular to the normals.
		Vector3 p1 = hit1.point;
		Vector3 p2 = hit2.point;
		Vector3 v1 = Quaternion.AngleAxis(90,new Vector3(0,0,-1)) * hit1.normal;
		Vector3 v2 = Quaternion.AngleAxis(90,new Vector3(0,0,-1)) * hit2.normal;
		Vector3 corner = Math2D.LineIntersectionPoint(p1, p1+v1, p2, p2+v2);
		// TODO Check how far corner is from the line eg did we get the right corner?
		
		// Calculate a point outside the collider with a given offset
		var ropePoint = corner + (dir.normalized * mother.ropeOffset);

		// Check if ropePoint is inside a collider and move it out in that case
		var lc = Physics2D.Linecast (ropePoint, ropePoint, 1 << 9);// TODO WHY THE FUCK DOES THIS MAKE A STACKOVERFLOW???
		int i = 1;
		while (lc.collider != null && i <= 3) {
			Debug.LogError("RopePoint was set inside a collider: Moving it out nr of time: " + i);
			ropePoint = corner + (dir.normalized * mother.ropeOffset * i++);
			lc = Physics2D.Linecast (ropePoint, ropePoint, 1 << 9);
		}

		end = NewSeg(mother, ropePoint, end, hit1.collider, bendsCross);
		// The new segment is now in control of the line this last hat so it inherits bendcross while this takes the new cross
		bendsCross = cross.z;
		UpdateECol();
		// Check if the new ropes are ok
		// TODO Should this be done in ropeCast?
		ropeCast ();
		end.ropeCast ();
		// TODO hitCol has lost its meaning fix!
	}

	public Vector2 GetStart() {
		return col.transform.TransformPoint(startOffset);
	}

	void OnDrawGizmos() {
		Gizmos.DrawSphere (GetStart(), 0.05f);
	}
}

using UnityEngine;
using System.Collections;
using System.Linq;

public class RopeCastingSegment : MonoBehaviour {

	private RopeCasting mother;
	// The point the rope goes through. Should be offset slightly compared to the corner of the collider

	// Where the rope goes through in relation to col
	private Vector2 startOffset;

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
	// The LineRenderer
	private LineRenderer lr;
	// The layers that rope will collider with. Should match the collision matrix.
	// In this case only Platforms and Players Rope Colliders.
	private const int ropeCollisionMask = (1<<9) | (1<<11);

	public static RopeCastingSegment NewSeg(RopeCasting mother, Vector2 start, RopeCastingSegment end, Collider2D col, float bendsCross) {
		var gObj = new GameObject("RopeSegment");
		gObj.transform.parent = mother.transform;
		gObj.layer = 12;

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
			rig.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		}

		// Add Linerenderer
		var lr = gObj.AddComponent<LineRenderer> ();
		lr.SetVertexCount (2);
		lr.SetWidth (mother.ropeWidth, mother.ropeWidth);
		lr.material = mother.ropeMaterial;
		nxt.lr = lr;

		nxt.UpdateECol();
		return nxt;
	}

	public static RopeCastingSegment NewEndSeg(RopeCasting mother, Vector2 start, Collider2D col) { // TODO make it an enum instead of this? This is not functional though :S
		var gObj = new GameObject("RopeSegmentEnd");
		gObj.transform.parent = mother.transform;
		gObj.layer = 12;

		var nxt = gObj.AddComponent<RopeCastingSegment>();
		nxt.mother = mother;
		nxt.bendsCross = 0; // This value should never be used on ends
		nxt.col = col;
		nxt.startOffset = nxt.col.transform.InverseTransformPoint(start);
		nxt.isEnd = true;
		return nxt;
	}

	// TODO Move this to its own function and let ropecasting control when they are updated?
	void FixedUpdate () {
		if (!mother.gameObject.activeSelf) {
			Debug.LogError("FixedUpdate while dead!!!!");
		}

		// Check if the link at the end of this segment is in an illegal position.
		if (!isEnd && !end.isEnd) {
			var lc = Physics2D.Linecast (end.GetStart(), end.GetStart(), 1 << 9); // We do not check for players as that might break it // TODO This test might be excesive
			if (lc.collider != null) {
				Debug.LogError("Start is inside a collider. Should we jus kill here?");
				DestroyBend(); // TODO Kill instead?
			}
			if (!end.col.gameObject.activeInHierarchy) {
				Debug.Log("The col this bends around turned inacctive");
				DestroyBend();
			}
		}
		UpdateECol ();

		CheckBend();
	}

	void Update () {
		if (!mother.gameObject.activeSelf) {
			Debug.LogError("Update while dead!!!!");
		}

		if (isEnd) return;
		lr.SetPosition (0, GetStart ());
		lr.SetPosition (1, end.GetStart());
	}

	// Check the bend at the end of this segment eg. the bend with a corner in end.start
	private void CheckBend() {
		// Check if the rope went back over 180 deg.
		if (!isEnd && !end.isEnd) { // Don't check if its the end or pointing at the end.
			// Do they have a different cross and non of them are zero?
			if (CrossZ() * bendsCross < 0 - Mathf.Epsilon) {
				DestroyBend();
			}
		}
	}

	private float CrossZ() {
		return Vector3.Cross (Vector (), end.Vector ()).z;
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
		eCol.points = new Vector2[] {transform.InverseTransformPoint(GetStart()), transform.InverseTransformPoint(end.GetStart())};
	}

	// TODO TODO TODO Remember to do this each time pickup is linked
	void IgnoreCollisions ()
	{
		// Ignore the players cross colliders
		foreach (var col in mother.GetCrossColliders()) {
			Physics2D.IgnoreCollision(eCol, col);
		}
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
		// If mother is inactive we assume dead its dead and do nothing
		if (!mother.gameObject.activeSelf) {
			Debug.Log("Triggering dead segment");
			return;
		}
		var success = RopeCast (0);
		if (!success && mother.destroyOnFailedRopeCast){
			Debug.LogError("RopeCast failed and the rope is killed");
			mother.KillRope();
		}
	}

	private bool RopeCast (int recursionlvl) {
		if (recursionlvl >= 10) {
			Debug.LogError("Deep recursion detected. Conceding up.");
			return false;
		}
		// TODO Is there a way to avoid doing this twice?
		// We could save all colliders hit and then do this in FixedUpdate on a limited layer if it takes to much time otherwise.
		var pCols = mother.GetCrossColliders ();
		foreach (var col in pCols) {
			col.gameObject.layer = 2;
		}
		var hits = Physics2D.LinecastAll (GetStart(), end.GetStart(), ropeCollisionMask);
		var hits_ = Physics2D.LinecastAll (end.GetStart(), GetStart(), ropeCollisionMask);
		
		foreach (var col in pCols) {
			col.gameObject.layer = 11;
		}

		if (hits.Length != hits_.Length) {
			Debug.LogError("The ropeCast did not hit the same amount of colliders.");
			return false;
		}

		bool clean = false;
		int i = 0;
		while (!clean) {
			clean = true;
			if (i++ >= 5) {
				Debug.LogError("Failed to sanittice shit");
				return false;
			}

			if (hits.Length == 0 || hits_.Length == 0) {
				// This happens when we are checking if a new segment is ok
				// TODO Find out why this also happens on a line that triggered the trigger. Is it just a corner?
				return true;
			}
	
			if (Mathf.Approximately(hits[0].fraction, 0)) {
				if (hits[0].collider.gameObject.layer == 11) {
					Debug.Log ("Ropecast started in a player - Fixing it");
					hits = hits.Skip(1).ToArray();
					clean = false;
				} else {
					Debug.Log("RopeCast started in a platform - Killing the rope");
					mother.KillRope();
					return false;
				}
				// Debug.LogError("Illegal RopeCast, " + hits[0].point + " is inside a platform");
			}
	
			if (Mathf.Approximately(hits_[0].fraction, 0)) {
				if (hits[0].collider.gameObject.layer == 11) {
					Debug.Log ("Ropecast started in a player - Fixing it");
					hits_ = hits_.Skip(1).ToArray();
					clean = false;
				} else {
					Debug.Log("RopeCast started in a platform - Killing the rope");
					mother.KillRope();
					return false;
				}
				// Debug.LogError("Illegal RopeCast, " + hits_[0].point + " is inside a platform");
			}
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
			return false;
		}

		// Split by the first collider hit. It will check and make sure the others are ok too
		return DefineSplit(hit1, hit2, recursionlvl); // We let DefineSplit check if the segments it created are ok.
		// TODO Check that the bends are still OK Sould not be needed?!?!?
	}

	// This function takes two points on a single collider checks if they are parallel and if they are finds appropriate sides to find corners with.
	// It then splits the segment by thouse corners and checks if the splits are ok
	// hit1 and hit2 should be on the same collider!
	// hit1 and hit2 should always be in order based on their order on the rope
	private bool DefineSplit (RaycastHit2D hit1, RaycastHit2D hit2, int recursionlvl) {
		if (hit1.collider != hit2.collider) {
			Debug.LogError("DefineSplit should only be called with the same collider");
			return false;
		}

		// TODO Do something else for polygon circle etc.
		if (!(hit1.collider is BoxCollider2D)) {
			Debug.LogError("DUDE I DID ONLY SUPPORT BOX COLLIDERS WTF MAN!?!?!");
			return false;
		}
		// The angle between the normals of the surfaces hit on the collider
		var ang = Vector2.Angle(hit1.normal, hit2.normal); // TODO use dot/cross prod?
		// This wil behave odd if we use edge colliders
		if (Mathf.Approximately(ang, 180) || Mathf.Approximately(ang, 0)) {
			// The lines are parallel so we need to find the middle line
			Vector3 mid = (hit1.point + hit2.point) / 2;
			var normal = Quaternion.AngleAxis(90,new Vector3(0,0,-1)) * hit1.normal;
			var ray0 = mid + (normal*1000);
			var ray0_ = mid + (normal*-1000);

			// We place the collider on its own collider so that we can check collision only with that
			var oldLayer = hit1.collider.gameObject.layer;
			hit1.collider.gameObject.layer = 10; // TODO Change to signifie RopeCast Layer
			var hits = Physics2D.LinecastAll(ray0, mid, 1 << 10);
			var hits_ = Physics2D.LinecastAll(ray0_, mid, 1 << 10);
			hit1.collider.gameObject.layer = oldLayer;

			if (hits.Length != 1 || hits_.Length != 1) {
				Debug.LogError("Found more than one collider even though we are using a restricted layer");
				return false;
			}

			// Split the segments by the line that is closest and the second corner.
			// We need to split by hit2 first so that the next split is overlapping hit1
			// splitSegment will check if the split is ok and figure out that the other corner needs a split too.
			// We use fraction because distance doesn't work
			var hit = (hits[hits.Length-1].fraction > hits_[hits_.Length-1].fraction) ? hits[0] : hits_[0];
			var success = SplitSegment(hit, hit2);
			if (!success) return false;

			// Split on the other corner if there is still a collision. We can't just RopeCast as there might still be a parallel collision
			if (Physics2D.Linecast(GetStart(), end.GetStart()).collider != null) {
				success = SplitSegment(hit1, hit);
				if (!success) return false;
				success = end.end.RopeCast(recursionlvl + 1);
				if (!success) return false;
			}
			// Check if the new ropes are ok
			// TODO Should this be done in ropeCast?
			success = RopeCast (recursionlvl + 1);
			if (!success) return false;
			return end.RopeCast (recursionlvl + 1);
		} else {
			var success = SplitSegment (hit1, hit2);
			if (!success) return false;

			// Check if the new ropes are ok
			// TODO Should this be done in ropeCast?
			success = RopeCast (recursionlvl + 1);
			if (!success) return false;
			return end.RopeCast (recursionlvl + 1);
		}
	}

	// Finds a corner based on the hits and splits the segment by that corner.
	// Should always be followed by RopeCast() and end.RopeCast() to check if ropes are ok.
	// hit1 and hit2 should always be on the same collider!
	// hit1 and hit2 should always be in order based on their order on the rope
	private bool SplitSegment (RaycastHit2D hit1, RaycastHit2D hit2) {
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
		
		// Calculate a point outside the collider with a given offset
		var ropePoint = corner + (dir.normalized * mother.ropeOffset);

		// Check if ropePoint is inside a collider and move it out in that case
		var lc = Physics2D.Linecast (ropePoint, ropePoint, 1 << 9); // We do not check for players as that might break it. // TODO WHY THE FUCK DOES THIS MAKE A STACKOVERFLOW???
		int i = 1;
		while (lc.collider != null) {
			// TODO dont do shit and let the player die?
			if (i > 4) return false;
			Debug.LogError("RopePoint was set inside a collider: Moving it out nr of time: " + i);
			ropePoint = corner + (dir.normalized * mother.ropeOffset * i++ * 2);
			lc = Physics2D.Linecast (ropePoint, ropePoint, 1 << 9); // We do not check for players as that might break it.
		}

		end = NewSeg(mother, ropePoint, end, hit1.collider, bendsCross);
		// TODO We could do end.CheckCross() Here if its needed.
		// The new segment is now in control of the line this last hat so it inherits bendcross while this takes the new cross

		// This assumes that hit1 is before hit2 on the rope
		bendsCross = cross.z;
		UpdateECol();
		return true;
	}

	public Vector2 GetStart() {
		return col.transform.TransformPoint(startOffset);
	}

	void OnDrawGizmos() {
		Gizmos.DrawSphere (GetStart(), 0.05f);
	}
}

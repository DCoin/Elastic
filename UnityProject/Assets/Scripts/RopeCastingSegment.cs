using UnityEngine;
using System.Collections;

public class RopeCastingSegment : MonoBehaviour {

	private RopeCasting mother;
	// The point the rope goes through. Should be offset slightly compared to the corner of the collider
	public Vector2 start;
	// The next segment on the rope. Will be null if it is the last.
	public RopeCastingSegment end;
	private bool isEnd;
	// The collider near start that initiated the creation of this segment. Will be null if it is the first og last element.
	private Collider2D col;
	// Did the bend between this and end's vector go right?
	private float bendsCross;
	// TODO Should we include something to indicate where on the collider the problem is? So we can recalculate if it stil bends.
	// The collider that defines this segment.
	private EdgeCollider2D eCol;

	public static RopeCastingSegment NewSeg(RopeCasting mother, Vector2 start, RopeCastingSegment end, Collider2D col, float bendsCross) {
		var gObj = new GameObject("RopeSegment");
		gObj.transform.parent = mother.transform;

		var nxt = gObj.AddComponent<RopeCastingSegment>();
		nxt.mother = mother;
		nxt.start = start;
		nxt.end = end;
		nxt.col = col;
		nxt.bendsCross = bendsCross;
		nxt.eCol = gObj.AddComponent<EdgeCollider2D>();
		nxt.eCol.isTrigger = true;
		nxt.IgnoreCollisions ();
		nxt.isEnd = false;

		if (col == null || end.isEnd) { // in this case this is the first or second to last segment
			var rig = gObj.AddComponent<Rigidbody2D> ();
			rig.gravityScale = 0; // TODO fix this hack to make it collide ? (it will not detect collision if there is no non kinematic rigidbodies involved).
		}
		nxt.UpdateECol();
		return nxt;
	}

	public static RopeCastingSegment NewEndSeg(RopeCasting mother, Vector2 start) { // TODO make it an enum instead of this? This is not functional though :S
		var gObj = new GameObject("RopeSegmentEnd");
		gObj.transform.parent = mother.transform;

		var nxt = gObj.AddComponent<RopeCastingSegment>();
		nxt.mother = mother;
		nxt.start = start;
		nxt.bendsCross = 0; // This value should never be used on ends
		nxt.isEnd = true;
		return nxt;
	}

	void FixedUpdate () {
		// Update ends that follow players.
		if (isEnd) {
			start = mother.p2.transform.position;
		} else if (col == null) { // It is the first segment TODO add a seperate signifier for this?
			start = mother.p1.transform.position;
		}
		UpdateECol ();

		// Check if the rope went back over 180 deg.
		if (!isEnd && !end.isEnd) { // Don't check if its the end or pointing at the end.
			var cross = Vector3.Cross(Vector(),end.Vector());
			// Do they have a different cross and non of them are zero?
			if (cross.z * bendsCross < 0 - Mathf.Epsilon) {
				// I assume i dont have to touch: mother col eCol start isEnd
				bendsCross = end.bendsCross;
				var oldEnd = end;
				end = end.end;
				Object.Destroy(oldEnd.gameObject);

				// If we just deleted the segment with a rigidbody we need to add it to ourself
				if (end.isEnd && rigidbody2D == null) {
					var rig = gameObject.AddComponent<Rigidbody2D> ();
					rig.gravityScale = 0; // TODO This hack again...
				}

				UpdateECol();

				// TODO Check if any of the other ends are good?
			}
		}
	}

	void UpdateECol() {
		// TODO this could be done once for the segments that don't move
		// TODO Do we need the inverser transforms?
		if (!isEnd) 
			eCol.points = new Vector2[] {transform.InverseTransformPoint (start), transform.InverseTransformPoint (end.start)};
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
		return end.start - start;
	}

	public float Length() {
		return isEnd ? 0 : Vector2.Distance (start, end.start);
	}
	
	void OnTriggerStay2D (Collider2D hitCol) {
		if (hitCol.gameObject.layer != 9) return; // Hack to ignore all but platforms

		// TODO Is there a way to avoid doing this twice?
		// We could save all colliders hit and then do this in FixedUpdate on a limited layer if it takes to much time otherwise.
		var hits = Physics2D.LinecastAll (start, end.start, 1 << 9); // 1 << 9 means layer 9 which is platforms
		var hits_ = Physics2D.LinecastAll (end.start, start, 1 << 9); // 1 << 9 means layer 9 which is platforms

		if (hits.Length == 0) {
			Debug.LogError("No collider was found even though something triggered the trigger");
			return;
		}
		if (hits.Length == 1 || true) { // This part was only build for 1 collider but we test it for more.
			// The angle between the normals of surface of the collider
			var ang = Vector2.Angle(hits[0].normal, hits_[0].normal);
			if (Mathf.Approximately(ang, 180) || Mathf.Approximately(ang, 0)) {
				Debug.LogError("THEY ARE PARRALLELLL FUUUUUCKKKK"); // TODO Fix. I could assume that there is an end close by.
				return;
			}

			// Find which normal is the first in a clockwise rotation so that the angle is the smallest
			// and use this to calculate a vector that is exactly between the two normals.
			var cross = Vector3.Cross(hits[0].normal, hits_[0].normal);
			Vector3 dir;
			if (cross.z < 0) {
				// hits[0] is the first in the shortest clockwise angle rotation and the point is on the left side (by vector direction)
				dir = Quaternion.AngleAxis(ang/2, new Vector3(0,0,-1)) * hits[0].normal; // Turn the first normal by half the angle
			} else {
				// hits_[0] is the first in the shortest clockwise angle rotation
				dir = Quaternion.AngleAxis(ang/2, new Vector3(0,0,-1)) *  hits_[0].normal; // Turn the second normal by half the angle
			}

			// Calculate the corner of the collider by intersect the rays from the points with vectors perpendicular to the normals.
			Vector3 p1 = hits[0].point;
			Vector3 p2 = hits_[0].point;
			Vector3 v1 = Quaternion.AngleAxis(90,new Vector3(0,0,-1)) * hits[0].normal;
			Vector3 v2 = Quaternion.AngleAxis(90,new Vector3(0,0,-1)) * hits_[0].normal;
			Vector3 corner = Math2D.LineIntersectionPoint(p1, p1+v1, p2, p2+v2);

			// Calculate a point outside the collider with a given offset
			var ropePoint = corner + (dir.normalized * mother.ropeOffset);
			end = NewSeg(mother, ropePoint, end, hitCol, bendsCross);
			// The new segment is now in control of the line this last hat so it inherits bendcross while this takes the new cross
			bendsCross = cross.z;
			UpdateECol();
		} else {
			Debug.LogError("SHIIT I DONT KNOW WHAT TO DO WITH THIS AMOUNT OF CILLIDERS: " + hits.Length); // TODO Do something here
		}
	// TODO Check that the bends are still OK
	}

	void OnDrawGizmos() {
		Gizmos.DrawSphere (start, 0.05f);
	}
}

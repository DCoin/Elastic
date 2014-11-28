using UnityEngine;
using System.Collections;

public class PickupScript : MonoBehaviour {
	
	public float ropeDrag = 0.01F;	
	public float ropeMass = 0.01F;
	public float dampingratio = 0.5F;
	public float frequency = 12F;
	public float springdistance = 0.000001F;
	public int segments = 3;
	public GameObject RopePrefab;
	public bool PickupAble = true;
	private RopeScript rope;
	private Vector2 respawnPoint;

	// Use this for initialization
	void Awake () {
		// Set starting respawn point to start position
		// This can be overwritten later with the SetRespawnPoint method
		respawnPoint = transform.position;
	}
	
	/// <summary>
	/// Sets the respawn point of this squad.
	/// </summary>
	/// <param name="point">Point.</param>
	public void SetRespawnPoint(Vector2 point) {
		this.respawnPoint = point;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!GetComponent<SpringJoint2D>() && !PickupAble) {
			PickupAble = true;
		}
	}

	public void Respawn() {
		// Reset position of parenting (this) pickup
		transform.position = respawnPoint;
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.inertia = 0f;
		}

	void OnCollisionEnter2D(Collision2D col) {
		if (PickupAble && col.transform.root.name != "Platforms") {
			Transform Squad = col.gameObject.transform.root;
			rigidbody2D.isKinematic = false;
			GameObject go = Instantiate (RopePrefab, transform.position, Quaternion.identity) as GameObject;
			rope = go.GetComponent<RopeScript> ();
			rope.gameObject.name = "Rope2";
			rope.transform.parent = Squad;
			rope.objects [1] = gameObject;
			rope.ropeDrag = ropeDrag;
			rope.ropeMass = ropeMass;
			rope.dampingratio = dampingratio;
			rope.frequency = frequency;
			rope.springdistance = springdistance;
			rope.segments = segments;
			rope.isPersistent = false;
			rope.itemsThatLoops.Add(transform.gameObject);
			//rope.collidersToIgnore.Add(col.collider);
			rope.collidersToIgnore.Add(transform.collider2D);
			Transform child = null;
			foreach (Transform c in col.transform.root) {
				if (c.name == "Rope") {
					child = c;
				} else {
					foreach (Collider2D playercol in c.GetComponentsInChildren<CircleCollider2D>()) {
					rope.collidersToIgnore.Add(playercol);
					}
				}
			}
			if (child) {
				string no = (child.childCount/2).ToString();
				foreach (Transform gamobj in child.transform) {
					rope.collidersToIgnore.Add(gamobj.collider2D);
					if (gamobj.name == "Joint_"+no) {
						rope.objects [0] = gamobj.gameObject;
					}
				}
			}
			PickupAble = false;
			}
	}
}

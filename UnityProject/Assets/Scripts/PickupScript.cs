using UnityEngine;
using System.Collections;

public class PickupScript : MonoBehaviour {
	
	public float ropeDrag = 0.01F;	
	public float ropeMass = 0.01F;
	public float dampingratio = 1F;
	public float frequency = 12F;
	public float springdistance = 0.000001F;
	public int segments = 3;
	public GameObject RopePrefab;
	public bool pickupAble = true;
	public GameObject newRopePrefab;
	//public float minRopeDist = 1f;
	//public float acceleration = 0.3f;
	private RopeScript rope;
	private Vector2 respawnPoint;
	private bool newRope = false;
	private RopeCasting newRopeCast;
	private Squad squad;

	void Start () {
		if (FindObjectOfType<RopeCasting>() != null) newRope = true;
		if (newRope) {
			var ropes = GetComponentsInChildren<RopeCasting> (true);
			if (ropes.Length == 0) {
				Debug.LogError("Failed at creating new rope");
				return;
			}
			newRopeCast = ropes[0];
			newRopeCast.killActions += unlink;
		}
	}

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
		if (!newRope && !GetComponent<SpringJoint2D>() && !pickupAble) {
			pickupAble = true;
		}
	}

	public void Respawn() {
		// Reset position of parenting (this) pickup
		transform.position = respawnPoint;
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.inertia = 0f;
		if (newRope) KillRope();
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (newRope) {
			newCollision(col);
		}
		else if (pickupAble && col.transform.root.name != "Platforms") {
			Transform squad = col.gameObject.transform.root;
			rigidbody2D.isKinematic = false;
			GameObject go = Instantiate (RopePrefab, transform.position, Quaternion.identity) as GameObject;
			rope = go.GetComponent<RopeScript> ();
			rope.gameObject.name = "PickupRope";
			rope.transform.parent = squad;
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
			pickupAble = false;
		}
	}

	void newCollision (Collision2D col)
	{
		if (pickupAble) {
			var player = col.gameObject.GetComponentInParent<PlayerController> ();
			if (player != null) {
				Pickup(player);
			}
		}
	}

	public void Pickup (PlayerController player)
	{
		squad = player.GetComponentInParent<Squad> ();
		if (squad == null) {
			Debug.LogError("Squad of the player was not found");
			return;
		}

		squad.pickup = this;


		//var ropeO = Instantiate (newRopePrefab) as GameObject; // TODO We could make a copy of the players rope instead to get color etc.
		//ropeO.transform.parent = player.transform.root; // I dont like this but im going to keep using it anyway
		//ropeO.name = "PickupRope";
		//var rope = ropeO.GetComponent<RopeCasting> ();
		newRopeCast.p1 = player.roller;
		newRopeCast.p2 = gameObject;
		newRopeCast.Activate ();
		//newRopeCast.minRopeDist = minRopeDist;
		//newRopeCast.acceleration = acceleration;
		rigidbody2D.isKinematic = false; // TODO brug gravityscale = 0?

		pickupAble = false;
	}

	public void KillRope () {
		// This call will hopefully trigger the unlink function trough call backs
		newRopeCast.KillRope ();
	}

	private void unlink () {
		squad.pickup = null;
		squad = null;
		pickupAble = true;
	}
}

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
	public float minRopeDist = 1f;
	public float acceleration = 0.3f;
	private RopeScript rope;
	private Vector2 respawnPoint;
	private bool newRope = false;

	void Start () {
		if (FindObjectOfType<RopeCasting>() != null) newRope = true;
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
		}

	void OnCollisionEnter2D(Collision2D col) {
		if (newRope) {
			newCollision(col);
		}
		else if (pickupAble && col.transform.root.name != "Platforms") {
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
		var squad = player.GetComponentInParent<Squad> ();
		if (squad == null) {
			Debug.LogError("Squad of the rope was not found");
			return;
		}

		var ropeO = Instantiate (newRopePrefab) as GameObject; // TODO We could make a cope of the players rope instead to get color etc.
		var rope = ropeO.GetComponent<RopeCasting> ();
		rope.p1 = player.roller;
		rope.p2 = gameObject;
		rope.minRopeDist = minRopeDist;
		rope.acceleration = acceleration;
		rigidbody2D.isKinematic = false; // TODO brug gravityscale = 0?

		pickupAble = false;
	}
}

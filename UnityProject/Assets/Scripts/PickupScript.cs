using UnityEngine;
using System.Collections;

public class PickupScript : MonoBehaviour {
	
	public float ropeDrag = 0.01F;	
	public float ropeMass = 0.01F;
	public float dampingratio = 0.5F;
	public float frequency = 4F;
	public float springdistance = 0.000001F;
	public int segments = 5;
	public GameObject RopePrefab;
	public bool PickupAble = true;
	private RopeScript rope;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!GetComponent<SpringJoint2D>() && !PickupAble) {
			PickupAble = true;
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (PickupAble && col.transform.root.name != "Level") {
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
			rope.itemsThatLoops.Add(transform.gameObject);
			rope.collidersToIgnore.Add(col.collider);
			Transform child = null;
			foreach (Transform c in col.transform.parent) {
				if (c.name == "Rope") {
					child = c;
				} else {
					rope.collidersToIgnore.Add(c.collider2D);
				}
			}
			if (child) {
				string no = (child.childCount/2).ToString();
				foreach (Transform gamobj in child.transform) {
					Debug.Log (gamobj.name);
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

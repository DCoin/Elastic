using UnityEngine;
using System.Collections;

public class LevelDivider : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D col) {
		// New rope
		var squad = col.gameObject.GetComponentInParent<Squad> ();
		var pickup = col.gameObject.GetComponentInParent<PickupScript> ();
		if (squad != null && squad.pickup != null || pickup != null && !pickup.pickupAble) {
			transform.collider2D.isTrigger = true;
		}
		// Old rope
		if (col.transform.root.Find ("PickupRope")) {
			transform.collider2D.isTrigger = true;
		}
	}

	void OnTriggerExit2D(Collider2D col) 
	{
		if (collider2D.isTrigger == false) return;
		var p = Physics2D.OverlapArea (collider2D.bounds.min, collider2D.bounds.max, (1<<8) | (1<<12) | (1<<13));
		if (p == null) {
			transform.collider2D.isTrigger = false;
		}
	}
}

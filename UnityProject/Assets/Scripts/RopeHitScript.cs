using UnityEngine;
using System.Collections;


public class RopeHitScript : MonoBehaviour {

	// What function(s) should be called when the rope is hit?
	public delegate void FuncsToCall();
	public FuncsToCall funcsToCall;

	void OnCollisionEnter2D(Collision2D col) {
		// Check if the colliding entity has a player script, and is currently heavy
		var pc = col.gameObject.GetComponent<PlayerController>() as PlayerController;
		if (pc != null) {
			if (pc.IsHeavy) {
				funcsToCall();
			}
		
		foreach (var item in transform.root.transform) {
			var rs = transform.root.GetComponent<RopeScript>();
			if (rs != null) {
				rs.ResetPosition();
			}
		}
		}
	}
}

using UnityEngine;
using System.Collections;

public class Squad : MonoBehaviour {
	// TODO the current implementation of the squad kill/respawn is only one way of doing it.
	// we should investigate other ways of doing it. maybe allowing independant movement when severed,
	// and then players attempt for reattachment?

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
	
	// TODO proper method comment
	public void Respawn() {
		// Reactivate all gameobjects
		foreach (var child in transform) {
			var c = child as Transform;
			c.gameObject.SetActive(true); // Dunno if this is right?
		}

		// Reset position of parenting (this) squad
		transform.position = respawnPoint;

		// TODO assign proper methods in the player objects for handling this responsibility
		// Reset velocity and relative position of players
		foreach (var pc in transform.GetComponentsInChildren<PlayerController>()) {
			pc.transform.localPosition = Vector2.zero;
			pc.rigidbody2D.velocity = Vector2.zero;
			pc.rigidbody2D.angularVelocity = .0f;
		}
		// rebuild all ropes
		foreach (var rs in transform.GetComponentsInChildren<RopeScript>()) {
			rs.BuildRope();
			rs.ResetPosition();
		}

	}

	// TODO Proper method comment
	public void Kill(float respawnTime) {
		Invoke("Respawn", respawnTime);
		Kill ();
	}

	// TODO proper method comment
	public void Kill() {
		// break all ropes
		foreach (var script in transform.GetComponentsInChildren<RopeScript>()) {
			script.DestroyRope();
		}

		// Deactivate all gameobjects
		foreach (var child in transform) {
			var c = child as Transform;
			c.gameObject.SetActive(false); // Dunno if this is right?
		}
		// Anything else needs doing to disable?
		// Maybe call to check for rope-severing?
	}
}

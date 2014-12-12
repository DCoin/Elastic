using UnityEngine;
using System.Collections;

public class Squad : MonoBehaviour {
	// TODO the current implementation of the squad kill/respawn is only one way of doing it.
	// we should investigate other ways of doing it. maybe allowing independant movement when severed,
	// and then players attempt for reattachment?

	public PickupScript pickup;

	private AudioSource audioSource;

	// Exposing these, other classes use 'em :)
	public Vector2 RespawnPoint {get; set;}
	public float CurrentRepsawnTime {get; private set;}
	public bool Alive {get; private set;}

	// Use this for initialization
	void Awake () {
		// Set starting respawn point to start position
		// This can be overwritten later with the SetRespawnPoint method
		RespawnPoint = transform.position;

		Alive = true;
	}


	/// <summary>
	/// Respawn this Squad at the previously assigned respawn point.
	/// </summary>
	public void Respawn() {
		// Prevent double respawns
		CancelInvoke("Respawn");
		CancelInvoke("PlayRespawnSound");

		// Reactivate all gameobjects
		foreach (var child in transform) {
			var c = child as Transform;
			c.gameObject.SetActive(true); // Dunno if this is right?
		}

		// Reset position of parenting (this) squad
		transform.position = RespawnPoint;

		// TODO assign proper methods in the player objects for handling this responsibility
		// Reset velocity and relative position of players
		foreach (var pc in transform.GetComponentsInChildren<PlayerController>()) {
			pc.transform.localPosition = Vector2.zero;
			pc.rigidbody2D.velocity = Vector2.zero;
			pc.rigidbody2D.angularVelocity = .0f; // TODO This should be on the roller?
		}
		// rebuild all ropes
		foreach (var rs in transform.GetComponentsInChildren<RopeScript>()) {
			rs.DestroyRope();
			if (rs.isPersistent) {
				rs.BuildRope();
			}
			rs.ResetPosition();
		}

		// rebuild all new ropes
		foreach (var rs in transform.GetComponentsInChildren<RopeCasting>()) {
			rs.ResetPosition();
		}

		CurrentRepsawnTime = 0.0f;
		Alive = true;
	}

	private void PlayRespawnSound() {
		SoundManager.PlaySound(SoundManager.SoundTypes.Squad_Respawn);
	}


	/// <summary>
	/// Kill the squad, and respawns them after a specified amount of time.
	/// </summary>
	/// <param name="respawnTime">Respawn time.</param>
	public void Kill(float respawnTime) {
		CurrentRepsawnTime = respawnTime;

		// play the respawn sound right before respawning
		Invoke("PlayRespawnSound", respawnTime-1.5f); // TODO This breaks if respawnTime is 1.5f or lower?

		Invoke("Respawn", respawnTime);
		Kill ();
	}

	/// <summary>
	/// Kill the squad.
	/// </summary>
	public void Kill() {
		if (pickup != null) pickup.KillRope ();
		
		// Play the kill sound
		SoundManager.PlaySound(SoundManager.SoundTypes.Squad_Kill);

		// break all ropes
		foreach (var script in transform.GetComponentsInChildren<RopeScript>()) {
			script.DestroyRope();
		}

		// Deactivate all gameobjects
		foreach (var child in transform) {
			var c = child as Transform;
			c.gameObject.SetActive(false); // Dunno if this is right?
		}

		Alive = false;
		// Anything else needs doing to disable?
		// Maybe call to check for rope-severing?
	}
}

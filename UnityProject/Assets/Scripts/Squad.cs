using UnityEngine;
using System.Collections;

public class Squad : MonoBehaviour {
	// TODO the current implementation of the squad kill/respawn is only one way of doing it.
	// we should investigate other ways of doing it. maybe allowing independant movement when severed,
	// and then players attempt for reattachment?

	private Vector2 respawnPoint;
	
	public AudioClip killSound;
	public float killSoundVolume = 0.6f;
	public AudioClip respawnSound;
	public float respawnSoundVolume = 0.6f;

	public PickupScript pickup;

	private AudioSource audioSource;

	// Use this for initialization
	void Awake () {
		// Set starting respawn point to start position
		// This can be overwritten later with the SetRespawnPoint method
		respawnPoint = transform.position;

		audioSource = gameObject.AddComponent<AudioSource>();

	}

	/// <summary>
	/// Sets the respawn point of this squad.
	/// </summary>
	/// <param name="point">Point.</param>
	public void SetRespawnPoint(Vector2 point) {
		this.respawnPoint = point;
	}

	/// <summary>
	/// Respawn this Squad at the previously assigned respawn point.
	/// </summary>
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

	}

	private void PlayRespawnSound() {
		audioSource.clip = respawnSound;
		audioSource.Play();
	}


	/// <summary>
	/// Kill the squad, and respawns them after a specified amount of time.
	/// </summary>
	/// <param name="respawnTime">Respawn time.</param>
	public void Kill(float respawnTime) {
		Invoke("PlayRespawnSound", respawnTime-1.5f); // TODO This breaks if respawnTime is 1.5f or lower?
		Invoke("Respawn", respawnTime);
		Kill ();
	}

	/// <summary>
	/// Kill the squad.
	/// </summary>
	public void Kill() {
		if (pickup != null) pickup.KillRope ();
		
		audioSource.clip = killSound;
		audioSource.Play();
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

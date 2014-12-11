using UnityEngine;
using System.Collections;

public class LethalPlatform : MonoBehaviour {
	public float respawnTime = 7;

	private DuelMode duelMode;

	void Start() {
		duelMode = GameObject.FindObjectOfType(typeof(DuelMode)) as DuelMode;
	}

	void OnCollisionStay2D (Collision2D col) {
		var squad = col.collider.GetComponentInParent<Squad> ();
		if (squad != null) squad.Kill(duelMode != null ? duelMode.killRespawnTime : respawnTime);

		var pickup = col.collider.GetComponent<PickupScript> ();
		if (pickup != null) pickup.Respawn();
	}
}

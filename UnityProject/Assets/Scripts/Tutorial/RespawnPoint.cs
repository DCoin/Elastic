using UnityEngine;
using System.Collections;

public class RespawnPoint : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D col) {
		// If we collide with a player, set its respawnpoint to this
		if (col.gameObject.GetComponent<PlayerController>()) {
			var squad = col.gameObject.GetComponentInParent<Squad>();
			if (squad == null) {
				Debug.LogError("This player is not part of a squad!");
				gameObject.SetActive(false);
				return;
			}

			squad.RespawnPoint = gameObject.transform.position;
		}
	}
}

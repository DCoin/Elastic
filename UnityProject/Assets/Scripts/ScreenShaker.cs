using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScreenShaker : MonoBehaviour {
	// Assuming number of players does not change during a game

	public float shakeMultiplier = 0.1f;

	private struct PlayerState {
		public PlayerController player;
		public Vector2 velocity;
		public bool onGround;
		public bool isHeavy;

		public PlayerState(PlayerController p) {
			this.player = p;
			this.velocity = p.rigidbody2D.velocity;
			this.onGround = p.onGround;
			this.isHeavy = p.IsHeavy;
		}
	}

	private List<PlayerState> players;

	// Find all players and start to track them.
	void Start () {
		players = new List<PlayerState>();

		var pcs = GameObject.FindObjectsOfType<PlayerController>();
		foreach (var p in pcs) {
			players.Add(new PlayerState(p));
		}
	}
	
	// Update is called once per frame
	void Update () {
		// get new set of player values
		var playersNew = players.Select(p => UpdatePlayerValues(p)).ToList();

		// compare states between last frame and now
		for (int i = 0; i < players.Count; i++) {
			var oldp = players[i];
			var newp = playersNew[i];

			// Check if someone just hit the ground!
			if (!oldp.onGround && newp.onGround) {
				// Check if that player was heavy
				// TODO allow speed threshold to enable screen shaking even if not heavy
				if (oldp.isHeavy) {
					BumpScreen(-oldp.velocity);
				}
			}
		}

		// assign updated list in preperation to next frame
		players = playersNew;
	}


	private void BumpScreen(Vector2 velocity)
	{
		Camera.main.transform.position += (Vector3) velocity * shakeMultiplier;
	}

	// method for screen shake
	// method for smoke trails at landing

	private PlayerState UpdatePlayerValues(PlayerState p) {
		var pc = p.player;
		return new PlayerState {
			player 	 = pc,
			velocity = pc.rigidbody2D.velocity,
			onGround = pc.onGround,
			isHeavy  = pc.IsHeavy
		};
	}
}

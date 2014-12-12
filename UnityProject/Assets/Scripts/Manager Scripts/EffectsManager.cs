using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EffectsManager : MonoBehaviour {
	// Assuming number of players does not change during a game

	public bool screenShake = true;
	public float shakeMultiplier = 0.1f;

	public bool ghosts = true;
	public Sprite ghostSprite;

	public bool slowmoKill = true;
	public float slowdownTo = 0.33f;
	public float slowmoTime = 0.5f;
	private bool slowmoActive = false;
	private float standardTimeScale;
	private float slowmoClock;

	private struct PlayerState {
		public PlayerController player;
		public Vector2 velocity;
		public Vector2 position;
		public bool onGround;
		public bool isHeavy;
		public int lastCollisionLayer;
		public string lastCollisionTag;
		public bool alive;

		public PlayerState(PlayerController p) {
			this.player = p;
			this.velocity = p.rigidbody2D.velocity;
			this.position = p.transform.position;
			this.onGround = p.onGround;
			this.isHeavy = p.IsHeavy;
			this.lastCollisionLayer = p.LastCollisionLayer;
			this.lastCollisionTag = p.LastCollisionTag;
			this.alive = p.gameObject.GetComponentInParent<Squad>().Alive;
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

		// For slowmo part
		standardTimeScale = Time.timeScale;
	}

	void Update () {
		// get new set of player values
		players.RemoveAll(p => p.player == null);
		var playersNew = players.Select(p => UpdatePlayerValues(p)).ToList();

		// compare states between last frame and now
		for (int i = 0; i < players.Count; i++) {
			var oldp = players[i];
			var newp = playersNew[i];

			// SCREEN SHAKING //
			if (screenShake) {
				// Check if someone just hit the ground!
				if (!oldp.onGround && newp.onGround) {
					// Check if that player was heavy
					// Don't shake if landing on a trampoline
					// TODO allow speed threshold to enable screen shaking even if not heavy
					if (oldp.isHeavy) {
						if (newp.lastCollisionTag == "Trampoline")
							BumpScreen(-oldp.velocity, shakeMultiplier * 0.5f);
						else
							BumpScreen(-oldp.velocity, shakeMultiplier);
					}
				}
			}

			// KILL-GHOSTS //
			if (ghosts) {
				if (oldp.alive && !newp.alive) {
					// Someone just died, spawn a ghost!
					// TODO get these values in a proper manner
					RespawnGhost.CreateGhost(
						oldp.position, 
						newp.player.gameObject.GetComponentInParent<Squad>().RespawnPoint,
						newp.player.gameObject.GetComponentInParent<Squad>().CurrentRepsawnTime - 1.5f,
						1.5f,
						ghostSprite);
				}
			}

			// KILL-SLOWMO
			if (slowmoKill) {
				if (!slowmoActive && oldp.alive && !newp.alive) {
					// Someone just died, let's slooowmo!
					// TODO get these values in a proper manner
					Time.timeScale = slowdownTo;
					slowmoClock = 0.0f;
					slowmoActive = true;
				}
			}

			if (slowmoActive && Time.timeScale > 0.0f) { // Check if the game isn't paused before you fiddle with it
				slowmoClock += Time.deltaTime;

				Time.timeScale = Mathf.SmoothStep(slowdownTo, standardTimeScale, slowmoClock / slowmoTime);

				if (Time.timeScale == standardTimeScale) {
					slowmoActive = false;
				}
			}

		}

		// assign updated list in preperation to next frame
		players = playersNew;
	}


	private void BumpScreen(Vector2 velocity, float multiplier)
	{
		Camera.main.transform.position += (Vector3) velocity * multiplier;
	}

	// method for screen shake
	// method for smoke trails at landing

	private PlayerState UpdatePlayerValues(PlayerState p) {
		var pc = p.player;
		return new PlayerState {
			player 	 = pc,
			velocity = pc.rigidbody2D.velocity,
			position = pc.transform.position,
			onGround = pc.onGround,
			isHeavy  = pc.IsHeavy,
			lastCollisionLayer 	= pc.LastCollisionLayer,
			lastCollisionTag 	= pc.LastCollisionTag,
			alive	 = pc.gameObject.GetComponentInParent<Squad>().Alive
		};
	}
}

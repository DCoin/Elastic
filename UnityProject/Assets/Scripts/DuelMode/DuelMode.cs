using UnityEngine;
using System.Collections;

public class DuelMode : MonoBehaviour {
	// TODO implement winning condition check thingy

	// At the moment, DuelMode takes a direct reference to this cam-script
	// maybe we can decouple this somehow? (maybe an interface?)
	public MoveCameraScript moveCam;
	public float camMoveTime = .4f;

	public DuelModeArea[] areas;
	public int startArea;

	public GameObject squad1;
	public GameObject squad2;
	public GameObject pickup;

	public float RespawnTime = 2.0F;

	private Squad sq1;
	private Squad sq2;
	private PickupScript pu;

	private int currentArea;

	// Use this for initialization
	void Start () {

		/* Error checking */
		if (areas.Length == 0) {
			Debug.LogError("No areas assigned.");
			enabled = false;
			return;
		}
		if (startArea < 0 || startArea > areas.Length-1) {
			Debug.LogError("Specified starting area number does not exist.");
			enabled = false;
			return;
		}
		if (!squad1 || !squad2) {
			Debug.LogError("Squads not assigned.");
			enabled = false;
			return;
		}
		if (!moveCam) {
			Debug.LogError("No camera script assigned.");
			enabled = false;
			return;
		}
		// TODO Add check for whether pickup is assigned as well?

		/* Initialization */

		// Get refs to squad scripts
		sq1 = squad1.GetComponent<Squad>() as Squad;
		sq2 = squad2.GetComponent<Squad>() as Squad;
		pu = pickup.GetComponent<PickupScript>() as PickupScript;

		// Set the respawn time on the players
		foreach (PlayerController p in sq1.GetComponentsInChildren<PlayerController>()) {
			p.RespawnTime = RespawnTime;
				}
		
		foreach (PlayerController p in sq2.GetComponentsInChildren<PlayerController>()) {
						p.RespawnTime = RespawnTime;
				}


		// Set up state according to starting area
		currentArea = startArea;

		// Camera
		var size = MoveCameraScript.CalculateOrthographicSize(areas[currentArea].camSize);
		moveCam.MoveTo(
			areas[currentArea].transform.position,
			size,
			camMoveTime);

		// Players
		sq1.SetRespawnPoint(areas[currentArea].GetSpawn1InGlobal());
		sq2.SetRespawnPoint(areas[currentArea].GetSpawn2InGlobal());

		// Pickup
		pu.SetRespawnPoint(areas[currentArea].GetPickupInGlobal());
	}
	

	// TODO find out if Fixed update is best here?
	// Check if any squad is out of bounds or eligible to proceed to next area
	void FixedUpdate() {
		Rect rect = areas[currentArea].GetAreaInGlobal();

		// Squad 1
		foreach (var pc in squad1.GetComponentsInChildren<PlayerController>()) {
			var player = pc as PlayerController;

			Vector2 pos = player.transform.position;
			if (pos.x < rect.xMin ||
			    pos.y < rect.yMin ||
			    pos.y > rect.yMax) {
				sq1.Respawn();
				break; // stop checking if any one is OOB
			}
			if (pos.x > rect.xMax) {
				ProceedNextArea(false);
				return; // One squad proceeds, no need to check more
			}
		}

		// Squad 2
		foreach (var pc in squad2.GetComponentsInChildren<PlayerController>()) {
			var player = pc as PlayerController;
			
			Vector2 pos = player.transform.position;
			if (pos.x > rect.xMax ||
			    pos.y < rect.yMin ||
			    pos.y > rect.yMax) {
				sq2.Respawn();
				break; // stop checking if any one is OOB
			}
			if (pos.x < rect.xMin) {
				ProceedNextArea(true);
				return; // One squad proceeds, no need to check more
			}
		}

		// Pickup
		if (pu.PickupAble) {
			Vector2 pos = pu.transform.position;
			if (pos.x > rect.xMax ||
				pos.y < rect.yMin ||
				pos.y > rect.yMax || 
			    pos.x < rect.xMin ) {
					pu.Respawn ();
				}
		}
	}

	// This following method is written in a state like manner (but dunno if best approach)

	/// <summary>
	/// Proceeds the game to the next area, depending on advancing squad.
	/// </summary>
	/// <param name="left">If set to <c>true</c> proceed left (Squad 2 wins), if not, proceed right (Squad 1 wins).</param>
	private void ProceedNextArea(bool left) {
		// If no more areas, don't do anything. We prolly wanna change this later
		if (left ? currentArea == 0 : currentArea == areas.Length-1)
			return;

		// set next area
		currentArea += left ? -1 : 1;

		// Set new spawn locations
		sq1.SetRespawnPoint(areas[currentArea].GetSpawn1InGlobal());
		sq2.SetRespawnPoint(areas[currentArea].GetSpawn2InGlobal());
		pu.SetRespawnPoint(areas[currentArea].GetPickupInGlobal());

		// Reset losing squad
		if (left) 	sq1.Respawn();
		else 		sq2.Respawn();

		// Move Camera
		var size = MoveCameraScript.CalculateOrthographicSize(areas[currentArea].camSize);
		moveCam.MoveTo(
			areas[currentArea].transform.position,
			size,
			camMoveTime);
	}
}

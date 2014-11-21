using UnityEngine;
using System.Collections;

public class DuelMode : MonoBehaviour {
	
	public GameObject[] players;
	public GameObject[] parties;
	public int levelWidth = 300;
	public int MovePartyAtLevelChangeDistance = 50;
	public int MovePartyYHeight = -116;
	public float RespawnTime = 2.0F;
	private int level;
	
	
	[HideInInspector]
	public int levelstart;
	[HideInInspector]
	public int levelend;
	
	
	// Use this for initialization
	void Start () {
		level = 0;
		levelstart = -levelWidth/2;
		levelend = levelWidth/2;
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// TODO Oops, hardcoded players here, need more generic solution
		if (Mathf.Max (players [0].transform.position.x, players [1].transform.position.x) > (level * levelWidth) + (levelWidth / 2)) {
			levelstart = levelWidth*(level+1)-levelWidth/2;
			levelend = levelstart+levelWidth;
			MoveSpheres (levelWidth*(level+1) - MovePartyAtLevelChangeDistance, 1);
		} else if (Mathf.Min (players [2].transform.position.x, players [3].transform.position.x) < (level * levelWidth) - (levelWidth / 2)) {
			levelend = levelWidth*(level-1)+levelWidth/2;
			levelstart = levelend-levelWidth;
			MoveSpheres (levelWidth*(level-1) + MovePartyAtLevelChangeDistance, -1);
		}
	}

	void Update () {
		// Go to menu on escape
		if (Input.GetKey(KeyCode.Escape)) Application.LoadLevel(0);
	}
	
	private void MoveSpheres(int pos, int levelchange) {
		
		GameObject MovedParty;
		GameObject MovedPlayer1;
		GameObject MovedPlayer2;
		var CameraScript = GetComponent<MoveCameraScript>(); 
		if (levelchange == 1) { 
			MovedParty = parties[1];
			MovedPlayer1 = players[2];
			MovedPlayer2 = players[3];
			Vector3 tmp = Camera.main.transform.position;
			tmp.x += levelWidth;
			CameraScript.targetPosition = tmp;
		} else {
			MovedParty = parties[0];
			MovedPlayer1 = players[0];
			MovedPlayer2 = players[1];
			Vector3 tmp = Camera.main.transform.position;
			tmp.x -= levelWidth;
			CameraScript.targetPosition = tmp;
		}
		CameraScript.moveCamera = true;
		
		
		MovedPlayer1.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		MovedPlayer1.GetComponent<Rigidbody2D> ().angularVelocity = 0F;
		MovedPlayer2.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		MovedPlayer2.GetComponent<Rigidbody2D> ().angularVelocity = 0F;
		MovedParty.transform.position = new Vector3 (pos, MovePartyYHeight, 0);
		
		MovedPlayer1.transform.localPosition = Vector3.zero;
		MovedPlayer2.transform.localPosition = Vector3.zero;

		foreach (var item in MovedParty.transform) {
			var rs = MovedParty.GetComponent<RopeScript>();
			if (rs != null) {
				rs.ResetPosition();
			}
		}

		level += levelchange;
	}
}

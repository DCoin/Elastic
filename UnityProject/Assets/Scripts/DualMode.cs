using UnityEngine;
using System.Collections;

public class DualMode : MonoBehaviour {
	
	public GameObject[] Players;
	public GameObject[] Parties;
	public int levelWidth = 300;
	public int MovePartyAtLevelChangeDistance = 50;
	public int MovePartyYHeight = -116;

	private int level;

	// Use this for initialization
	void Start () {
		level = 0;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Mathf.Max (Players [0].transform.position.x, Players [1].transform.position.x) > (level * levelWidth) + (levelWidth / 2)) {
			MoveSpheres (levelWidth*(level+1) - MovePartyAtLevelChangeDistance, 1);
		} else if (Mathf.Min (Players [2].transform.position.x, Players [3].transform.position.x) < (level * levelWidth) - (levelWidth / 2)) {
			MoveSpheres (levelWidth*(level-1) + MovePartyAtLevelChangeDistance, -1);
		}
	}

	private void MoveSpheres(int pos, int levelchange) {

		GameObject MovedParty;
		GameObject MovedPlayer1;
		GameObject MovedPlayer2;
		
		if (levelchange == 1) { 
			MovedParty = Parties[1];
			MovedPlayer1 = Players[2];
			MovedPlayer2 = Players[3];
		} else {
			MovedParty = Parties[0];
			MovedPlayer1 = Players[0];
			MovedPlayer2 = Players[1];
		}
		
		
		MovedPlayer1.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		MovedPlayer1.GetComponent<Rigidbody2D> ().angularVelocity = 0F;
		MovedPlayer2.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		MovedPlayer2.GetComponent<Rigidbody2D> ().angularVelocity = 0F;
		MovedParty.transform.position = new Vector3 (pos, MovePartyYHeight, 0);
		
		MovedPlayer1.transform.localPosition = Vector3.zero;
		MovedPlayer2.transform.localPosition = Vector3.zero;

		level += levelchange;
	}
}

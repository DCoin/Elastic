using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class PlaygroundMode : MonoBehaviour {
	
	public Transform SpawnPointStart;
	public Transform SpawnPointEnd;

	public float killRespawnTime;
	
	public GameObject SquadPrefab;
	public GameObject newRopePrefab;
	public GameObject playerPrefab;

	// Use this for initialization
	void OnLevelWasLoaded (int level) {


		//Order the teams list by Color
		GameObject TP = GameObject.Find ("TeamPicker");
		if (TP) {
			//Calculate how far each teams spawn and resetpoints should be apart
			float minpos = SpawnPointEnd.position.x < SpawnPointStart.position.x ? SpawnPointEnd.position.x : SpawnPointStart.position.x;
			float maxpos = SpawnPointEnd.position.x > SpawnPointStart.position.x ? SpawnPointEnd.position.x : SpawnPointStart.position.x;
			float dist = (maxpos-minpos) / TP.GetComponent<TeamPicker>().teams.Count-1;

			//Sort the list
			var sortedList = TP.GetComponent<TeamPicker>().teams.OrderBy(go=>go.GetComponentInChildren<TeamSelectionController>().currentTeam.teamColor.ToString()).ToList();
			string prevteamidentifier = "";
			GameObject prevSquad = null;
			GameObject prevPlayer = null;
			List<GameObject> players = new List<GameObject>();

			int teamcount = 0;

			foreach (GameObject go in sortedList) {
				print ("Player should be here: " + go.transform.name);
				//Create a new player
				GameObject newPlayer = Instantiate(playerPrefab) as GameObject;
				PlayerController pc = newPlayer.GetComponent<PlayerController>();
				TeamSelectionController tsc = go.GetComponentInChildren<TeamSelectionController>();
				pc.controller = tsc.controller;
				pc.leftSide = tsc.leftSide;
				pc.GetComponent<EyeAnimator>().hat = go.GetComponent<HatSelectionController>().hat;
				newPlayer.name = go.transform.name.Substring(0,go.transform.name.Length-3);

				if (prevteamidentifier != go.GetComponentInChildren<TeamSelectionController>().currentTeam.teamSprite.ToString()) {
					//Create new squad 
					print ("Creating new squad");
					prevSquad = Instantiate(SquadPrefab) as GameObject;
					prevSquad.transform.position = new Vector3(minpos+dist*teamcount, SpawnPointEnd.position.y, SpawnPointEnd.position.z);

					prevSquad.GetComponent<Squad>().SetRespawnPoint(prevSquad.transform.position);
					teamcount++;
					prevSquad.transform.name = "Squad"+teamcount;
				} else {
					//Create a rope between the player and the previous player unless other team!
					GameObject rope = Instantiate(newRopePrefab) as GameObject;
					RopeCasting rc = rope.GetComponent<RopeCasting>();

					rc.p1 = prevPlayer.GetComponent<PlayerController>().roller;
					rc.p2 = newPlayer.GetComponent<PlayerController>().roller;
					rc.ropeMaterial = go.GetComponentInChildren<TeamSelectionController>().currentTeam.teamColor;
					rope.transform.parent = prevSquad.transform;
				}
				print ("Assigning Player" +go.transform.name.Substring(go.transform.name.Length-4, 1) + " to " + prevSquad.name);
				newPlayer.transform.parent = prevSquad.transform;
				newPlayer.transform.localPosition = Vector3.zero;
				players.Add(newPlayer);
				
				prevPlayer = newPlayer;
				prevteamidentifier = go.GetComponentInChildren<TeamSelectionController>().currentTeam.teamSprite.ToString();
				Destroy (go);

			}
			GameObject.Find ("Main Camera").GetComponent<FollowCam>().objects = players.ToArray();
			}

		foreach (Squad sq in GameObject.FindObjectsOfType<Squad>()) {
			var squad = sq;
			foreach (RopeCasting rope in squad.GetComponentsInChildren<RopeCasting> ()) {
				if (rope != null) {
					rope.killActions += () => squad.Kill(killRespawnTime);
				}
			}
		}

		Destroy (TP);
		
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

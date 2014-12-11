using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class TeamPicker : MonoBehaviour {

	public int count = 0;
	public List<Team> teamVisual;

	//Used to store the teams
	public List<GameObject> teams;
	
	public GameObject teamSpriteSingle;
	public GameObject teamSpritePair; 

	public GameObject RopePrefab;


	public int eyeCount;
	public int nextlevel;

	private static bool isRunning = false;

	private int teamCount;

	[System.Serializable] // This makes it show up in the inspector, somehow
	public class Team
	{
		public Material teamColor;
		public Sprite teamSprite;
	}

	void OnLevelWasLoaded(int level) {
		if (isRunning)
						return;

		isRunning = true;
		//This object should only be loaded from the hatpicker level
		if (level == 3) {
						GameObject HP = GameObject.Find ("HatPicker");
						if (HP) {
								eyeCount = HP.GetComponent<HatPicker> ().eyeCount;
								nextlevel = HP.GetComponent<HatPicker> ().nextlevel;
								Destroy (HP);
						}


						bool pair;
						print ("nextleveL: " + nextlevel + "eyecount " + eyeCount);
						if (nextlevel == 4 || nextlevel == 5 || nextlevel == 7) { // pair levels (selectduellevel or tutorial
								teamCount = eyeCount / 2;
								pair = true;
						} else { //every player select team levels
								teamCount = eyeCount;
								pair = false;
						}
						int controller_index = 0;
						bool leftside = true;
						for (int i = 1; i <= eyeCount; i++) {
								if (pair) {
										if (i % 2 == 1) {
												print ("In i : " + i);
												GameObject go = Instantiate (teamSpritePair) as GameObject;
												go.GetComponent<TeamSelectionController> ().controller = controller_index;
												go.GetComponent<TeamSelectionController> ().currentTeam = teamVisual [i - 1];
					
												GameObject parent = GameObject.Find ("Player" + i + "Hat");
												if (parent) {
														print ("Creating rope and stuff");
														go.transform.parent = parent.transform;
														GameObject rope = Instantiate (RopePrefab) as GameObject;
														rope.transform.parent = parent.transform;
														rope.transform.localPosition = Vector3.zero;
														RopeScript ropescript = rope.GetComponent<RopeScript> ();
														ropescript.objects [0] = parent;
														int next = i + 1;
														ropescript.objects [1] = GameObject.Find ("Player" + next + "Hat");
														ropescript.segments = 4;
														ropescript.frequency = 4.0f;
														rope.GetComponent<LineRenderer> ().material = teamVisual [i - 1].teamColor;

														teams.Add (parent);
														//Disable the controller of hats and eye picking
												}

												go.transform.localPosition = new Vector3 (0.55f, 0.4f, 0.0f);
												print ("GO");
												controller_index++;
										}
								} else {
										GameObject go = Instantiate (teamSpriteSingle) as GameObject;
										go.GetComponent<TeamSelectionController> ().controller = controller_index;
										go.GetComponent<TeamSelectionController> ().leftSide = leftside;
										go.GetComponent<TeamSelectionController> ().currentTeam = teamVisual [i - 1];
										go.GetComponent<TeamSelectionController> ().single = true;

										GameObject parent = GameObject.Find ("Player" + i + "Hat");
										if (parent) {
												go.transform.parent = parent.transform;
												//Disable the controller of hats and eye picking
												teams.Add (parent);
										}
										go.transform.localPosition = new Vector3 (0.0f, 0.3f, 0.0f);
										print ("GOGOOGOG");
				
										leftside = !leftside;
										if (i % 2 == 0) {
												controller_index++;
										}
								}

								//Remove visual stuff and disable the hatselection controlling
								GameObject playerHat = GameObject.Find ("Player" + i + "Hat");
								playerHat.GetComponent<HatSelectionController> ().enabled = false;
								foreach (TextMesh t in playerHat.GetComponentsInChildren<TextMesh>()) {
										Destroy (t);
								}
								foreach (MeshRenderer t in playerHat.GetComponentsInChildren<MeshRenderer>()) {
										Destroy (t);
								}
						}

						MoveObjectSmooth mover = GameObject.Find ("Main Camera").AddComponent<MoveObjectSmooth> ();
						mover.deltaTarget = new Vector3 (0.0f, eyeCount < 5 ? 1.2f : 0.6f, 0.0f);

						//If the next level is the playground we need the TeamPicker object to sort out the squads
						if (nextlevel == 6) {
								DontDestroyOnLoad (gameObject);
						}
				}

	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (count >= teamCount) {
			Destroy (GameObject.Find ("Main Camera"));
			Application.LoadLevel (nextlevel); // the level
		}
	}
}

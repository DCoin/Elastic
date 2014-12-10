using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class TeamPicker : MonoBehaviour {

	public int count = 0;
	public List<Sprite> teamArray;
	public List<Sprite> teamSprites;
	public List<GameObject> teams;
	
	public GameObject teamSpriteSingle;
	public GameObject teamSpritePair; 


	public int eyeCount;
	public int nextlevel;

	private int teamCount;

	void OnLevelWasLoaded(int level) {
		GameObject HP = GameObject.Find ("HatPicker");
		if (HP) {
			eyeCount = HP.GetComponent<HatPicker> ().eyeCount;
			nextlevel = HP.GetComponent<HatPicker> ().nextlevel;
			Destroy (HP);
		}


		bool pair;
		print ("nextleveL: " + nextlevel + "eyecount " + eyeCount);
		if (nextlevel == 4 || nextlevel == 5) { // pair levels (selectduellevel or tutorial
						teamCount = eyeCount / 2;
						pair = true;
				} else { //every player select team levels
					teamCount = eyeCount;
					pair = false;
				}

		for (int i = 0; i <= eyeCount; i++) {
			if (pair) {
				if ( i % 2 == 1 ) {
					GameObject go = Instantiate(teamSpritePair) as GameObject;
					SpriteRenderer spr = go.GetComponent<SpriteRenderer>();
					spr.sprite = teamSprites[i];
					go.transform.parent = GameObject.Find ("Player"+i+"Hat").transform;
					go.transform.localPosition = new Vector3(0.55f, 0.4f,0.0f);
					teams.Add (go);
				}
			} else {
				GameObject go = Instantiate(teamSpriteSingle) as GameObject;
				SpriteRenderer spr = go.GetComponent<SpriteRenderer>();
				spr.sprite = teamSprites[i];
				go.transform.parent = GameObject.Find ("Player"+i+"Hat").transform;
				teams.Add (go);
			}
		}

		MoveObjectSmooth mover = GameObject.Find ("Main Camera").AddComponent<MoveObjectSmooth> ();
		mover.deltaTarget = new Vector3 (0.0f, 1.2f, 0.0f);

	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (count >= teamCount) {
			Destroy (GameObject.Find ("Main Camera"));
			Application.LoadLevel (nextlevel); // the level
		}
	}
}

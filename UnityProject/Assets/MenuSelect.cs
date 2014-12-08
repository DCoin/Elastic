﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuSelect : MonoBehaviour {

	[HideInInspector]
	public int menuselect = 0;
	private float lastHatSelection;
	public float SelectionDelay = 0.3f;
	public GameObject[] MenuItems;
	public Color selectcolor;
	public Color nonSelectcolor;
	public int eyeCount;
	public List<int> eyeChoices = new List<int>();
	private int currentlevel = -1;
	public int nextlevel = -1;
	private SelectArenaMoveScript camScript;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
		
		if (MenuItems.Length == 0) {
			MenuItems = new GameObject[] { gameObject };
		}
		ChangeColor(menuselect);
	}

	void OnLevelWasLoaded (int level) {
		//Change the seletable items on the duel arena select level scene
		if (level == 5) {
			MenuItems = new GameObject[] {GameObject.Find ("Level1"), GameObject.Find ("Level2"), GameObject.Find ("Level3")};
			camScript = GameObject.Find ("Main Camera").GetComponent<SelectArenaMoveScript>();  
				}
		currentlevel = level;
		menuselect = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
				if (lastHatSelection + SelectionDelay < Time.fixedTime) {
						lastHatSelection = Time.fixedTime;
						if (ControllerManager.GetJumpInputBool (0, true)) {
								menuselect--;
								if (menuselect < 0) {
								menuselect = MenuItems.Length-1;
								}
								ChangeColor (menuselect);
				if (currentlevel == 5) {
					print ("ms" + menuselect);
					camScript.newPos = new Vector3 (0.0f, (menuselect*-2.5f),-10.0f);
				}
						} else if (ControllerManager.GetHeavyInputBool (0, true)) {
								menuselect++;
								if (menuselect > MenuItems.Length-1) {
										menuselect = 0;
								}
								ChangeColor (menuselect);
				print ("eh"+currentlevel);
				if (currentlevel == 5) {
					print ("ms" + menuselect);
					camScript.newPos = new Vector3 (0.0f, (menuselect*-2.5f),-10.0f);
				}
						} else if (ControllerManager.GetStickButtonInput (0, true) || ControllerManager.GetAButtonInput (0, true)) {
				if (currentlevel == -1) {
								if (menuselect == 0) {
										eyeChoices = new List<int> {2, 4};
										nextlevel = 4;
										Application.LoadLevel (1); //SelectPlayerNo scene
								} else if (menuselect == 1) {
										eyeChoices = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
										nextlevel = 5;
										Application.LoadLevel (1); //SelectPlayerNo scene
								} else  if (menuselect == 2) {
										Application.LoadLevel (5); //SelectPlayerNo scene
								}
				} else if (currentlevel == 5) {
					if (menuselect == 0) {
						nextlevel = 6; // Duel level 1
					} else if (menuselect == 1) {
						nextlevel = 6; // Duel level 2
					} else  if (menuselect == 2) {
						nextlevel = 6; // Duel level 3
					}
					eyeCount = 4;
					Application.LoadLevel (2); //SelectPlayerNo scene
				}
						}
				}
		}

	void ChangeColor(int menuselect) {
		int i = 0;
		foreach (GameObject go in MenuItems) {
			if (menuselect == i) {
				foreach (TextMesh t in go.GetComponentsInChildren<TextMesh>()) {
				t.color = selectcolor;
						}
			} else {
						foreach (TextMesh t in go.GetComponentsInChildren<TextMesh>()) {
							t.color = nonSelectcolor;
						}
			}
			i++;
				}
	}
}

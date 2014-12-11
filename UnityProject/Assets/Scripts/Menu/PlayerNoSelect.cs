using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerNoSelect : MonoBehaviour {
	
	private float lastHatSelection;
	public float SelectionDelay = 0.3f;
	public int menuselect = 0;
	public int eyeCount;
	private MenuSelect menu;
	public int nextlevel;
	private List<int> eyeChoices;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
	}
	void OnLevelWasLoaded(int level) {
		GameObject menu_go = GameObject.Find ("MenuItems");
		if (menu_go) {
		menu = menu_go.GetComponent<MenuSelect> ();
		eyeChoices = menu.eyeChoices;
		nextlevel = menu.nextlevel;
		GetComponent<TextMesh> ().text = eyeChoices [0].ToString ();
		Destroy (menu_go);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (lastHatSelection + SelectionDelay < Time.fixedTime) {
			lastHatSelection = Time.fixedTime;
			if (ControllerManager.GetJumpInputBool (0, true)) {
				menuselect--;
				if (menuselect < 0) {
					menuselect = eyeChoices.Count-1;
				}
				GetComponent<TextMesh> ().text = eyeChoices[menuselect].ToString();
			}
			else if (ControllerManager.GetHeavyInputBool (0, true)) {
				menuselect++;
				if (menuselect >= eyeChoices.Count) {
					menuselect = 0;
				}
				GetComponent<TextMesh> ().text = eyeChoices[menuselect].ToString();
			} else if (ControllerManager.GetStickButtonInput (0, true) || ControllerManager.GetAButtonInput (0, true)) {
				eyeCount = eyeChoices[menuselect];
				print ("Eyecount in playerno: " + eyeCount);
				Application.LoadLevel(2); //HatPicker scene
			}
		}
	}
}

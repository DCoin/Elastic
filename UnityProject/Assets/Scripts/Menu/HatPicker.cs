using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class HatPicker : MonoBehaviour {

	public int count;
	public AudioClip playerReady;
	public AudioClip eyeColorSwap;
	public AudioClip hatSwap;
	public bool done = false;
	public bool changelevel = false;
	public List<HatManager.HatNames> hatArray;

	[HideInInspector]
	public int eyeCount;
	[HideInInspector]
	public int nextlevel;
	
	void OnLevelWasLoaded(int level) {
		GameObject PN = GameObject.Find ("PlayerNo");
		GameObject MI = GameObject.Find ("MenuItems");
		if (PN) {
			eyeCount = PN.GetComponent<PlayerNoSelect> ().eyeCount;
			nextlevel = PN.GetComponent<PlayerNoSelect> ().nextlevel;
			Destroy (PN);
			Destroy (MI);
		} else {

					if (MI) {
				eyeCount = MI.GetComponent<MenuSelect> ().eyeCount;
				nextlevel = MI.GetComponent<MenuSelect> ().nextlevel;
				Destroy (MI);
			}
				}
	}
	// Use this for initialization
	void Start () {
		hatArray = HatManager.HatNames.GetValues(typeof(HatManager.HatNames)).Cast<HatManager.HatNames>().ToList();
		count = 0;
		GameObject mainCamera = GameObject.Find ("Main Camera");
		if (eyeCount >= 1 && eyeCount <= 3) {
				mainCamera.transform.position = new Vector3 (-4.0f + (float)eyeCount, mainCamera.transform.position.y, mainCamera.transform.position.z);
				} else if (eyeCount >= 5 && eyeCount <= 6) {
					//2 rows, 3 columns
					mainCamera.transform.position = new Vector3 (-1, -35.5f, mainCamera.transform.position.z);
					mainCamera.camera.orthographicSize = 3.3f;

					//Move players
					for (int i = 4; i <= 6; i++) {
						int next = i+1;
						GameObject.Find ("Player"+i+"Hat").transform.position = GameObject.Find ("Player"+next+"Hat").transform.position;
					}
				} else if (eyeCount != 4) {
					//2 rows, 4 columns
					mainCamera.transform.position = new Vector3 (mainCamera.transform.position.x, -35.5f, mainCamera.transform.position.z);
					mainCamera.camera.orthographicSize = 3.3f;
				}
		
		//Remove unneeded eyes and let the needed ones pass on.
		for (int i = 8; i > 0; i--) {
			if (i > eyeCount) {
			Destroy (GameObject.Find ("Player"+i+"Hat"));
			} else {
				DontDestroyOnLoad(GameObject.Find ("Player"+i+"Hat"));
			}
		}
		DontDestroyOnLoad (gameObject); //We save nextlevel and eyecount here
		DontDestroyOnLoad (GameObject.Find ("Main Camera")); // We dont to fix the camera twice
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (count >= eyeCount) {
						Application.LoadLevel (3); // team picker level
				}
	}
}

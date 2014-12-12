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
	public Font headingFont;
	public string headingText;

	[HideInInspector]
	public List<GameObject> allPlayers;
	[HideInInspector]
	public int eyeCount;
	[HideInInspector]
	public int nextlevel;

	
	void OnLevelWasLoaded(int level) {
		GameObject MI = GameObject.Find ("MenuItems");
			if (MI) {
				eyeCount = MI.GetComponent<MenuSelect> ().eyeCount;
				print ("Eyecount from MI: " + eyeCount);
				nextlevel = MI.GetComponent<MenuSelect> ().nextlevel;
			}
	}

	private void OnGUI()
	{
		GUIStyle myStyle = new GUIStyle();
		myStyle.normal.textColor = Color.white;
		myStyle.font = headingFont;
		myStyle.alignment = TextAnchor.UpperCenter;
		myStyle.fontSize = 60;
		GUI.Label (new Rect (Screen.width/2-50, 10, 100, 50), headingText, myStyle);
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
					mainCamera.camera.orthographicSize = 4f;

					//Move players
					for (int i = 4; i <= 6; i++) {
						int next = i+1;
						GameObject.Find ("Player"+i+"Hat").transform.position = GameObject.Find ("Player"+next+"Hat").transform.position;
					}
				} else if (eyeCount != 4) {
					//2 rows, 4 columns
					mainCamera.transform.position = new Vector3 (mainCamera.transform.position.x, -35.5f, mainCamera.transform.position.z);
					mainCamera.camera.orthographicSize = 4f;
				}
		allPlayers = new List<GameObject>();
		//Remove unneeded eyes and let the needed ones pass on.
		for (int i = 8; i > 0; i--) {
			if (i > eyeCount) {
			Destroy (GameObject.Find ("Player"+i+"Hat"));
			} else {
				DontDestroyOnLoad(GameObject.Find ("Player"+i+"Hat"));
				allPlayers.Add (GameObject.Find ("Player"+i+"Hat"));
			}
		}
		DontDestroyOnLoad (gameObject); //We save nextlevel and eyecount here
		DontDestroyOnLoad (GameObject.Find ("Main Camera")); // We dont to fix the camera twice
	}
	
	// Update is called once per frame
	void Update () {
		if (count >= eyeCount) {
			count = 0;
			Destroy (gameObject);
				Application.LoadLevel (3); // team picker level
				}
	}
}

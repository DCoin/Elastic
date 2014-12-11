﻿using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;

public class HatSelectionController : MonoBehaviour {
	
	public int controller = 0;
	public bool leftSide = true;

	private System.Array colorArray;
	public int hatindex = 0;
	private int teamindex = 0;
	private int colorindex = 0;
	public float hatSelectionDelay = 0.2f;
	private float lastHatSelection;
	private bool checkedOut = false;
	private List<Color> colors = new List<Color>();
	public GameObject hatPicker;
	private HatPicker hatPickerScript;
	public HatManager.HatNames  hat;
	public bool setHats = false;


	// Use this for initialization
	void Start () {
		colors.Add (Color.black);
		colors.Add (Color.blue);
		colors.Add (new Color(159.0f/255.0f, 200.0f/255.0f, 255.0f/255.0f)); // light blue
		colors.Add (Color.white);
		colors.Add (Color.cyan);
		colors.Add (Color.gray);
		colors.Add (Color.green);
		colors.Add (new Color(255.0f/255.0f, 165.0f/255.0f, 0.0f/255.0f)); // orange
		colors.Add (new Color(135.0f/255.0f, 42.0f/255.0f, 42.0f/255.0f)); // brown
		colors.Add (new Color(255.0f/255.0f, 192.0f/255.0f, 203.0f/255.0f)); // pink
		colors.Add (Color.magenta);
		colors.Add (Color.red);
		colors.Add (Color.yellow);
		Object.DontDestroyOnLoad (gameObject);
		hatPickerScript = GameObject.Find ("HatPicker").GetComponent<HatPicker> ();
	}

	
	// Update is called once per frame
	void Update () {
		if (lastHatSelection + hatSelectionDelay < Time.fixedTime) {
			lastHatSelection = Time.fixedTime;
			if (ControllerManager.GetJumpInputBool (controller, leftSide) && !checkedOut) {
				hatindex--;
				if (hatindex < 0) {
					hatindex = hatPickerScript.hatArray.Count-1;
				}
				hat = hatPickerScript.hatArray [hatindex];
				GetComponent<EyeAnimator>().ChangeHat(hat);

				audio.clip = hatPickerScript.hatSwap;
				audio.Play();
		}
			else if (ControllerManager.GetHeavyInputBool (controller, leftSide) && !checkedOut) {
				hatindex++;
				if (hatindex > hatPickerScript.hatArray.Count-1) {
					hatindex = 0;
				}
				hat = hatPickerScript.hatArray [hatindex];
				GetComponent<EyeAnimator>().ChangeHat(hat);
				
				audio.clip = hatPickerScript.hatSwap;
				audio.Play();
			}
			else if (!Mathf.Approximately(ControllerManager.GetHorizontalInput(controller, leftSide), 0f) && !checkedOut) {
				float dir = ControllerManager.GetHorizontalInput(controller, leftSide);
				int incr = 0;
				if (dir > 0.33f) {
					incr = 1;
				} else if (dir < -0.33f) {
					incr = -1;
				}
				colorindex = colorindex + incr;
				if (colorindex < 0) {
					colorindex = colors.Count-1;
				} else if (colorindex > colors.Count-1) {
					colorindex = 0;
				}
				print(colorindex);
				GetComponent<EyeAnimator>().SetIrisColor(colors[colorindex]);
				audio.clip = hatPickerScript.eyeColorSwap;
				audio.Play();
			}
		else if (ControllerManager.GetStickButtonInput (controller, leftSide)) {
				if (!checkedOut) {
					checkedOut = true;
					foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) {
						mesh.enabled = true;
					}
					hatPickerScript.count += 1;
					hatPickerScript.hatArray.Remove(hat);
					//Remove hats from other players
					RemoveHat(hat);
				} else {
					checkedOut = false;
					foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) {
						mesh.enabled = false;
					}
					hatPickerScript.count -= 1;
					hatPickerScript.hatArray.Add (hat);
				}
				
				audio.clip = hatPickerScript.playerReady;
				audio.Play();
		}
		}
			
		}

	void RemoveHat(HatManager.HatNames hat) {
		foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player")) {
			if (p.transform.name != transform.name) {
				var HatSel = p.GetComponent<HatSelectionController>();
				if (HatSel.hat == hat) {
					HatSel.hatindex++;
					if (HatSel.hatindex > hatPickerScript.hatArray.Count-1) {
						HatSel.hatindex = 0;
					}
					HatSel.hat = hatPickerScript.hatArray[HatSel.hatindex];
					p.GetComponent<EyeAnimator>().ChangeHat(HatSel.hat);
				}
			}
		}
	}
}

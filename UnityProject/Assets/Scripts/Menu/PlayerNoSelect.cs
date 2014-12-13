using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerNoSelect : MonoBehaviour
{
	
	private float lastHatSelection;
	public float SelectionDelay = 0.3f;
	public int menuselect = 0;
	public int eyeCount;
	private MenuSelect menu;
	public int nextlevel;
	private List<int> eyeChoices;
	private Vector3 camPosition;
	private GameObject menu_go;

	public Font headingFont;
	public string headingText;

	public Texture MenuOptions;

	private void OnGUI()
	{
		GUIStyle myStyle = new GUIStyle();
		myStyle.normal.textColor = Color.white;
		myStyle.font = headingFont;
		myStyle.alignment = TextAnchor.UpperCenter;
		myStyle.fontSize = Screen.width/20;
		GUI.Label (new Rect (Screen.width/2-Screen.width/22, 10, 100, 50), headingText, myStyle);

		//Draw menu options
		GUI.DrawTexture(new Rect(Screen.width/2-(Screen.width/12),(Screen.height-Screen.height/4),(Screen.width/6),(Screen.width/6)), MenuOptions, ScaleMode.ScaleToFit, true, 0.0f);

	}

	// Use this for initialization
	void Start()
	{
	}
	void OnLevelWasLoaded(int level)
	{
		print ("Playerselect scene loaded from lvl " + level);
	    menu_go = GameObject.Find("MenuItems");
		if (menu_go)
		{
			menu = menu_go.GetComponent<MenuSelect>();
			eyeChoices = menu.eyeChoices;
			nextlevel = menu.nextlevel;
			GetComponent<TextMesh>().text = eyeChoices[0].ToString();
			menu.GetComponent<MenuSelect>().enabled = false;
			//Destroy(menu_go);
		}
		//Set the camera
		camPosition = GameObject.Find ("Main Camera").transform.position;
		if (level == 2) {
					GameObject.Find ("Main Camera").transform.position = camPosition;
				}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (ControllerManager.GetJumpInputBool(0, true))
		{
			if (lastHatSelection + SelectionDelay < Time.fixedTime)
			{
				// Play sound!
				SoundManager.PlaySound(SoundManager.SoundTypes.Menu_Nav);

				lastHatSelection = Time.fixedTime;
				menuselect--;
				if (menuselect < 0)
				{
					menuselect = eyeChoices.Count - 1;
				}
				GetComponent<TextMesh>().text = eyeChoices[menuselect].ToString();
			}
		}
		else if (ControllerManager.GetHeavyInputBool(0, true))
		{
			// Play sound!
			SoundManager.PlaySound(SoundManager.SoundTypes.Menu_Nav);

			if (lastHatSelection + SelectionDelay < Time.fixedTime)
			{
				lastHatSelection = Time.fixedTime;
				menuselect++;
				if (menuselect >= eyeChoices.Count)
				{
					menuselect = 0;
				}
				GetComponent<TextMesh>().text = eyeChoices[menuselect].ToString();
			}
		}
		if (ControllerManager.GetStickButtonInput(0, true) || ControllerManager.GetAButtonInput(0))
		{
			// Play sound!
			SoundManager.PlaySound(SoundManager.SoundTypes.Menu_Select);

			eyeCount = eyeChoices[menuselect];
			menu_go.GetComponent<MenuSelect>().eyeCount = eyeCount;
			menu_go.GetComponent<MenuSelect>().eyeChoices = eyeChoices;
			gameObject.GetComponent<PlayerNoSelect>().enabled = false;
			Application.LoadLevel(2); //HatPicker scene
		}
		else if (ControllerManager.GetBButtonInput(0))
		{
			// Play sound!
			SoundManager.PlaySound(SoundManager.SoundTypes.Menu_Back);

			Destroy(gameObject);
			Destroy (menu.gameObject);
			print ("Loading title screen");
			Application.LoadLevel(0); //Title screen
		}
		
	}
}

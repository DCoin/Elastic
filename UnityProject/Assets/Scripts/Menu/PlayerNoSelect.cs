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
			eyeCount = eyeChoices[menuselect];
			menu_go.GetComponent<MenuSelect>().eyeCount = eyeCount;
			menu_go.GetComponent<MenuSelect>().eyeChoices = eyeChoices;
			gameObject.GetComponent<PlayerNoSelect>().enabled = false;
			Application.LoadLevel(2); //HatPicker scene
		}
		else if (ControllerManager.GetBButtonInput(0))
		{
			Destroy(gameObject);
			Destroy (menu.gameObject);
			print ("Loading title screen");
			Application.LoadLevel(0); //Title screen
		}
		
	}
}

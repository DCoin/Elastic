using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;

public class TeamSelectionController : MonoBehaviour
{
	
	public int controller = 0;
	public bool leftSide = true;
	
	public int teamindex = 0;
	public float teamSelectionDelay = 0.2f;
	private float lastTeamSelection;
	private bool checkedOut = false;
	private TeamPicker teamPickerScript;
	
	public bool single = false;
	
	public TeamPicker.Team currentTeam;
	public SpriteRenderer currentSprite;
	public LineRenderer currentRope;
	
	// Use this for initialization
	void Start()
	{
		teamPickerScript = GameObject.Find("TeamPicker").GetComponent<TeamPicker>();
		currentSprite = GetComponent<SpriteRenderer>();
		currentSprite.sprite = currentTeam.teamSprite;
		
		currentRope = transform.root.GetComponentInChildren<LineRenderer>();
		if (currentRope)
		{
			currentRope.material = currentTeam.teamColor;
		}
	}
	
	
	// Update is called once per frame
	void Update()
	{
		
		if (ControllerManager.GetJumpInputBool(controller, leftSide) && !checkedOut)
		{
			if (lastTeamSelection + teamSelectionDelay < Time.fixedTime)
			{
				lastTeamSelection = Time.fixedTime;
				teamindex--;
				if (teamindex < 0)
				{
					teamindex = teamPickerScript.teamVisual.Count - 1;
				}
				currentTeam = teamPickerScript.teamVisual[teamindex];
				currentSprite.sprite = currentTeam.teamSprite;
				if (currentRope)
				{
					currentRope.material = currentTeam.teamColor;
				}
			}
			
			//audio.clip = teamPickerScript.hatSwap;
			//audio.Play();
		}
		else if (ControllerManager.GetHeavyInputBool(controller, leftSide) && !checkedOut)
		{
			if (lastTeamSelection + teamSelectionDelay < Time.fixedTime)
			{
				lastTeamSelection = Time.fixedTime;
				teamindex++;
				if (teamindex > teamPickerScript.teamVisual.Count - 1)
				{
					teamindex = 0;
				}
				currentTeam = teamPickerScript.teamVisual[teamindex];
				currentSprite.sprite = currentTeam.teamSprite;
				if (currentRope)
				{
					currentRope.material = currentTeam.teamColor;
				}
			}
			//audio.clip = teamPickerScript.hatSwap;
			//audio.Play();
		}
		if (ControllerManager.GetStickButtonInput(controller, leftSide))
		{
			if (lastTeamSelection + teamSelectionDelay * 2 < Time.fixedTime)
			{
				lastTeamSelection = Time.fixedTime;
				if (!checkedOut)
				{
					checkedOut = true;
					GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
					teamPickerScript.count = teamPickerScript.count + 1;
					if (!single)
					{
						teamPickerScript.teamVisual.Remove(currentTeam);
						RemoveTeamSprite(currentTeam);
					}
				}
				else
				{
					checkedOut = false;
					GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
					teamPickerScript.count = teamPickerScript.count - 1;
					if (!single)
					{
						teamPickerScript.teamVisual.Add(currentTeam);
					}
				}
			}


			//audio.clip = teamPickerScript.playerReady;
			//audio.Play();
		}
		else if (ControllerManager.GetBButtonInput (controller)) {
			print ("Pressing back on teampicker scene");
			if (lastTeamSelection + teamSelectionDelay < Time.fixedTime)
			{
				lastTeamSelection = Time.fixedTime;
				//Destroy all hats in the scene
				foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player")) {
					print(p.name);
					Destroy (p);
				}
				Destroy (GameObject.Find ("Main Camera"));
				Destroy (teamPickerScript.gameObject);
				Destroy (GameObject.Find ("HeadingTeam"));

				//Destroy teams in the teampicker
				foreach (GameObject t in teamPickerScript.teams) {
					Destroy (t);
				}
				Application.LoadLevel(2);
			}
		}
		
	}
	
	void RemoveTeamSprite(TeamPicker.Team teamSprite)
	{
		foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
		{
			//Get all other team sprites
			if (p.transform.root.name != transform.root.name)
			{
				var TeamSel = p.GetComponentInChildren<TeamSelectionController>();
				if (TeamSel)
				{
					if (TeamSel.currentTeam == currentTeam)
					{
						TeamSel.teamindex++;
						if (TeamSel.teamindex > teamPickerScript.teamVisual.Count - 1)
						{
							TeamSel.teamindex = 0;
						}
						TeamSel.currentTeam = teamPickerScript.teamVisual[TeamSel.teamindex];
						TeamSel.currentSprite.sprite = TeamSel.currentTeam.teamSprite;
						if (TeamSel.currentRope)
						{
							TeamSel.currentRope.material = TeamSel.currentTeam.teamColor;
						}
					}
				}
			}
		}
	}
}

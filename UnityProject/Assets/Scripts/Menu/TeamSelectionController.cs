using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;

public class TeamSelectionController : MonoBehaviour {
	
	public int controller = 0;
	public bool leftSide = true;

	public int teamindex = 0;
	public float teamSelectionDelay = 0.2f;
	private float lastTeamSelection;
	private bool checkedOut = false;
	private TeamPicker teamPickerScript;

	public TeamPicker.Team currentTeam;
	public SpriteRenderer currentSprite;
	public LineRenderer currentRope;

	// Use this for initialization
	void Start () {
		teamPickerScript = GameObject.Find ("TeamPicker").GetComponent<TeamPicker> ();
		currentSprite = GetComponent<SpriteRenderer> ();
		currentSprite.sprite = currentTeam.teamSprite;
		
		currentRope = transform.root.GetComponentInChildren<LineRenderer> ();
		if (currentRope) {
						currentRope.material = currentTeam.teamColor;
				}
	}
	
	
	// Update is called once per frame
	void FixedUpdate () {
		if (lastTeamSelection + teamSelectionDelay < Time.fixedTime) {
			lastTeamSelection = Time.fixedTime;
			if (ControllerManager.GetJumpInputBool (controller, leftSide) && !checkedOut) {
				teamindex--;
				if (teamindex < 0) {
					teamindex = teamPickerScript.teamVisual.Count-1;
				}
				currentTeam = teamPickerScript.teamVisual [teamindex];
				currentSprite.sprite = currentTeam.teamSprite;
				if (currentRope) {
				currentRope.material = currentTeam.teamColor;
				}
				
				//audio.clip = teamPickerScript.hatSwap;
				//audio.Play();
			}
			else if (ControllerManager.GetHeavyInputBool (controller, leftSide) && !checkedOut) {
				teamindex++;
				if (teamindex > teamPickerScript.teamVisual.Count-1) {
					teamindex = 0;
				}
				currentTeam = teamPickerScript.teamVisual [teamindex];
				currentSprite.sprite = currentTeam.teamSprite;
				if (currentRope) { 
				currentRope.material = currentTeam.teamColor;
				}
				
				//audio.clip = teamPickerScript.hatSwap;
				//audio.Play();
			}
			else if (ControllerManager.GetStickButtonInput (controller, leftSide)) {
				if (!checkedOut) {
					checkedOut = true;
					GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
					teamPickerScript.count = teamPickerScript.count + 1;
					teamPickerScript.teamVisual.Remove(currentTeam);
					RemoveTeamSprite(currentTeam);
				} else {
					checkedOut = false;
					GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);
					teamPickerScript.count = teamPickerScript.count - 1;
					teamPickerScript.teamVisual.Add (currentTeam);
				}
				
				//audio.clip = teamPickerScript.playerReady;
				//audio.Play();
			}
		}
		
	}
	
	void RemoveTeamSprite(TeamPicker.Team teamSprite) {
		foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player")) {
			//Get all other team sprites
			if (p.transform.root.name != transform.root.name) {
				var TeamSel = p.GetComponentInChildren<TeamSelectionController>();
				if (TeamSel) {
					if (TeamSel.currentTeam == currentTeam) {
						TeamSel.teamindex++;
						if (TeamSel.teamindex > teamPickerScript.teamVisual.Count-1) {
							TeamSel.teamindex = 0;
						}
						TeamSel.currentTeam = teamPickerScript.teamVisual[TeamSel.teamindex];
						TeamSel.currentSprite.sprite = TeamSel.currentTeam.teamSprite;
						if (TeamSel.currentRope) {
						TeamSel.currentRope.material = TeamSel.currentTeam.teamColor;
						}
					}
				}
			}
		}
	}
}

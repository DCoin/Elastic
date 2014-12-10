using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;

public class TeamSelectionController : MonoBehaviour {
	
	public int controller = 0;
	public bool leftSide = true;

	private int teamindex = 0;
	public float teamSelectionDelay = 0.2f;
	private float lastTeamSelection;
	private bool checkedOut = false;
	private TeamPicker teamPickerScript;
	public Sprite teamSprite;
	private SpriteRenderer currentSprite;

	// Use this for initialization
	void Start () {
		teamPickerScript = GameObject.Find ("TeamPicker").GetComponent<TeamPicker> ();
		currentSprite = GetComponent<SpriteRenderer> ();
	}
	
	
	// Update is called once per frame
	void FixedUpdate () {
		if (lastTeamSelection + teamSelectionDelay < Time.fixedTime) {
			lastTeamSelection = Time.fixedTime;
			if (ControllerManager.GetJumpInputBool (controller, leftSide) && !checkedOut) {
				print ("heyman");
				teamindex--;
				if (teamindex < 0) {
					teamindex = teamPickerScript.teamSprites.Count-1;
				}
				teamSprite = teamPickerScript.teamSprites [teamindex];
				currentSprite.sprite = teamSprite;
				
				//audio.clip = teamPickerScript.hatSwap;
				//audio.Play();
			}
			else if (ControllerManager.GetHeavyInputBool (controller, leftSide) && !checkedOut) {
				teamindex++;
				if (teamindex > teamPickerScript.teamSprites.Count-1) {
					teamindex = 0;
				}
				teamSprite = teamPickerScript.teamSprites [teamindex];
				currentSprite.sprite = teamSprite;
				
				//audio.clip = teamPickerScript.hatSwap;
				//audio.Play();
			}
			else if (ControllerManager.GetStickButtonInput (controller, leftSide)) {
				if (!checkedOut) {
					checkedOut = true;
					foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) {
						mesh.enabled = true;
					}
					teamPickerScript.count += 1;
					teamPickerScript.teamSprites.Remove(teamSprite);
					RemoveTeamSprite(teamSprite);
				} else {
					checkedOut = false;
					foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) {
						mesh.enabled = false;
					}
					teamPickerScript.count -= 1;
					teamPickerScript.teamSprites.Add (teamSprite);
				}
				
				//audio.clip = teamPickerScript.playerReady;
				//audio.Play();
			}
		}
		
	}
	
	void RemoveTeamSprite(Sprite teamSprite) {
		foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player")) {
			if (p.transform.name != transform.name) {
				var TeamSel = p.GetComponentInChildren<TeamSelectionController>();
				if (TeamSel.teamSprite == teamSprite) {
					TeamSel.teamindex++;
					if (TeamSel.teamindex > teamPickerScript.teamSprites.Count-1) {
						TeamSel.teamindex = 0;
					}
					TeamSel.teamSprite = teamPickerScript.teamSprites[TeamSel.teamindex];
					TeamSel.currentSprite.sprite = TeamSel.teamSprite;
				}
			}
		}
	}
}

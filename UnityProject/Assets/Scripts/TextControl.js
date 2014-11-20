﻿#pragma strict

function Start () {}

var IsStoryButton = false;
var IsTutorialButton = false;
var IsLevelButton = false;
var IsQuitButton = false;
	
function OnMouseEnter()
{
	renderer.material.color = Color.green;
}

function OnMouseExit()
{
renderer.material.color = Color.white;
} 

function OnMouseUp()
{
	if (IsStoryButton){
			Application.LoadLevel(3);
			}
	else if (IsTutorialButton) {
			Application.LoadLevel(1);
				} 
	else if (IsLevelButton) {
			Application.LoadLevel(2);
				}
	else if (IsQuitButton){
			Application.Quit ();
				}
}



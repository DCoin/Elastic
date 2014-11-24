﻿using UnityEngine;
using System.Collections;

public class SwitchScript : MonoBehaviour {

	public Transform door;
	[HideInInspector]
	public Vector3 targetPosition = Vector3.zero;
	public float smoothTime = 0.3F;
	public string Squadname = "Squad";
	public float moveDoorUpDistance = 10;
	public Sprite switchOnSprite;

	private Vector3 velocity = Vector3.zero;
	private bool moveDoor = false;
	// Use this for initialization
	void Start () {
		targetPosition = door.transform.position + new Vector3 (0, moveDoorUpDistance, 0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (moveDoor) {
			// Smoothly move the camera towards that target position
			door.transform.position = Vector3.SmoothDamp (door.transform.position, targetPosition, ref velocity, smoothTime);
			if (door.transform.position == targetPosition) {
				moveDoor = false;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.transform.root.name.Contains(Squadname)) {
			moveDoor = true;
			transform.GetComponent<SpriteRenderer>().sprite = switchOnSprite;
			
		}
	}
}

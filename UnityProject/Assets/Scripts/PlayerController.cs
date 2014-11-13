using UnityEngine;
using System.Collections;
using InControl;

public class PlayerController : MonoBehaviour {

	public float speed = 10f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		InputDevice activeDevice = InputManager.ActiveDevice;
		transform.Translate(Vector2.right*speed*Time.deltaTime*activeDevice.RightStickX.Value); // THIS IS SHIT AND MUST GO AWAY!!!
	}
}

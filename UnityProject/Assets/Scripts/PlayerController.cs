using UnityEngine;
using System.Collections;
using InControl;

public class PlayerController : MonoBehaviour {

	public float speed = 10f;
	public int controller = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var inputDevice = (InputManager.Devices.Count > controller) ? InputManager.Devices[controller] : null;
		if (inputDevice == null) {
			//TODO Do something appropriate
		} else {
			transform.Translate(Vector2.right * speed * Time.deltaTime * inputDevice.RightStickX.Value); // THIS IS SHIT AND MUST GO AWAY!!!
		}
	}
}

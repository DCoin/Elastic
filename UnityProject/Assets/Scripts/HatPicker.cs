using UnityEngine;
using System.Collections;

public class HatPicker : MonoBehaviour {

	public int count;
	public AudioClip playerReady;
	public AudioClip eyeColorSwap;
	public AudioClip hatSwap;

	// Use this for initialization
	void Start () {

		count = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		print("Count: " + count);
		if (count >= 4) {
			print("HEY MAN");
			Application.LoadLevel(1);
		}
	}
}

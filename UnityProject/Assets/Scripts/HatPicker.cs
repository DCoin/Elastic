using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HatPicker : MonoBehaviour {

	public int count;
	public AudioClip playerReady;
	public AudioClip eyeColorSwap;
	public AudioClip hatSwap;
	public List<HatManager.HatNames> hatArray;

	// Use this for initialization
	void Start () {
		hatArray = HatManager.HatNames.GetValues(typeof(HatManager.HatNames)).Cast<HatManager.HatNames>().ToList();
		count = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (count >= 4) {
			Application.LoadLevel(1);
		}
	}
}

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

	private int eyeCount;
	private int nextlevel;
	
	void OnLevelWasLoaded(int level) {
		GameObject PN = GameObject.Find ("PlayerNo");
		if (PN) {
			eyeCount = PN.GetComponent<PlayerNoSelect> ().eyeCount;
			nextlevel = PN.GetComponent<PlayerNoSelect> ().nextlevel;
						Destroy (PN);
				} else {
			
			GameObject MI = GameObject.Find ("MenuItems");
					if (MI) {
				eyeCount = MI.GetComponent<MenuSelect> ().eyeCount;
				nextlevel = MI.GetComponent<MenuSelect> ().nextlevel;
				Destroy (MI);
			}
				}
	}
	// Use this for initialization
	void Start () {
		hatArray = HatManager.HatNames.GetValues(typeof(HatManager.HatNames)).Cast<HatManager.HatNames>().ToList();
		count = 0;
		GameObject mainCamera = GameObject.Find ("Main Camera");
		GameObject mo = GameObject.Find ("Menu Options");
		if (eyeCount >= 1 && eyeCount <= 3) {
			mainCamera.transform.position = new Vector3(-4.0f+(float)eyeCount, mainCamera.transform.position.y,  mainCamera.transform.position.z);
			mo.transform.position = new Vector3(mo.transform.position.x-4.0f+(float)eyeCount, mo.transform.position.y,  mo.transform.position.z);

		}
		for (int i = 8; i > eyeCount; i--) {
			Destroy (GameObject.Find ("Player"+i+"Hat"));
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (count >= eyeCount) {
			Application.LoadLevel(nextlevel);
		}
	}
}

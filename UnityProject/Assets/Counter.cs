using UnityEngine;
using System.Collections;

public class Counter : MonoBehaviour {

	public int count;

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

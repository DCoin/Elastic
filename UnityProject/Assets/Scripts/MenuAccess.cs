using UnityEngine;
using System.Collections;

public class MenuAccess : MonoBehaviour {

	void Update () {
		// Go to menu on escape
		if (Input.GetKey(KeyCode.Escape)) Application.LoadLevel(0);
	}
}

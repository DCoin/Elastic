using UnityEngine;
using System.Collections;

public class RopeHitScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.rigidbody) {
						if (col.transform.root.name != transform.root.name && col.rigidbody.mass > 10 && col.transform.name != "Ground" && col.transform.name != "Ceiling") {
								Die ();
						}
				}
	}

	void Die() {
		Debug.Log ("Die " + transform.root.name);
		foreach (Transform child in transform.root) {
						if (child.name.Contains ("Sphere")) {
								child.transform.localPosition = Vector3.zero;
				child.rigidbody2D.angularVelocity = 0.0F;
				child.rigidbody2D.velocity = Vector2.zero;
			} else if (child.name == "Rope2") {
				Destroy(child.GetComponent<RopeScript>().objects[1].GetComponent<SpringJoint2D>());
				Destroy(child.gameObject);
			}
				}
		
		transform.root.transform.position = new Vector3(60,-100,0);
		//transform.root.GetComponent<PlayerData> ().hide = true;
	}
}

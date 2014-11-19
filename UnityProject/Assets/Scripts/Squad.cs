using UnityEngine;
using System.Collections;

public class Squad : MonoBehaviour {
	
	public bool hide = false;
	
	// Use this for initialization
	void Start () {
		hide = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (hide) {
			hide = false;
			
			foreach (Transform c in transform) {
				c.gameObject.SetActive (false);
			}
			StartCoroutine (HideAndWait());
			
		}
	}
	
	
	IEnumerator HideAndWait()
	{  
		var go = GameObject.Find ("DuelLogic");
		yield return new WaitForSeconds(go.GetComponent<DuelMode>().RespawnTime);
		
		foreach (Transform c in transform) {
			c.gameObject.SetActive (true);
		}
		
		//For some reason the spring does respond after being reactivated. This hack fixes it.
		yield return new WaitForSeconds(0.001F);
		SpringJoint2D the_broken_joint = null;
		foreach (Transform c in transform) {
			if (c.name == "Rope") {
				the_broken_joint = c.GetComponent<RopeScript>().objects[1].GetComponent<SpringJoint2D>();
			}
		}
		
		the_broken_joint.enabled = false;
		yield return new WaitForSeconds(0.001F);
		the_broken_joint.enabled = true;
	}
}

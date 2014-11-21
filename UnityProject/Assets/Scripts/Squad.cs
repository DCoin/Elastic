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
		GetComponentInChildren<RopeScript> ().BuildRope ();
	}
}

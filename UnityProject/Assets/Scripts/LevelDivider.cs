using UnityEngine;
using System.Collections;

public class LevelDivider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D col) {
		// New Rope
		//if (col.gameObject.GetComponentInParent<Squad>()
		
		// Old rope
		if (col.transform.root.Find ("PickupRope")) { // I dont like this but im going to keep using it anyway
				transform.collider2D.isTrigger = true;
				}
	}

	void OnTriggerExit2D(Collider2D col) 
	{
		StartCoroutine (WaitAndSetTrigger(col.transform.root));
	}

	IEnumerator WaitAndSetTrigger(Transform squad)
	{  
		yield return new WaitForSeconds(3);
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		float width = mesh.bounds.size.x;
		bool removetrigger = true;
		foreach (Collider2D col in squad.GetComponentsInChildren<Collider2D>()) {
			if (col.transform.position.x > transform.position.x-width/2 && col.transform.position.x < transform.position.x+width/2) {
				removetrigger = false;
				break;
			}
		}
		if (removetrigger) {
						transform.collider2D.isTrigger = false;
		} else {  WaitAndSetTrigger(squad); }
	}
}

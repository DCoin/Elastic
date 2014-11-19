using UnityEngine;
using System.Collections;

public class RopeHitScript : MonoBehaviour {

	private bool showRespawnAnimation = false;
	public Sprite respawnLocationSprite;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnCollisionEnter2D(Collision2D col) {
		if (col.rigidbody) {
						if (col.transform.root.name != transform.root.name && col.rigidbody.mass > 4) {
								Die ();
						}
				}
	}
	
	void Die() {
		Debug.Log ("Die " + transform.root.name);
		foreach (Transform child in transform.root) {
			child.transform.localPosition = Vector3.zero;
			if (child.name.Contains ("Player")) {
				child.rigidbody2D.angularVelocity = 0.0F;
				child.rigidbody2D.velocity = Vector2.zero;
			} else if (child.name == "Rope2") {
				Destroy(child.GetComponent<RopeScript>().objects[1].GetComponent<SpringJoint2D>());
				Destroy(child.gameObject);
			}
		}
		GameObject closest = null;
		float dist = int.MaxValue;
		foreach(GameObject rp in GameObject.FindGameObjectsWithTag("RespawnPoint"))
		{
			if (rp.transform.name.Contains(transform.root.name.Substring(transform.root.name.Length-1,1))){
				var d = Vector3.Distance(transform.position, rp.transform.position);
				if (d < dist) {
					if (isInLevel(rp)) {
						closest = rp;
						dist = d;
					}
				}
			}
		}
		//SpriteRenderer spr = closest.AddComponent<SpriteRenderer> ();
		//spr.sprite = (Sprite)Resources.Load ("blue-circle");	
		transform.root.transform.position = closest.transform.position;
		transform.root.GetComponent<Squad> ().hide = true;
	}
	
	private bool isInLevel(GameObject go) {
		GameObject duellogic = GameObject.Find ("DuelLogic");
		int levelstart = duellogic.GetComponent<DuelMode> ().levelstart;
		int levelend = duellogic.GetComponent<DuelMode> ().levelend;
		return (levelstart < go.transform.position.x && levelend > go.transform.position.x);
	}
	
}

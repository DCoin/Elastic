using UnityEngine;
using System.Collections;

public class Squad : MonoBehaviour {
	// TODO the current implementation of the squad kill/respawn is only one way of doing it.
	// we should investigate other ways of doing it. maybe allowing independant movement when severed,
	// and then players attempt for reattachment?

	public bool hide = false;

	private Vector2 respawnPoint;

	// Use this for initialization
	void Awake () {
		hide = false;

		// Set starting respawn point to start position
		// This can be overwritten later with the SetRespawnPoint method
		respawnPoint = transform.position;
	}
	
	// Update is called once per frame
	/*void Update () {
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
	} */

	/// <summary>
	/// Sets the respawn point of this squad.
	/// </summary>
	/// <param name="point">Point.</param>
	public void SetRespawnPoint(Vector2 point) {
		this.respawnPoint = point;
	}
	
	// TODO proper method comment
	public void Respawn() {
		// TODO assign proper methods in the player objects for handling this responsibility
		// Reset position of parenting (this) squad
		transform.position = respawnPoint;

		// Reset velocity relative position of players
		foreach (var pc in transform.GetComponentsInChildren<PlayerController>()) {
			pc.transform.localPosition = Vector2.zero;
			pc.rigidbody2D.velocity = Vector2.zero;
			pc.rigidbody2D.angularVelocity = .0f;
		}
		// Reset velocity and position of rope
		foreach (var rs in transform.GetComponentsInChildren<RopeScript>()) {
			rs.ResetPosition();
		}
		// Reactivate all gameobjects
		foreach (var child in transform) {
			var c = child as Transform;
			c.gameObject.SetActive(true); // Dunno if this is right?
		}
	}

	// TODO Proper method comment
	public void Kill(float respawnTime) {
		Invoke("Respawn", respawnTime);
		Kill ();
	}

	// TODO proper method comment
	public void Kill() {
		// Reactivate all gameobjects
		foreach (var child in transform) {
			var c = child as Transform;
			c.gameObject.SetActive(false); // Dunno if this is right?
		}
		// Anything else needs doing to disable?
		// Maybe call to check for rope-severing?
	}
}

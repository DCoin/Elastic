using UnityEngine;
using System.Collections;

public class SelectArenaMoveScript : MonoBehaviour {

	
	private bool move = false;
	public Vector3 newPos;
	private Vector3 velocity = Vector3.zero;
	public float smoothTime = 0.1F;
	private MenuSelect menu;
	private int oldmenuselect = 0;

	// Use this for initialization
	void Start () {
		menu = GameObject.Find ("MenuItems").GetComponent<MenuSelect> ();
		newPos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (move || menu.menuselect != oldmenuselect) {
			if (transform.position != newPos){
				// Smoothly move the camera towards that target position
				transform.position = Vector3.SmoothDamp (transform.position, newPos, ref velocity, smoothTime);
			} else {
				move = false;
				oldmenuselect = menu.menuselect;
				newPos = new Vector3 (0.0f, (menu.menuselect*-2.5f),-10.0f);
				velocity = Vector3.zero;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class SwitchScriptAlternating : MonoBehaviour {

	public Transform Squad1door;
	public Transform Squad2door;
	[HideInInspector]
	public Vector3 Squad1DoorTargetPosition = Vector3.zero;
	[HideInInspector]
	public Vector3 Squad2DoorTargetPosition = Vector3.zero;
	public float smoothTime = 0.3F;
	public string Squadname = "Squad";
	public float moveDoorUpDistance = 10;
	public Sprite switchLeftSprite;
	public Sprite switchNeutralSprite;
	public Sprite switchRightSprite;

	private Vector3 velocity = Vector3.zero;
	private Vector3 velocity2 = Vector3.zero;
	private bool moveDoor = false;
	private string squadName;
	private Vector3 Squad1DoorStartPosition;
	private Vector3 Squad2DoorStartPosition;
	// Use this for initialization
	void Start () {
		Squad1DoorTargetPosition = Squad1door.transform.position + new Vector3 (0, moveDoorUpDistance, 0);
		Squad1DoorStartPosition = Squad1door.transform.position;
		Squad2DoorTargetPosition = Squad2door.transform.position + new Vector3 (0, moveDoorUpDistance, 0);
		Squad2DoorStartPosition = Squad2door.transform.position;

		if (switchNeutralSprite) {
			transform.GetComponent<SpriteRenderer> ().sprite = switchNeutralSprite;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (moveDoor) {
			if (squadName.Contains("1")) {
				if (Squad1door.transform.position != Squad1DoorTargetPosition){
				// Smoothly move the camera towards that target position
					Squad1door.transform.position = Vector3.SmoothDamp (Squad1door.transform.position, Squad1DoorTargetPosition, ref velocity, smoothTime);
					if (Squad2door.transform.position != Squad2DoorStartPosition) {
						Squad2door.transform.position = Vector3.SmoothDamp (Squad2door.transform.position, Squad2DoorStartPosition, ref velocity2, smoothTime);
					}
				} 
				else {
					moveDoor = false;
				}
			} else if (squadName.Contains("2")) {
				if (Squad2door.transform.position != Squad2DoorTargetPosition){
					// Smoothly move the camera towards that target position
					Squad2door.transform.position = Vector3.SmoothDamp (Squad2door.transform.position, Squad2DoorTargetPosition, ref velocity, smoothTime);
					if (Squad1door.transform.position != Squad1DoorStartPosition) {
						Squad1door.transform.position = Vector3.SmoothDamp (Squad1door.transform.position, Squad1DoorStartPosition, ref velocity2, smoothTime);
					}
				} 
				else {
					moveDoor = false;
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.transform.root.name.Contains(Squadname)) {
			if (!moveDoor) 
				SoundManager.PlaySound(SoundManager.SoundTypes.Grate); // TODO will only play once for each grate

			moveDoor = true;
			squadName = col.transform.root.name;
			if (squadName.Contains("1")) {
				transform.GetComponent<SpriteRenderer>().sprite = switchRightSprite;
			} else {
				transform.GetComponent<SpriteRenderer>().sprite = switchLeftSprite;
			}
			
		}
	}
}

using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;

public class HatSelectionController : MonoBehaviour {
	
	public int controller = 0;
	public bool leftSide = true;

	private System.Array hatArray;
	private System.Array colorArray;
	private int hatindex = 0;
	private int colorindex = 0;
	public float hatSelectionDelay = 0.2f;
	private float lastHatSelection;
	private bool checkedOut = false;
	private List<Color> colors = new List<Color>();
	public GameObject hatPicker;
	private HatPicker hatPickerScript;


	// Use this for initialization
	void Start () {
		hatArray = HatManager.HatNames.GetValues(typeof(HatManager.HatNames));
		colors.Add (Color.black);
		colors.Add (Color.blue);
		colors.Add (new Color(159.0f/255.0f, 200.0f/255.0f, 255.0f/255.0f)); // light blue
		colors.Add (Color.white);
		colors.Add (Color.cyan);
		colors.Add (Color.gray);
		colors.Add (Color.green);
		colors.Add (new Color(255.0f/255.0f, 165.0f/255.0f, 0.0f/255.0f)); // orange
		colors.Add (new Color(135.0f/255.0f, 42.0f/255.0f, 42.0f/255.0f)); // brown
		colors.Add (new Color(255.0f/255.0f, 192.0f/255.0f, 203.0f/255.0f)); // pink
		colors.Add (Color.magenta);
		colors.Add (Color.red);
		colors.Add (Color.yellow);
		Object.DontDestroyOnLoad (gameObject);
		hatPickerScript = GameObject.Find ("HatPicker").GetComponent<HatPicker> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (lastHatSelection + hatSelectionDelay < Time.fixedTime) { // TODO Unnest ifs? (Do whatever makes the code more self-explanatory -Kasra)
			lastHatSelection = Time.fixedTime;
			if (ControllerManager.GetJumpInputBool (controller, leftSide) && !checkedOut) {
			hatindex--;
				if (hatindex < 0) {
					hatindex = hatArray.Length-1;
				}
				GetComponent<EyeAnimator>().ChangeHat((HatManager.HatNames)hatArray.GetValue(hatindex));
				audio.clip = hatPickerScript.hatSwap;
				audio.Play();
		}
			else if (ControllerManager.GetHeavyInputBool (controller, leftSide) && !checkedOut) {
				hatindex++;
				if (hatindex > hatArray.Length-1) {
					hatindex = 0;
				}
				GetComponent<EyeAnimator>().ChangeHat((HatManager.HatNames)hatArray.GetValue(hatindex));
				audio.clip = hatPickerScript.hatSwap;
				audio.Play();
			}
			else if (!Mathf.Approximately(ControllerManager.GetHorizontalInput(controller, leftSide), 0f) && !checkedOut) {
				float dir = ControllerManager.GetHorizontalInput(controller, leftSide);
				int incr = 0;
				if (dir > 0.33f) {
					incr = 1;
				} else if (dir < -0.33f) {
					incr = -1;
				}
				colorindex = colorindex + incr;
				if (colorindex < 0) {
					colorindex = colors.Count-1;
				} else if (colorindex > colors.Count-1) {
					colorindex = 0;
				}
				print(colorindex);
				GetComponent<EyeAnimator>().SetIrisColor(colors[colorindex]);
				audio.clip = hatPickerScript.eyeColorSwap;
				audio.Play();
			}
		else if (ControllerManager.GetStickButtonInput (controller, leftSide)) {
				if (!checkedOut) {
					checkedOut = true;
					foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) {
						mesh.enabled = true;
					}
					hatPickerScript.count += 1;
					audio.clip = hatPickerScript.playerReady;
					audio.Play();
				} else {
					checkedOut = false;
					foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) {
						mesh.enabled = false;
					}
					hatPickerScript.count -= 1;
				}
		}
		}
			
		}
}

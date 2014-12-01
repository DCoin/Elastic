using UnityEngine;
using System.Collections;

public class MusicPlayerScript : MonoBehaviour {


	public AudioClip[] BackgroundMusicClips;

	private int musicindex = 0;

	// Use this for initialization
	void Awake () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Input.GetKeyUp(KeyCode.F9)){
			musicindex++;
			if (musicindex > BackgroundMusicClips.Length-1)
				musicindex = 0;
			audio.clip = BackgroundMusicClips[musicindex];
			audio.Play();
			}
		else if(Input.GetKeyUp(KeyCode.F8)){
			musicindex--;
			if (musicindex < 0)
				musicindex = BackgroundMusicClips.Length-1;
			audio.clip = BackgroundMusicClips[musicindex];
			audio.Play();
		}
	}
}

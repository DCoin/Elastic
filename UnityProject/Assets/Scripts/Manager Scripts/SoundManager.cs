using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SoundManager : MonoBehaviour {

	// One value for every type of sound we have
	public enum SoundTypes
	{

		Squad_Kill,
		Squad_Respawn,

		Player_Land,

		Grate,

		Menu_Back,
		Menu_Error,
		Menu_eyecolor_swap,
		Menu_Hat_swap,
		Menu_Nav,
		Menu_Player_ready,
		Menu_Select,

		Switch1,
		Switch2,
		Switch3,
		Switch4,
		Switch5,
		Switch6,

		Stretch0,
		stretch1,
		stretch2,
		stretch3,
		stretch4,
		stretch5,


	} 

	[System.Serializable] // This makes it show up in the inspector, somehow
	public class Sound
	{
		public AudioClip clip;
		public float volume = 1.0f;
	}

	[System.Serializable]
	public class SoundGroup
	{
		public SoundTypes type;
		public Sound[] sounds;
	}

	public SoundGroup[] soundGroups;

	// Only one soundmanager at a time
	private static SoundManager _instance = null;
	
	private static SoundManager GetInstance() {
		//If _instance hasn't been set yet, we grab it from the scene!
		//This will only happen the first time this reference is used.
		// It will fail if there isn't a soundmanager prefab in the scene.
		// TODO make it add a soundmanager itself perhaps? 
		if(_instance == null) {
			_instance = GameObject.FindObjectOfType<SoundManager>();
			if (!_instance) {
				Debug.LogError("No Sound Manager Prefab in scene");
			}
		}
		return _instance;
	}

	/// <summary>
	/// Gets a random sound from the group and plays it.
	/// </summary>
	/// <param name="t">What type of sound do you want?</param>
	public static void PlaySound(SoundTypes t) {
		var soundmanager = GetInstance();
		if (!soundmanager) {
			Debug.Log("can't play sound: " + t + ", no sound manager present");
			return;
		}

		var sound = soundmanager.GetRandomClipOfGroup(t);
		AudioSource.PlayClipAtPoint(sound.clip, Vector3.zero, sound.volume);
	}

	// TODO comments on these two, possible make them work togehter
	private Sound[] GetAllClipsOfGroup(SoundTypes t) {
		var clips = soundGroups.Single(g => g.type == t).sounds;
		return clips;
	}

	// TODO getting random out of list could be made generic
	private Sound GetRandomClipOfGroup(SoundTypes t) {
		var clips = soundGroups.Single(g => g.type == t).sounds;
		return clips[Random.Range(0,clips.Count())];
	}
}

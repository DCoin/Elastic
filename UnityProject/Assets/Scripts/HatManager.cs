using UnityEngine;
using System.Collections;
using System.Linq;

public class HatManager : MonoBehaviour {

	// One value for every hat we have
	public enum HatNames
	{
		ArmyHat,
		CowboyHat
	}

	[System.Serializable] // This makes it show up in the inspector, somehow
	public class Hat
	{
		public HatNames name;
		public Sprite hatSprite;
	}

	public Hat[] hats;

	// Only one hatmanager at a time
	private static HatManager _instance = null;

	public static HatManager Instance 
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			// It will fail if there isn't a hatmanager prefab in the scene.
			// TODO make it add a hatmanager itself perhaps? 
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<HatManager>();
				if (!_instance) {
					Debug.LogError("No Hat Manager Prefab in scene");
				}
			}
			return _instance;
		}
	}


	/// <summary>
	/// Gets the hat sprite associated with the assigned enum.
	/// </summary>
	/// <returns>The hat sprite.</returns>
	/// <param name="hatName">The HatName Enum.</param>
	public Sprite GetHatSprite(HatNames hatName) {
		// Seleects one (and ONLY ONE) of the sprites to return,
		// Will fail if more sprites have same enum assigned
		var sprite = hats.Single(h => h.name == hatName).hatSprite;
		return sprite;
	}
}

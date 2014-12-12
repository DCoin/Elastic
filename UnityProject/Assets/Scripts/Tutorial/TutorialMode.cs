using UnityEngine;
using System.Collections;

public class TutorialMode : MonoBehaviour {
	
	public GameObject squad1;
	public GameObject squad2;

	public float killRespawnTime = 2.0F;

	private Squad sq1;
	private Squad sq2;

	// Use this for initialization
	void Start () {
		// Get refs to squad scripts
		sq1 = squad1.GetComponent<Squad>() as Squad;
		sq2 = squad2.GetComponent<Squad>() as Squad;
		
		// Set up kill on squads and get the rope colors
		// We assume Squads only have one rope
		var rope = squad1.GetComponentInChildren<RopeCasting> ();
		if (rope != null) {
			rope.killActions += () => sq1.Kill(killRespawnTime);
			//sq1color = rope.ropeMaterial.color;
		}
		rope = squad2.GetComponentInChildren<RopeCasting> ();
		if (rope != null) {
			rope.killActions += () => sq2.Kill(killRespawnTime);
			//sq2color = rope.ropeMaterial.color;
		}
	}
}

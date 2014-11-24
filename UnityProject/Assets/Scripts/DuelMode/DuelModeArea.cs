using UnityEngine;
using System.Collections;

public class DuelModeArea : MonoBehaviour {

	// Spawn points in this area, in local space
	public Vector2 squad1spawnPoint = new Vector2(-10,0);
	public Vector2 squad2spawnPoint = new Vector2(10,0);
	public Vector2 pickupspawnPoint = new Vector2(0,10);

	// Rectangle for camera, to show this area
	public Vector2 camSize = new Vector2(10, 10);

	// Drawing in the scene view, for convenience
	void OnDrawGizmosSelected() {
		Vector2 pos = transform.position;

		// Draw a box in the scene view showing the selected size
		Gizmos.DrawWireCube(pos, camSize); 

		// Draw circles for the three spawn points
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(pos + squad1spawnPoint, 1.0f);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(pos + squad2spawnPoint, 1.0f);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(pos + pickupspawnPoint, 1.0f);
	}

	// TODO: I wonder if there's a way to only make these accesible in code? 
	// Going for the public fields is wrong :<

	/// <summary>
	/// Gets the spawn point for squad 1 in global coordinates.
	/// </summary>
	/// <returns>The spawn point in global coordinates.</returns>
	public Vector2 GetSpawn1InGlobal() {
		Vector2 pos = transform.position;
		return pos + squad1spawnPoint;
	}

	/// <summary>
	/// Gets the spawn point for squad 2 in global coordinates.
	/// </summary>
	/// <returns>The spawn point in global coordinates.</returns>
	public Vector2 GetSpawn2InGlobal() {
		Vector2 pos = transform.position;
		return pos + squad2spawnPoint;
	}

	
	/// <summary>
	/// Gets the spawn point for pickup in global coordinates.
	/// </summary>
	/// <returns>The spawn point in global coordinates.</returns>
	public Vector2 GetPickupInGlobal() {
		Vector2 pos = transform.position;
		return pos + pickupspawnPoint;
	}

	/// <summary>
	/// Gets the area covered by this section as a rectangle, in global coordinates.
	/// </summary>
	/// <returns>Rectangle describing position and size.</returns>
	public Rect GetAreaInGlobal() {
		Vector2 pos = transform.position;
		return new Rect(
			pos.x - (camSize.x / 2),
			pos.y - (camSize.y / 2),
			camSize.x,
			camSize.y);
	}
}

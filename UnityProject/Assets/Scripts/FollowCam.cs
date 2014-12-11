using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class FollowCam : MonoBehaviour {

	// List of objects to keep in focus
	public GameObject[] objects;
	// The minimum amount of units the camera will cover
	public Vector2 minimumDistance = new Vector2(10,10);
	// Extra units around the two objects
	public Vector2 border = new Vector2(1,1);
	// Time for camera to follow
	public float smoothTime = 0.3F;

	private Vector3 velocityPosition;
	private float velocitySize;
	
	// Update is called once per frame
	void Update () {
		// Moving - find midpoint for cam
		//--------

		// If no objects to follow, abort
		if (objects.Length == 0) return;

		// Find min and max for all transforms
		Vector3 first = objects[0].transform.position;

		float minX = first.x;
		float minY = first.y;
		float maxX = first.x;
		float maxY = first.y;

		foreach (var g in objects) {
			if (g != null) {
				minX = Mathf.Min(minX, g.transform.position.x);
				minY = Mathf.Min(minY, g.transform.position.y);
				maxX = Mathf.Max(maxX, g.transform.position.x);
				maxY = Mathf.Max(maxY, g.transform.position.y);
			}
		}

		Vector2 minPoint = new Vector2(minX, minY);
		Vector2 maxPoint = new Vector2(maxX, maxY);

		// Find midpoint between all points
		Vector3 newPos = Vector3.Lerp(minPoint,	maxPoint, 0.5f);

		// Be sure cam keeps distance
		newPos.z = -10;

		// Move cam towards midpoint
		camera.transform.position = Vector3.SmoothDamp(
			camera.transform.position, 
			newPos, 
			ref velocityPosition, 
			smoothTime);


		// Scaling
		//---------

		// Get neccesary camera values
		float aspect = camera.aspect;

		// Find difference vector
		Vector2 diff = maxPoint - minPoint;

		// Find distances between objects for each axis,
		// also counting in the extra border space
		float xDist = Mathf.Abs(diff.x) + (border.x * 2);
		float yDist = Mathf.Abs(diff.y) + (border.y * 2);

		// Compare distances with minimum distances, use whatever larger
		float xMin = Mathf.Max(xDist, minimumDistance.x / 2);
		float yMin = Mathf.Max(yDist, minimumDistance.y / 2);

		// Find needed orthograpgic cam size according to restricting dimension
		float camSize = Mathf.Max(
			(xMin / aspect) / 2,
			yMin / 2);

		// Set camera ortographic camera size to calculated distance
		Camera.main.orthographicSize = Mathf.SmoothDamp(
			Camera.main.orthographicSize, 
			camSize, 
			ref velocitySize, 
			smoothTime);
	}
}

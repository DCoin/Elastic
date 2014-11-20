using UnityEngine;
using System.Collections;

public class MoveCameraScript : MonoBehaviour {

	// Used for smooth moving camera function
	private Vector3 velocityPosition;
	private Vector3 targetPosition;
	private float velocitySize;
	private float targetSize;
	private float smoothTime = 0.3F;
	private bool moving = false;

	void LateUpdate() {
		// Move and scale the camera, if ordered to do so
		if (moving) {
			// Smoothly move the camera towards that target position
			Camera.main.transform.position = Vector3.SmoothDamp (
				Camera.main.transform.position, 
				targetPosition, 
				ref velocityPosition, 
				smoothTime);
			
			// Smoothly scale the camera towards the target size
			Camera.main.orthographicSize = Mathf.SmoothDamp(
				Camera.main.orthographicSize, 
				targetSize, 
				ref velocitySize, 
				smoothTime);

			// Stop moving and scaling when target is met
			if (Camera.main.transform.position == targetPosition &&
			    Camera.main.orthographicSize == targetSize) {
					moving = false;
				}
			}
	}

	/// <summary>
	/// Moves and scales the camera to specified values, over a specified amount of time
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
	/// <param name="targetOrthographicSize">Target orthographic size.</param>
	/// <param name="moveTime">Move time.</param>
	public void MoveTo(Vector2 targetPosition, float targetOrthographicSize, float moveTime) {
		this.targetPosition = new Vector3(targetPosition.x, targetPosition.y, -10); //assuming -10 for proper distance to playing field
		this.targetSize = targetOrthographicSize;
		this.smoothTime = moveTime;

		this.velocityPosition = Vector3.zero;
		this.velocitySize = 0.0f;

		this.moving = true;
	}

	// TODO only considers containing the whole x-axis on screen, maybe adjust for Y as well?
	// TODO proper method comment
	public static float CalculateOrthographicSize(Vector2 bounds) {
		// Get aspect ratio
		var ratio = Camera.main.aspect;

		return (bounds.x / 2) / ratio;
	}
}

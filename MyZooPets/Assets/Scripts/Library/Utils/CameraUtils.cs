using UnityEngine;

/// <summary>
/// Utility functions for the camera.
/// </summary>
public static class CameraUtils{
	/// <summary>
	/// Returns true if the user is touching the pet.
	/// </summary>
	public static bool IsTouchingPet(Camera camera, Vector2 screenPos){
		Ray ray = camera.ScreenPointToRay(screenPos);
		RaycastHit hit;
		int layerMask = 1 << 0;
		bool isOnPet = false;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
			if(hit.collider.name == "Head" || hit.collider.name == "Tummy"){
				isOnPet = true;
			}
		}
		return isOnPet;
	}
	//Returns a random world point at the z position given within the margin (0-1)
	public static Vector3 RandomWorldPointOnScreen(Camera camera, float xMargin, float yMargin, float zPos){
		Vector2 screenPos = new Vector2(Random.Range(Screen.width * xMargin, Screen.width * (1 - xMargin)), Random.Range(Screen.height * yMargin, Screen.height * (1 - yMargin)));
		return ScreenToWorldPointZ(camera, screenPos, zPos);
	}
	//Returns a world position at the z position given, from a Vector2 screen position
	public static Vector3 ScreenToWorldPointZ(Camera camera, Vector2 screenPosition, float zPos){
		Vector3 worldPos = camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -1 * camera.transform.position.z));
		return  worldPos + new Vector3(0, 0, zPos);
	}

	public static float RandomSign(){
		if(Random.Range(0, 2) == 0){
			return -1f;
		}
		return 1f;
	}
}

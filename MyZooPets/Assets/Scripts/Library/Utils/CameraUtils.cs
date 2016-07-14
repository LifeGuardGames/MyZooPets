using UnityEngine;
using System.Collections;

//---------------------------------------------------
// CameraUtils
// Utility functions for the camera.
//---------------------------------------------------

public static class CameraUtils {
	
	//---------------------------------------------------
	// IsTouchingNGUI()
	// Returns true if the user is touching an NGUI element.
	//---------------------------------------------------
    public static bool IsTouchingNGUI(Camera camera, Vector2 screenPos){
        Ray ray = camera.ScreenPointToRay (screenPos);
        RaycastHit hit;
        int layerMask = 1 << 10;
        bool isOnNGUILayer = false;
        // Raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            isOnNGUILayer = true;
        }
        return isOnNGUILayer;
    }

	//---------------------------------------------------
	// IsTouchingNGUI()
	// Returns true if the user is touching the pet.
	//---------------------------------------------------
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
	//Returns a random world point at z = 0 on the screen, within the margin (0-1)
	public static Vector3 RandomWorldPointOnScreen(Camera camera, float xMargin, float yMargin) {
		Vector2 screenPos = new Vector2(Random.Range(Screen.width*xMargin,Screen.width*(1-xMargin)),Random.Range(Screen.height*yMargin,Screen.height*(1-yMargin)));
		return ScreenToWorldPointZero(camera,screenPos);
	}
	//Returns a world position at the same z as the object
	public static Vector3 ScreenToWorldPointZero(Camera camera, Vector2 screenPosition){
		return camera.ScreenToWorldPoint(new Vector3(screenPosition.x,screenPosition.y,-1*camera.transform.position.z));
	}
	public static float RandomSign() {
		if (Random.Range(0, 2) == 0) {
			return -1f;
		}
		return 1f;
	}
}

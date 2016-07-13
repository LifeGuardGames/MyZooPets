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
	public static Vector3 RandomWorldPointOnScreen(Camera camera) {
		Vector3 screenPos = new Vector3(Random.value*Screen.width,Random.value*Screen.height,camera.transform.position.z*-1);
		return camera.ScreenToWorldPoint(screenPos);
	}
}

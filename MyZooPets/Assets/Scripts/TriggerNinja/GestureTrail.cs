using UnityEngine;
using System.Collections;

/// <summary>
/// Gesture trail is in charge of creating, maintaining, and destroying the particle
/// trail taht follows a user's finger.
/// </summary>
public class GestureTrail : MonoBehaviour{	
	public string trailResourceName; // the resource name of the trail
	public float trailZValue; // the Z value the trail should be set to

	private GameObject goTrail; // the actual trail that is created
	private Vector3 lastTouch; // the last position of the trail

	public string GetTrailName(){
		return trailResourceName;
	}

	private float GetZ(){
		return trailZValue;
	}

	private GameObject GetTrail(){
		return goTrail;
	}

	private void SetLastPosition(Vector3 vec){
		lastTouch = vec;
	}

	public Vector3 GetLastPosition(){
		return lastTouch;
	}
	
	private bool CanGesture(){
		return true;	
	}




	/// <summary>
	/// Drags the started.
	/// Function called when the user begins to drag their finger.
	/// </summary>
	/// <param name="position">Position with user touch position in Vector2.</param>
	public void DragStarted(Vector2 position){

		// get the trail resource to create
		string trailName = GetTrailName();
				
		// get the proper vector3 position of the trail based on where the user is touching
		Vector3 screenPos = TranslateScreenPos(position);
		if(goTrail == null){
			goTrail = Instantiate(Resources.Load(trailName) as GameObject, screenPos, Quaternion.identity) as GameObject;
			goTrail.GetComponent<TrailRenderer>().autodestruct = false;
		}
		else{
			goTrail.GetComponent<TrailRenderer>().autodestruct = true;
		}
	}

	/// <summary>
	/// Drag ended.
	/// </summary>
	public void DragEnded(){
		// Let the trail commit seppuku
		if(goTrail != null){
			goTrail.GetComponent<TrailRenderer>().autodestruct = true;
		}
	}


	/// <summary>
	/// As user drags across the screen trail is updated
	/// </summary>
	/// <param name="userTouchPos">User touch position.</param>
	public void DragUpdated(Vector2 userTouchPos){
		// update the trail by setting the trail's position to wherever the user is currently touching the screen
		GameObject goTrail = GetTrail(); 
		if(goTrail){			
			// get the proper vector3 position of the trail based on where the user is touching
			Vector3 pos = TranslateScreenPos(userTouchPos);

			goTrail.transform.position = pos;
			
			SetLastPosition(goTrail.transform.position);
		}
	}

	/// <summary>
	/// Translates the screen position from 2d screen position to 3d position
	/// </summary>
	/// <returns>The world position.</returns>
	/// <param name="screenPos">Screen position.</param>
	private Vector3 TranslateScreenPos(Vector2 screenPos){
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10));
		
		// we are setting the Z manually because the translation from screen to world sets the Z to the camera's Z
		worldPoint.z = GetZ();
		
		return worldPoint;
	}

}
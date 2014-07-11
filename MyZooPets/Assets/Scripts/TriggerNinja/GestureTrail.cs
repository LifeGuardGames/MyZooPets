using UnityEngine;
using System.Collections;

///////////////////////////////////////////
// TrailManager
// The TrailManager is in charge of creating,
// maintaining, and destroying the particle
// trail that follows a user's finger.
///////////////////////////////////////////

public class GestureTrail : MonoBehaviour{	
	// the resource name of the trail
	public string trailResourceName;

	public string GetTrailName(){
		return trailResourceName;
	}
	
	// the Z value the trail should be set to
	public float trailZValue;

	private float GetZ(){
		return trailZValue;
	}
	
	// the actual trail that is created
	private GameObject goTrail;

	private GameObject GetTrail(){
		return goTrail;
	}
	
	// the last position of the trail
	private Vector3 lastTouch;

	private void SetLastPosition(Vector3 vec){
		lastTouch = vec;
	}

	public Vector3 GetLastPosition(){
		return lastTouch;
	}
	
	private bool CanGesture(){
		return true;	
	}
	
	///////////////////////////////////////////
	// DragStarted()
	// Function called when the user begins 
	// to drag their finger.
	// i_vPos is a Vector2 that contains the
	// screen coordinate where the user touched.
	///////////////////////////////////////////		
	public void DragStarted(Vector2 i_vPos){
		// get the trail resource to create
		string strTrail = GetTrailName();
				
		// get the proper vector3 position of the trail based on where the user is touching
		Vector3 vPos = TranslateScreenPos(i_vPos);
		
		goTrail = Instantiate(Resources.Load(strTrail) as GameObject, vPos, Quaternion.identity) as GameObject;
	}
	
	///////////////////////////////////////////
	// DragEnded()
	// When the drag ends, this function is
	// called.
	// Unfortunately I need to make this function
	// a bit more robust than originally intended,
	// because there is no FingerGestures feedback
	// for an "failed" PointCloud gesture -- just
	// a DragEnded event.  So this function has
	// to just take raw data (the result) and
	// do the play the proper color/sound based
	// on that result.
	///////////////////////////////////////////
	public void DragEnded(){
		StartCoroutine( OnDragEnded() );
	}

	// Linger the trail for some time
	private IEnumerator OnDragEnded() {
		Debug.Log("Drag ended");
		GameObject goTrail = GetTrail();
		
		if ( goTrail ) {
			float fLinger = Constants.GetConstant<float>( "Ninja_TrailLinger" );
			yield return new WaitForSeconds( fLinger );
			
			Destroy( goTrail );
		}		
	}
	
	///////////////////////////////////////////
	// DragUpdated()
	// As the user drags across the screen,
	// this function is called.
	// i_vPos is a Vector2 that contains the
	// screen coordinate where the user touched.	
	///////////////////////////////////////////		
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
	
	///////////////////////////////////////////
	// TranslateScreenPos()
	// Simple function that takes the 2d screen
	// position of a drag and turns it into
	// a 3d position for the trail.	
	///////////////////////////////////////////		
	private Vector3 TranslateScreenPos(Vector2 screenPos){
		Vector3 vPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10));
		
		// we are setting the Z manually because the translation from screen to world sets the Z to the camera's Z
		vPos.z = GetZ();
		
		return vPos;
	}

}
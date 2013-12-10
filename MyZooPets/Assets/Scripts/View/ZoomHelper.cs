using UnityEngine;
using System.Collections;

//---------------------------------------------------
// ZoomHelper
// Small class designed to make it easier for an
// object to be zoomed in on.
//---------------------------------------------------

public class ZoomHelper : MonoBehaviour {
	// zoom variables
	public float fZoomTime;
	public Vector3 vOffset;
	public Vector3 vRotation;
	
	//---------------------------------------------------
	// Zoom()
	//---------------------------------------------------	
	public void Zoom() {
		Vector3 vPos = gameObject.transform.position + vOffset;
		CameraManager.Instance.ZoomToTarget( vPos, vRotation, fZoomTime, null );		
	}
}

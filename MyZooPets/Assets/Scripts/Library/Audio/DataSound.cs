using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DataSound
// Individual piece of sound data as loaded from xml.
// Considered to be immutable.
//---------------------------------------------------

public class DataSound {

	// id of the sound (must match sound in the resources folder)
	private string strID;
	public string GetResourceName() {
		return strID;	
	}
	
	// volume of the sound
	private float fVolume = 0.5f;
	public float GetVolume() {
		return fVolume;	
	}
	
	// pitch of the sound
	private float fPitch = 1.0f;
	public float GetPitch() {
		return fPitch;	
	}
	
	public DataSound( string id ) {
		strID = id;	
	}
	
	public DataSound( string id, Hashtable hashData ) {
		strID = id;	

		// get volume if it exists
		if ( hashData.Contains("Volume") ) 
			fVolume = float.Parse((string)hashData["Volume"]);	
		
		// get the pitch if it exists
		if ( hashData.Contains("Pitch") )
			fPitch = float.Parse((string)hashData["Pitch"]);	 
	}
}

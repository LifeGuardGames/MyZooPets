using UnityEngine;
using System.Collections;

public class DataSound {

	// id of the sound (must match sound in the resources folder)
	private string strID;
	public string GetResourceName() {
		return strID;	
	}
	
	// volume of the sound
	private float fVolume = 1.0f;
	public float GetVolume() {
		return fVolume;	
	}
	
	public DataSound( string id ) {
		strID = id;	
	}
	
	public DataSound( string id, Hashtable hashData ) {
		strID = id;	

		// get volume if it exists
		if ( hashData.Contains("Volume" ) ) 
			fVolume = float.Parse((string)hashData["Volume"]);		
	}
}

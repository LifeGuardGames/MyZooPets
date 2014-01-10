using UnityEngine;
using System.Collections;

//---------------------------------------------------
// HashUtils
// Utility functions for hashtables.
//---------------------------------------------------
public static class HashUtils  {
	
	//---------------------------------------------------
	// GetHashValue()
	//---------------------------------------------------
    public static T GetHashValue<T>( Hashtable hash, string strKey, T defaultVal, string strError = null  )
    {
		T val = defaultVal;
		if ( hash.Contains(strKey) ) 
			val = (T)hash[strKey];
		else if ( !string.IsNullOrEmpty(strError) )
			Debug.LogError(strError + "Can't find key " + strKey);
		
		return val;
    }
}
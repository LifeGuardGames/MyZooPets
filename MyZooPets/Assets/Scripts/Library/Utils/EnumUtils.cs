using UnityEngine;
using System.Collections;

//---------------------------------------------------
// EnumUtils
// Utility functions for enums.
//---------------------------------------------------
public static class EnumUtils  {
	
	//---------------------------------------------------
	// GetRandomEnum()
	// Returns a random value in an enum.
	//---------------------------------------------------
    public static T GetRandomEnum<T>()
    {
	    System.Array array = System.Enum.GetValues(typeof(T));
	    T eVal = (T)array.GetValue(UnityEngine.Random.Range(0,array.Length));
	    return eVal;
    }
}

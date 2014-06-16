using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Utility functions for hashtables
/// </summary>
public static class HashUtils{

	/// <summary>
	/// Gets the hash value.
	/// </summary>
	/// <returns>The hash value.</returns>
	/// <param name="hash">Hash.</param>
	/// <param name="key">Key.</param>
	/// <param name="defaultVal">Default value.</param>
	/// <param name="strError">String error.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T GetHashValue<T>(Hashtable hash, string key, T defaultVal, string strError = null){
		T val = defaultVal;

		try{
			if(hash.Contains(key)) 
				val = (T)hash[key];
			else if(!string.IsNullOrEmpty(strError))
				Debug.LogError(strError + "Can't find key " + key);
		}
		catch(ArgumentNullException e){
			Debug.LogException(e);
		}

		return val;
	}
}
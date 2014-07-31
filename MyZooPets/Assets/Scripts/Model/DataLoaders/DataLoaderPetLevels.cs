using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data loader pet levels.
/// Hashtable -- Key: LevelID, Value: ImmutableDataPetLevel
/// </summary>
public class DataLoaderPetLevels : XMLLoaderGeneric<DataLoaderPetLevels>{

	/// <summary>
	/// Gets the level.
	/// </summary>
	/// <returns>The level.</returns>
	/// <param name="id">Identifier.</param>
    public static ImmutableDataPetLevel GetLevel(Level id){
		instance.InitXMLLoader();
		
		return instance.GetData<ImmutableDataPetLevel>(id.ToString());
	
    }
	
	public static ImmutableDataPetLevel GetLevel(int nLevel) {
		ImmutableDataPetLevel petLevel = null;
		
		Level level = (Level) nLevel;
		
		petLevel = GetLevel(level);
		
		return petLevel;
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataPetLevel data = new ImmutableDataPetLevel(id, xmlNode, errorMessage);

		// store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data); 
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "PetLevels";
	}
}

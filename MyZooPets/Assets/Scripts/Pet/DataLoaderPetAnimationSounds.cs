using UnityEngine;
using System.Collections;

public class DataLoaderPetAnimationSounds : XMLLoaderGeneric<DataLoaderPetAnimationSounds> {
	/// <summary>
	/// Gets the data.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="id">Identifier.</param>
	public static ImmutableDataPetAnimationSound GetData(string id){
		instance.InitXMLLoader();
		
		return instance.GetData<ImmutableDataPetAnimationSound>(id);
	}
	
	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataPetAnimationSound data = new ImmutableDataPetAnimationSound(id, xmlNode, errorMessage);
		
		// store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data); 
	}
	
	protected override void InitXMLLoader(){
		xmlFileFolderPath = "PetAnimationSounds";
	}
}

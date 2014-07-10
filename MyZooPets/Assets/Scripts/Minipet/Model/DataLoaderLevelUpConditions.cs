using UnityEngine;
using System.Collections;

/// <summary>
/// Data loader level up conditions.
/// Hash -- Key: LevelUpConditionID, Value: ImmutableDataMiniPetLevelUpConditions
/// </summary>
public class DataLoaderLevelUpConditions : XMLLoaderGeneric<DataLoaderLevelUpConditions>{

	/// <summary>
	/// Gets the data.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="id">Identifier.</param>
	public static ImmutableDataMiniPetLevelUpConditions GetData(string id){
		instance.InitXMLLoader();
		
		return instance.GetData<ImmutableDataMiniPetLevelUpConditions>(id);
	}
	
	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataMiniPetLevelUpConditions data = new ImmutableDataMiniPetLevelUpConditions(id, xmlNode, errorMessage); 
		
		// store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data); 
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "MiniPetLevels";
	}
}

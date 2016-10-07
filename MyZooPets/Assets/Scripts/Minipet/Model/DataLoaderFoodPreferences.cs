using UnityEngine;
using System.Collections;

public class DataLoaderFoodPreferences : XMLLoaderGeneric<DataLoaderFoodPreferences>{


	/// <summary>
	/// Gets the data.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="id">Identifier.</param>
	public static ImmutableDataFoodPreferences GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataFoodPreferences>(id);
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataFoodPreferences data = new ImmutableDataFoodPreferences(id, xmlNode, errorMessage);

		// store the data
		if(hashData.ContainsKey(id)) {
			Debug.LogError(errorMessage + "Duplicate keys!");
		}
		else {
			hashData.Add(id, data);
		}
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "MiniPetFoodPreferences";
	}
}

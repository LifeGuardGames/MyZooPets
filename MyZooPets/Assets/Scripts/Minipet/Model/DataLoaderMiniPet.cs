using UnityEngine;
using System.Collections;

/// <summary>
/// Data loader mini pet.
/// Hash -- Key: MiniPetID, Value: ImmutableDataMiniPet
/// </summary>
public class DataLoaderMiniPet : XMLLoaderGeneric<DataLoaderMiniPet>{

	/// <summary>
	/// Gets the data.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="id">Identifier.</param>
    public static ImmutableDataMiniPet GetData(string id){
		instance.InitXMLLoader();

		return instance.GetData<ImmutableDataMiniPet>(id);
    }

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataMiniPet data = new ImmutableDataMiniPet(id, xmlNode, errorMessage); 
		            
        // store the data
        if(hashData.ContainsKey(id))
            Debug.LogError(errorMessage + "Duplicate keys!");
        else
            hashData.Add(id, data); 
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "MiniPets";
	}
}


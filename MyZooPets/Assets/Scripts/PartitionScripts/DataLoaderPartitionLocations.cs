using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderPartitionLocations:XMLLoaderGeneric<DataLoaderPartitionLocations> {

	public static ImmutableDataPartitionLocation GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataPartitionLocation>(id);
	}
	
	public static List<ImmutableDataPartitionLocation> GetDataList(){
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataPartitionLocation>();
	}
	
	#region Loading Functions
	protected override void InitXMLLoader(){
		xmlFileFolderPath = "PartitionLocations";
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataPartitionLocation data = new ImmutableDataPartitionLocation(id, xmlNode, errorMessage); 
		// store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data); 
	}
	#endregion
}

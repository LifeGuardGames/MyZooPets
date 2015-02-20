using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderPartitions:XMLLoaderGeneric<DataLoaderPartitions> {
	public static ImmutableDataPartition GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataPartition>(id);
	}
	
	public static List<ImmutableDataPartition> GetDataList(){
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataPartition>();
	}
	
	#region Loading Functions
	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Partitions";
	}
	
	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataPartition data = new ImmutableDataPartition(id, xmlNode, errorMessage); 
		// store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data); 
	}
	#endregion
}

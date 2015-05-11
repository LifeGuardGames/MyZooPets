using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderMemoryTrigger : XMLLoaderGeneric<DataLoaderMemoryTrigger> {

	public static ImmutableDataMemoryTrigger GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataMemoryTrigger>(id);
	}

	public static List<ImmutableDataMemoryTrigger> GetDataList(){
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataMemoryTrigger>();
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataMemoryTrigger data = new ImmutableDataMemoryTrigger(id, xmlNode, errorMessage);

		// Store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data);
	}

	protected override void InitXMLLoader(){
		xmlFileFolderPath = "MemoryGame";
	}
}

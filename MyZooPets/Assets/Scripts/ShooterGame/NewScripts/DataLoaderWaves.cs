using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderWaves: XMLLoaderGeneric<DataLoaderWaves> {

	public static ImmutableDataWaves GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataWaves>(id);
	}

	public static List<ImmutableDataWaves> GetDataList(){
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataWaves>();
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataWaves data = new ImmutableDataWaves(id,xmlNode,errorMessage);
		// Store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data);
	}
	protected override void InitXMLLoader(){
		xmlFileFolderPath = "Shooter/Waves";
	}
}

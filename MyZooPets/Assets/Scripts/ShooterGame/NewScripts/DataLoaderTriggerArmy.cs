using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderTriggerArmy : XMLLoaderGeneric<DataLoaderTriggerArmy> {

	public static ImmutableDataTriggerArmy GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataTriggerArmy>(id);
	}

	public static List<ImmutableDataTriggerArmy>GetDataList(){
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataTriggerArmy>();
	}
	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataTriggerArmy data = new ImmutableDataTriggerArmy(id,xmlNode,errorMessage);
		// Store the data
		if(hashData.ContainsKey(id))
			Debug.LogError(errorMessage + "Duplicate keys!");
		else
			hashData.Add(id, data);
	}
	protected override void InitXMLLoader(){
		xmlFileFolderPath = "TriggerShooter";
	}
}

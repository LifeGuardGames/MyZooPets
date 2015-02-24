using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderTriggerArmy : XMLLoaderGeneric<DataLoaderTriggerArmy> {

	public static ImmutableDataTriggerArmy GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataTriggerArmy>(id);
	}

	public static ImmutableDataTriggerArmy GetEnemy(int difficulty){
		
		switch(difficulty){
		case 0:
			return GetData("Mober_0");
		case 1:
			return GetData("Mober_1");
		case 2:
			return GetData("Mober_2");
		case 3:
			return GetData( "Mober_3");
		case 4:
			return GetData( "Mober_4");
		default:
			return GetData("Mober_0");
		}
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
		xmlFileFolderPath = "Shooter/TriggerShooter";
	}
}

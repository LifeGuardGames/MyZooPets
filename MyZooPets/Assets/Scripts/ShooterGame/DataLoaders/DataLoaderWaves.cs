using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderWaves: XMLLoaderGeneric<DataLoaderWaves>{
	public static ImmutableDataWave GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataWave>(id);
	}
	
	public static ImmutableDataWave GetWave(int difficulty){
		switch(difficulty){
		case 0:
			return GetData("Tutorial Wave_1");
		case 1:
			return GetData("Starting Wave_" + Random.Range(1, 4).ToString());
		case 2:
			return GetData("Medium Wave_" + Random.Range(1, 4).ToString());
		case 3:
			return GetData("Hard Wave_" + Random.Range(1, 4).ToString());
		default:
			return GetData("Starting Wave_" + Random.Range(1, 4).ToString());
		}
	}

	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataWave data = new ImmutableDataWave(id, xmlNode, errorMessage);
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

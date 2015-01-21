using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataLoaderWaves: XMLLoaderGeneric<DataLoaderWaves> {
	public static ImmutableDataWave GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataWave>(id);
	}

	public static List<ImmutableDataWave> GetDataList(){
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataWave>();
	}
	public static ImmutableDataWave GetWave(int difficulty){

		switch(difficulty){
		case 1:
			return GetData( "Starting Wave_"+Random.Range (1,4).ToString());
			break;
		case 2:
			return GetData("Medium Wave_"+Random.Range (1,4).ToString());
			break;
		case 3:
			return GetData( "Hard Wave_"+Random.Range (1,4).ToString());
			break;
		default:
				return GetData( "Starting Wave_"+Random.Range (1,4).ToString());
				break;

		}

	}
	protected override void XMLNodeHandler(string id, IXMLNode xmlNode, Hashtable hashData, string errorMessage){
		ImmutableDataWave data = new ImmutableDataWave(id,xmlNode,errorMessage);
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

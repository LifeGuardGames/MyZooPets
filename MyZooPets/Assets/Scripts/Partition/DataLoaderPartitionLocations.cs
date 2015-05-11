using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataLoaderPartitionLocations:XMLLoaderGeneric<DataLoaderPartitionLocations> {

	public static ImmutableDataPartitionLocation GetData(string id){
		instance.InitXMLLoader();
		return instance.GetData<ImmutableDataPartitionLocation>(id);
	}
	
	public static List<ImmutableDataPartitionLocation> GetDataList(){
		instance.InitXMLLoader();
		return instance.GetDataList<ImmutableDataPartitionLocation>();
	}

	public static int GetAbsolutePartitionNumberFromLocationId(string locationId){
		return GetData(locationId).AbsolutePartition;
	}

	public static Vector3 GetOffsetFromLocationId(string locationId){
		return GetData(locationId).Offset;
	}

	public static MinigameTypes GetMinigameTypeFromLocationId(string locationId){
		PartitionLocationTypes partitionLocType = GetData(locationId).Attribute;
		if(partitionLocType == PartitionLocationTypes.Base){
			// Not supported for parsing
			return MinigameTypes.None;
		}
		else{
			// Parse convert string to another enum
			MinigameTypes minigameType = (MinigameTypes)Enum.Parse(typeof(MinigameTypes), partitionLocType.ToString());
			return minigameType;
		}
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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Partition manager takes care of giving the transforms of the particular partition
/// when something needs to be procedurally spawned on game start (ie. minipets)
/// Each partition will have 2 zones max that is available to the user
/// </summary>
public class PartitionManager : Singleton<PartitionManager> {

	public List<ImmutableDataPartitionLocation> partitionLocationsList;

	void Start(){
		Initialize();
	}

	private void Initialize(){
		ZoneTypes zone = SceneUtils.GetZoneTypeFromSceneName(Application.loadedLevelName);
		if(zone == ZoneTypes.None){
			Debug.LogError("Undetected zone type " + Application.loadedLevelName);
		}

		foreach(ImmutableDataPartitionLocation data in DataLoaderPartitionLocations.GetDataList()){
			if(data.Zone == zone){	// We only care about zones that are in the current scene
//				Tuple<Vector3, Vector3> positionPair = new Tuple<Vector3, Vector3>();
			}
		}
	}

	public Vector3 GetUnusedPositionNextToMinigame(MinigameTypes minigameType){
		return Vector3.zero;
	}

	/// <summary>
	/// Gets the unused position in partition.
	/// </summary>
	/// <returns>Used position in partition, zero if none exists</returns>
	/// <param name="partitionNumber">Partition number.</param>
	public Vector3 GetUnusedPositionInScene(bool isExcludeBasePartition = true){
		return Vector3.zero;
	}
}

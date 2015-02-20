using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Partition manager takes care of giving the transforms of the particular partition
/// when something needs to be procedurally spawned on game start (ie. minipets)
/// Each partition will have 2 zones max that is available to the user
/// </summary>
public class PartitionManager : Singleton<PartitionManager> {

	public List<ImmutableDataPartitionLocation> availablePartitionLocationsList;

	void Start(){
		Initialize();
	}

	// TODO need to take care of once per pp data handling
	private void Initialize(){
		availablePartitionLocationsList = DataLoaderPartitionLocations.GetDataList();
	}

	#region Minipet spawning use
	public MinigameTypes GetRandomUnlockedMinigameType(){
		return MinigameTypes.None;
	}

	public LgTuple<Vector3, int> GetBasePositionInBedroom(){
		// Initialize all to null first
		LgTuple<Vector3, int> tupleToReturn = null;
		ImmutableDataPartitionLocation locationToDelete = null;

		// Loop through available list and keep track if found
		foreach(ImmutableDataPartitionLocation location in availablePartitionLocationsList){
			if(location.Attribute == PartitionLocationTypes.Base){
				tupleToReturn = new LgTuple<Vector3, int>(location.Offset, location.Partition);	// TODO turn offset into actual position
				locationToDelete = location;
				break;
			}
		}
		// Remove the tuple from the list if is exists, outside foreach iteration
		if(locationToDelete != null){
			availablePartitionLocationsList.Remove(locationToDelete);
		}
		return tupleToReturn;
	}

	public LgTuple<Vector3, int> GetUnusedPositionNextToMinigame(MinigameTypes minigameType){
		// Converting to the MinigameTypes to a PartitionLocationType
		PartitionLocationTypes locationType = PartitionLocationTypes.None;
		switch(minigameType){
		case MinigameTypes.Clinic:
			locationType = PartitionLocationTypes.Clinic;
			break;
		case MinigameTypes.Memory:
			locationType = PartitionLocationTypes.Memory;
			break;
		case MinigameTypes.Runner:
			locationType = PartitionLocationTypes.Runner;
			break;
		case MinigameTypes.Shooter:
			locationType = PartitionLocationTypes.Shooter;
			break;
		case MinigameTypes.TriggerNinja:
			locationType = PartitionLocationTypes.TriggerNinja;
			break;
		default:
			break;
		}

		// Initialize all to null first
		LgTuple<Vector3, int> tupleToReturn = null;
		ImmutableDataPartitionLocation locationToDelete = null;

		// Loop through available list and keep track if found
		foreach(ImmutableDataPartitionLocation location in availablePartitionLocationsList){
			if(location.Attribute == locationType){
				tupleToReturn = new LgTuple<Vector3, int>(location.Offset, location.Partition);	// TODO turn offset into actual position
				locationToDelete = location;
				break;
			}
		}
		// Remove the tuple from the list if is exists, outside foreach iteration
		if(locationToDelete != null){
			availablePartitionLocationsList.Remove(locationToDelete);
		}
		return tupleToReturn;
	}

	/// <summary>
	/// Gets a random unused position in partition from availablePartitionLocationList
	/// </summary>
	/// <returns>Used position in partition, zero if none exists</returns>
	/// <param name="partitionNumber">Partition number.</param>
	public LgTuple<Vector3, int> GetRandomUnusedPosition(){
		// Initialize all to null first
		LgTuple<Vector3, int> tupleToReturn = null;
		ImmutableDataPartitionLocation locationToDelete = null;

		// Choose a random index and designate it
		int randomIndex = UnityEngine.Random.Range(0, availablePartitionLocationsList.Count);	// TODO Test this out!!
		locationToDelete = availablePartitionLocationsList[randomIndex];
		tupleToReturn = new LgTuple<Vector3, int>(locationToDelete.Offset, locationToDelete.Partition);

		// Remove the tuple from the list if is exists, outside foreach iteration
		if(locationToDelete != null){
			availablePartitionLocationsList.Remove(locationToDelete);
		}
		return tupleToReturn;
	}
	#endregion

	private int GetLastestUnlockedPartition(){
		return 0;
	}

	public bool IsPartitionInCurrentZone(int partitionNumber){
		string craftedId = "Partition" + StringUtils.FormatIntToDoubleDigitString(partitionNumber);
		ImmutableDataPartition partition = DataLoaderPartitions.GetData(craftedId);
		return (partition.Zone == SceneUtils.GetZoneTypeFromSceneName(Application.loadedLevelName)) ? true : false;
	}
}

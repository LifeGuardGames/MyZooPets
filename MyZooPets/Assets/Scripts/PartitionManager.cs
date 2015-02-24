using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Partition manager takes care of giving the transforms of the particular partition
/// when something needs to be procedurally spawned on game start (ie. minipets)
/// Each partition will have 2 zones max that is available to the user
/// 
/// NOTE: Partition manager doesnt save any data when spawning minipets, the minipet
/// locations will be saved from MinipetManager itself
/// </summary>
public class PartitionManager : Singleton<PartitionManager> {
	public Transform partitionParent;

	private Dictionary<int, Transform> partitionInteractableDictionary;
	private List<ImmutableDataPartitionLocation> openMinipetLocationsList;

	private void Initialize(){
		// Populate the partition interactable parent dictionary, only for ones existing
		foreach(Transform trans in partitionParent){
			PartitionMetadata metadata = trans.GetComponent<PartitionMetadata>();
			if(metadata != null){
				partitionInteractableDictionary.Add(metadata.partitionNumber, metadata.interactables);
			}
			else{
				Debug.LogError("Non partition detected " + trans.name);
			}
		}
		openMinipetLocationsList = DataLoaderPartitionLocations.GetDataList();
	}

	/// <summary>
	/// Gets the interactable parent transform based on what partition number is input
	/// </summary>
	/// <returns>Parent transform for interactables</returns>
	/// <param name="partitionNumber">Partition number</param>
	public Transform GetInteractableParent(int partitionNumber){
		if(partitionInteractableDictionary.ContainsKey(partitionNumber)){
			return partitionInteractableDictionary[partitionNumber];
		}
		else{
			Debug.LogError("Partition " + partitionNumber + " does not exist, double check your zone!");
			return null;
		}
	}

	#region Minipet spawning use
	public LgTuple<Vector3, int> GetBasePositionInBedroom(){
		// Initialize all to null first
		LgTuple<Vector3, int> tupleToReturn = null;
		ImmutableDataPartitionLocation locationToDelete = null;
		
		// Loop through available list and keep track if found
		foreach(ImmutableDataPartitionLocation location in openMinipetLocationsList){
			if(location.Attribute == PartitionLocationTypes.Base){
				tupleToReturn = new LgTuple<Vector3, int>(location.Offset, location.Partition);	// TODO turn offset into actual position
				locationToDelete = location;
				break;
			}
		}
		// Remove the tuple from the list if is exists, outside foreach iteration
		if(locationToDelete != null){
			openMinipetLocationsList.Remove(locationToDelete);
		}
		return tupleToReturn;
	}

	public MinigameTypes GetRandomUnlockedMinigameType(){
		int lastestPartition = GetLatestUnlockedPartition();
		List<MinigameTypes> minigameAux = new List<MinigameTypes>();
		foreach(ImmutableDataPartition partitionData in DataLoaderPartitions.GetDataList()){
			if(partitionData.Number <= lastestPartition && partitionData.MinigameList != null){
				Debug.Log("    partition num " + partitionData.Number + " " + lastestPartition);
				foreach(MinigameTypes minigameType in partitionData.MinigameList){
					Debug.Log("    adding " + minigameType.ToString());
					minigameAux.Add(minigameType);
				}
			}
		}
		if(minigameAux.Count == 0){
			return MinigameTypes.None;
		}
		else{
			int randomIndex = UnityEngine.Random.Range(0, minigameAux.Count);
			Debug.Log("    random index " + randomIndex);
			return minigameAux[randomIndex];
		}
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
		foreach(ImmutableDataPartitionLocation location in openMinipetLocationsList){
			if(location.Attribute == locationType){
				tupleToReturn = new LgTuple<Vector3, int>(location.Offset, location.Partition);	// TODO turn offset into actual position
				locationToDelete = location;
				break;
			}
		}
		// Remove the tuple from the list if is exists, outside foreach iteration
		if(locationToDelete != null){
			openMinipetLocationsList.Remove(locationToDelete);
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
		int randomIndex = UnityEngine.Random.Range(0, openMinipetLocationsList.Count);	// TODO Test this out!!
		locationToDelete = openMinipetLocationsList[randomIndex];
		tupleToReturn = new LgTuple<Vector3, int>(locationToDelete.Offset, locationToDelete.Partition);

		// Remove the tuple from the list if is exists, outside foreach iteration
		if(locationToDelete != null){
			openMinipetLocationsList.Remove(locationToDelete);
		}
		return tupleToReturn;
	}
	#endregion

	private int GetLatestUnlockedPartition(){
		ImmutableDataGate latestGate = GatingManager.Instance.GetLatestLockedGate();
		if(latestGate != null){	// All gates unlocked
			return latestGate.Partition - 1;	// Get latest gate and subtract 1
		}
		else{
			return DataLoaderPartitions.GetDataList().Count - 1;	// Get the gates list count, off by 1
		}
	}

	public bool IsPartitionInCurrentZone(int partitionNumber){
		string craftedId = "Partition" + StringUtils.FormatIntToDoubleDigitString(partitionNumber);
		ImmutableDataPartition partition = DataLoaderPartitions.GetData(craftedId);
		return (partition.Zone == SceneUtils.GetZoneTypeFromSceneName(Application.loadedLevelName)) ? true : false;
	}

	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 100, 100), "random minigame")){
			Debug.Log(GetRandomUnlockedMinigameType().ToString());
		}
		if(GUI.Button(new Rect(200, 100, 100, 100), "latest unlocked p")){
			Debug.Log(GetLatestUnlockedPartition());
		}
	}
}

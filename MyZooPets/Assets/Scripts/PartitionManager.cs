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
	public List<Transform> petWanderPoints;
	private Dictionary<int, Transform> partitionInteractableDictionary = new Dictionary<int, Transform>();
	private List<ImmutableDataPartitionLocation> openMinipetLocationsList;
	private bool isOpenLocationsInitalized = false;
	private bool isMinigameRandomCallLocked = false;
	public PanToMoveCamera cameraGO;
	private int lastKnownWanderPoint;

	public void Awake(){
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
	}

	private void CheckInitializeOpenLocations(){
		// Initialize it if havent yet
		if(!isOpenLocationsInitalized){
			openMinipetLocationsList = new List<ImmutableDataPartitionLocation>();
			int latestAbsolutePartition = GetLatestUnlockedAbsolutePartition();
			foreach(ImmutableDataPartitionLocation location in DataLoaderPartitionLocations.GetDataList()){
				if(location.AbsolutePartition <= latestAbsolutePartition){
					openMinipetLocationsList.Add(location);
				}
			}
			isOpenLocationsInitalized = true;
		}
	}

	/// <summary>
	/// Gets the interactable parent transform based on what partition number is input
	/// </summary>
	/// <returns>Parent transform for interactables</returns>
	/// <param name="partitionNumber">Partition number</param>
	public Transform GetInteractableParent(int absolutePartitionNumber){
		if(partitionInteractableDictionary.ContainsKey(absolutePartitionNumber)){
			return partitionInteractableDictionary[absolutePartitionNumber];
		}
		else{
			Debug.LogError("Partition " + absolutePartitionNumber + " does not exist, double check your zone!");
			return null;
		}
	}

	#region Minipet spawning use
	public LgTuple<Vector3, string> GetBasePositionInBedroom(){
		CheckInitializeOpenLocations();

		// Initialize all to null first
		LgTuple<Vector3, string> tupleToReturn = null;
		ImmutableDataPartitionLocation locationToDelete = null;
		
		// Loop through available list and keep track if found
		foreach(ImmutableDataPartitionLocation location in openMinipetLocationsList){
			if(location.Attribute == PartitionLocationTypes.Base){
				tupleToReturn = new LgTuple<Vector3, string>(location.Offset, location.Id);
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
	/// Gets the random type of the unlocked minigame
	/// NOTE: Should only get called ONCE, it can repeat if called again, leading to error
	/// </summary>
	/// <returns>The random unlocked minigame type.</returns>
	public MinigameTypes GetRandomUnlockedMinigameType(){
		if(!isMinigameRandomCallLocked){
			isMinigameRandomCallLocked = true;	// Lock the function call so its not called again
			int latestAbsolutePartition = GetLatestUnlockedAbsolutePartition();
			List<MinigameTypes> minigameAux = new List<MinigameTypes>();
			foreach(ImmutableDataPartition partitionData in DataLoaderPartitions.GetDataList()){
				if(partitionData.Number <= latestAbsolutePartition && partitionData.MinigameList != null){
					foreach(MinigameTypes minigameType in partitionData.MinigameList){
						minigameAux.Add(minigameType);
					}
				}
			}
			if(minigameAux.Count == 0){
				return MinigameTypes.None;
			}
			else{
				int randomIndex = UnityEngine.Random.Range(0, minigameAux.Count);
				return minigameAux[randomIndex];
			}
		}
		else{
			Debug.LogError("Illegal second call for minigame");
			return MinigameTypes.None;
		}
	}

	/// <summary>
	/// Gets the unused position next to minigame
	/// NOTE: GetRandomUnlockedMinigameType() does not keep track of local, if picked already
	/// </summary>
	/// <returns>The unused position next to minigame.</returns>
	public LgTuple<Vector3, string> GetPositionNextToMinigame(MinigameTypes minigameType){
		CheckInitializeOpenLocations();

		// Converting to the MinigameTypes to a PartitionLocationType
		PartitionLocationTypes locationType = PartitionLocationTypes.None;
		switch(minigameType){
		case MinigameTypes.Clinic:
			locationType = PartitionLocationTypes.Clinic;
			break;
		case MinigameTypes.Memory:
			locationType = PartitionLocationTypes.Memory;
			break;
		case MinigameTypes.RUNNER:
			locationType = PartitionLocationTypes.Runner;
			break;
		case MinigameTypes.Shooter:
			locationType = PartitionLocationTypes.Shooter;
			break;
		case MinigameTypes.TriggerNinja:
			locationType = PartitionLocationTypes.TriggerNinja;
			break;
		default:
			return null;
		}

		// Initialize all to null first
		LgTuple<Vector3, string> tupleToReturn = null;
		ImmutableDataPartitionLocation locationToDelete = null;

		// Loop through available list and keep track if found
		foreach(ImmutableDataPartitionLocation location in openMinipetLocationsList){
			if(location.Attribute == locationType){
				tupleToReturn = new LgTuple<Vector3, string>(location.Offset, location.Id);
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
	public LgTuple<Vector3, string> GetRandomUnusedPosition(){
		CheckInitializeOpenLocations();

		// Initialize all to null first
		LgTuple<Vector3, string> tupleToReturn = null;
		ImmutableDataPartitionLocation locationToDelete = null;
		
		if(openMinipetLocationsList.Count > 0){
			// Choose a random index and designate it
			int randomIndex = UnityEngine.Random.Range(0, openMinipetLocationsList.Count);
			locationToDelete = openMinipetLocationsList[randomIndex];
			tupleToReturn = new LgTuple<Vector3, string>(locationToDelete.Offset, locationToDelete.Id);

			// Remove the tuple from the list if is exists, outside foreach iteration
			if(locationToDelete != null){
				openMinipetLocationsList.Remove(locationToDelete);
			}
		}
		return tupleToReturn;
	}
	#endregion

	private int GetLatestUnlockedAbsolutePartition(){
		ImmutableDataGate latestGate = GatingManager.Instance.GetLatestLockedGate();
		if(latestGate != null){	// All gates unlocked
			return latestGate.AbsolutePartition - 1;	// Get latest gate and subtract 1
		}
		else{
			// All gates unlocked
			return DataLoaderPartitions.GetDataList().Count - 1;	// Get the gates list count, off by 1
		}
	}

	public bool IsPartitionInCurrentZone(int absolutePartitionNumber){
		string craftedId = "Partition" + StringUtils.FormatIntToDoubleDigitString(absolutePartitionNumber);
		ImmutableDataPartition partition = DataLoaderPartitions.GetData(craftedId);
		return (partition.Zone == SceneUtils.GetZoneTypeFromSceneName(SceneUtils.CurrentScene)) ? true : false;
	}

	/// <summary>
	/// When store opens, get the latest gate and return the allowed decoration types
	/// based on the partition xml data.
	/// </summary>
	/// <returns>The allowed deco type from latest unlocked gate.</returns>
	public List<string> GetAllowedDecoTypeFromLatestPartition(){
		string preparedPartitionString = "Partition" + StringUtils.FormatIntToDoubleDigitString(GetLatestUnlockedAbsolutePartition());
		ImmutableDataPartition partitionData = DataLoaderPartitions.GetData(preparedPartitionString);
		return new List<string>(partitionData.DecoCategoriesStore);
	}

	public Vector3 GetWanderPoint() {
		int currentPartition = cameraGO.currentLocalPartition;
		Vector3 wanderPoint;
		int rand = UnityEngine.Random.Range(0,4);
		while (rand == lastKnownWanderPoint) {
			rand = UnityEngine.Random.Range(0, 4);
		}
		lastKnownWanderPoint = rand;
		wanderPoint = petWanderPoints[currentPartition].GetChild(rand).position;
        return wanderPoint;
	}
	public int GetCurrentPartition() {
		int currentPartition = cameraGO.currentLocalPartition;
		return currentPartition;
	}
//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "base")){
//			Debug.Log(GetBasePositionInBedroom());
//		}
//		if(GUI.Button(new Rect(200, 100, 100, 100), "minigame")){
//			Debug.Log(GetUnusedPositionNextToMinigame(GetRandomUnlockedMinigameType()));
//		}
//		if(GUI.Button(new Rect(300, 100, 100, 100), "unused")){
//			Debug.Log(GetRandomUnusedPosition());
//		}
//		if(GUI.Button(new Rect(400, 100, 100, 100), "4")){
//			Debug.Log(IsPartitionInCurrentZone(2));
//		}
//		if(GUI.Button(new Rect(500, 100, 100, 100), "5")){
//			Debug.Log(IsPartitionInCurrentZone(4));
//		}
//		if(GUI.Button(new Rect(600, 100, 100, 100), "6")){
//			Debug.Log(IsPartitionInCurrentZone(-1));
//		}
//		if(GUI.Button(new Rect(700, 100, 100, 100), "7")){
//			Debug.Log(IsPartitionInCurrentZone(8));
//		}
//	}
}

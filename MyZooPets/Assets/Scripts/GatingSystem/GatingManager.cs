using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gating manager. This manager is in charge of behavior related to
/// the smoke monster and blocking access for the
/// player.
/// </summary>
public class GatingManager : Singleton<GatingManager> {
	public EventHandler<EventArgs> OnReachedGate;           // When the player gets to the gate
	public static EventHandler<EventArgs> OnDamageGate;     // When player damages the gate
	public static EventHandler<EventArgs> OnDestroyedGate;  // When a specific gate has been destroyed

	public Vector3 startingLocation;     // starting location for the gates -- might differ from area to area

	private string currentZone;			// area that this manager is in
	public string CurrentZone {
		get { return currentZone; }
	}

	public int damage = 1;

	private ImmutableDataGate latestUnlockedGate;
	public ImmutableDataGate LatestUnlockedGate {
		get { return latestUnlockedGate; }
	}

	/// <summary>
	/// The starting screen position. The Gates' position is decided by percentage of screen.
	/// need a base point to calculate the actual screen position for all the gates
	/// before converting them to world point
	/// </summary>
	public Vector3 startingScreenPosition;

	private PanToMoveCamera scriptPan; // the pan to movement script; it's got constants we need...
	public Dictionary<int, Gate> activeGates = new Dictionary<int, Gate>(); //gates currently in the game

	void Awake() {
		// set pan script
		scriptPan = CameraManager.Instance.PanScript;

		// Get current zone
		currentZone = SceneUtils.GetZoneTypeFromSceneName(SceneManager.GetActiveScene().name).ToString();
	}

	void Start() {
		// see if the gating system is enabled
		if(!DataManager.Instance.GameData.GatingProgress.IsEnabled()) {
			return;
		}

		// listen for partition changing event
		scriptPan.OnPartitionChanged += EnteredRoom;

		// prior to spawning gates, we need to do some work to see if any recurring gates need to be refreshed
		RecurringGateCheck();

		// now spawn the gates
		SpawnGates();
	}

	/// <summary>
	/// Calculates the latest unlocked gate.
	/// This should be called everytime that a gate is unlocked
	/// 'Null' if no unlocked gates yet
	/// </summary>
	private ImmutableDataGate GetLatestUnlockedGate() {
		List<ImmutableDataGate> gateList = DataLoaderGate.GetAllData();
		int maxGateNumberSoFar = -1;
		ImmutableDataGate latestGateSoFar = null;
		foreach(ImmutableDataGate gate in gateList) {
			if(gate.GateNumber > maxGateNumberSoFar && !DataManager.Instance.GameData.GatingProgress.IsGateActive(gate.GateID)) {
				maxGateNumberSoFar = gate.GateNumber;
				latestGateSoFar = gate;
			}
		}
		latestUnlockedGate = latestGateSoFar;   // Cache it
		return latestGateSoFar;
	}


	public ImmutableDataGate GetLatestLockedGate() {
		List<ImmutableDataGate> gateList = DataLoaderGate.GetAllData();
		int minLockedGateNumberSoFar = 999;
		ImmutableDataGate minLockedGateSoFar = null;
		foreach(ImmutableDataGate gate in gateList) {
			if(gate.GateNumber < minLockedGateNumberSoFar && DataManager.Instance.GameData.GatingProgress.IsGateActive(gate.GateID)) {
				minLockedGateNumberSoFar = gate.GateNumber;
				minLockedGateSoFar = gate;
			}
		}
		latestUnlockedGate = minLockedGateSoFar;    // Cache it
		return minLockedGateSoFar;
	}

	/// <summary>
	/// Some gates recur -- that is, if they have been destroyed, after a set amount of time, the gate
	/// will be refreshed for the player to clear again.
	/// NOTE: this function just sets up the data for the SpawnGates() function to work with.
	/// </summary>
	private void RecurringGateCheck() {
		// loop through all gates...if the gate is inactive (destroyed) but is marked as recurring, and the player can breath fire,
		// the gate should be refreshed.  Note that this is a fairly crude way of deciding if the gate should be refreshed or not,
		// but it works for our initial design of the gate...can be changed later

		DateTime now = LgDateTime.GetTimeNow();
		DateTime lastRecurringGateSpawnedPlayPeriod = DataManager.Instance.GameData.GatingProgress.LastRecurringGateSpawnedPlayPeriod;
		TimeSpan timeSinceLastSpawned = now - lastRecurringGateSpawnedPlayPeriod;

		// Next play period. refresh recurring gate
		if(timeSinceLastSpawned.TotalHours >= 12) {
			Hashtable gates = DataLoaderGate.GetZoneGates(currentZone);
			//Debug.Log("RECURRING GATE REFRESH");
			foreach(DictionaryEntry entry in gates) {
				ImmutableDataGate dataGate = (ImmutableDataGate)entry.Value;

				bool isRecurring = dataGate.IsRecurring;
				bool isGateActive = DataManager.Instance.GameData.GatingProgress.IsGateActive(dataGate.GateID);

				if(isRecurring && !isGateActive) {
					DataManager.Instance.GameData.GatingProgress.RefreshGate(dataGate);
				}
			}
		}
	}

	/// <summary>
	/// Spawns the gates.
	/// </summary>
	private void SpawnGates() {
		Hashtable hashGates = DataLoaderGate.GetZoneGates(currentZone);
		foreach(DictionaryEntry entry in hashGates) {
			ImmutableDataGate dataGate = (ImmutableDataGate)entry.Value;
			int partition = dataGate.LocalPartition;

			// if the gate is activate, spawn the monster at an offset 
			bool isGateActive = DataManager.Instance.GameData.GatingProgress.IsGateActive(dataGate.GateID);

			// check if gate is still in the scene. SpawnGates is also called after
			// game is paused so need to check the status of each gate. This is mainly
			// for the recurring gate to be spawned correctly
			bool isGateInSceneAlready = false;
			if(activeGates.ContainsKey(partition)) {
				try {
					GameObject gateObject = activeGates[partition].gameObject;
					if(gateObject)
						isGateInSceneAlready = true;
				}
				// Gate object has already been destroyed.
				catch(MissingReferenceException e) {
					Debug.LogException(e);
					isGateInSceneAlready = false;
				}
				catch(NullReferenceException e) {
					Debug.LogException(e);
					isGateInSceneAlready = false;
				}
			}

			if(isGateActive && !isGateInSceneAlready) {
				int startingCurrentPartition = scriptPan.currentLocalPartition;         // room the player is in
				float roomPartitionOffset = scriptPan.partitionOffset;      // the distance between each room
				int partitionCountFromStartingPartition = dataGate.LocalPartition - startingCurrentPartition;   // the distance between the starting room and this gate's room
				float distanceFromStartingPartition = partitionCountFromStartingPartition * roomPartitionOffset; // offset of the gate

				// how much screen space should the gate be moved by
				float screenOffset = Screen.width * dataGate.ScreenPercentage;
				Vector3 newScreenPosition = new Vector3(screenOffset, startingScreenPosition.y, startingScreenPosition.z);

				float maxScreenSpace = Screen.width - screenOffset;

				// convert screen space back to world space
				Vector3 worldLocation = Camera.main.ScreenToWorldPoint(newScreenPosition);

				//we only want the x position from worldLocation. y and z should stay the same
				Vector3 gateLocation = new Vector3(worldLocation.x, startingLocation.y, startingLocation.z);

				// move the offsetted gate to the proper partition
				gateLocation.x += distanceFromStartingPartition;

				// create the gate at the location and set its id
				string strPrefab = dataGate.GetMonster().ResourceKey;
				GameObject prefab = Resources.Load(strPrefab) as GameObject;
				GameObject goGate = Instantiate(prefab, gateLocation, Quaternion.identity) as GameObject;
				Gate scriptGate = goGate.GetComponent<Gate>();

				string gateID = dataGate.GateID;
				scriptGate.Init(gateID, dataGate.GetMonster(), maxScreenSpace);

				if(dataGate.IsRecurring) {
					DataManager.Instance.GameData.GatingProgress.LastRecurringGateSpawnedPlayPeriod =
						PlayPeriodLogic.GetCurrentPlayPeriod();
				}

				// hash the gate based on the room, for easier access
				activeGates[partition] = scriptGate;
			}
		}
	}

	/// <summary>
	/// When player clears the gate.
	/// </summary>
	public void GateCleared() {
		// enable the player to do stuff in the room
		EnableUI();
	}

	/// <summary>
	/// Determines whether this instance is in gated room.
	/// </summary>
	/// <returns><c>true</c> if this instance is in gated room; otherwise, <c>false</c>.</returns>
	public bool IsInGatedRoom() {
		int currentPartition = scriptPan.currentLocalPartition;
		bool isGated = HasActiveGate(currentZone, currentPartition);
		return isGated;
	}

	/// <summary>
	/// Determines whether partition has active gate
	/// </summary>
	/// <returns><c>true</c> if there is active gate the specified partition; otherwise, <c>false</c>.</returns>
	public bool HasActiveGate(int partition) {
		bool hasGate = HasActiveGate(currentZone, partition);
		return hasGate;
	}

	/// <summary>
	/// Determines whether area roomPartition has active gate.
	/// </summary>
	/// <returns><c>true</c> if there is active gate at the specified area roomPartition; otherwise, <c>false</c>.</returns>
	public bool HasActiveGate(string zone, int localParition) {
		bool isActive = false;
		ImmutableDataGate data = DataLoaderGate.GetData(zone, localParition);
		if(data != null) {
			isActive = DataManager.Instance.GameData.GatingProgress.IsGateActive(data.GateID);
		}
		return isActive;
	}

	/// <summary>
	/// Determines whether the player can enter room.
	/// </summary>
	/// <returns><c>true</c> if player can enter room ie player is moving from a smog room; otherwise, <c>false</c>.</returns>
	/// <param name="currentRoom">Current room.</param>
	/// <param name="eSwipeDirection">E swipe direction.</param>
	public bool CanEnterRoom(int currentLocalPartition, RoomDirection swipeDirection) {
		// start off optimistic
		bool isAllowed = true;

		// if there is an active gate in this room, check to see if it is blocking the direction the player is trying to go in
		ImmutableDataGate dataGate = DataLoaderGate.GetData(currentZone, currentLocalPartition);
		if(dataGate != null &&
			DataManager.Instance.GameData.GatingProgress.IsGateActive(dataGate.GateID) &&
			dataGate.DoesBlock(swipeDirection))
			isAllowed = false;

		return isAllowed;
	}

	/// <summary>
	/// Entereds the room.
	/// NOTE: Currently, exiting a gated room into another gated room is not by design,
	/// and also not supported
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public void EnteredRoom(object sender, PartitionChangedArgs args) {
		int leavingPartitionNumber = args.oldPartition;
		int enteringPartitionNumber = args.newPartition;

		bool isGateLeavingActive = HasActiveGate(currentZone, leavingPartitionNumber);
		bool isGateEnteringActive = HasActiveGate(currentZone, enteringPartitionNumber);

		if(isGateEnteringActive) {
			// if the player is entering a gated room, hide some ui and lock the click manager
			List<ClickLockExceptions> listExceptions = new List<ClickLockExceptions>();
			listExceptions.Add(ClickLockExceptions.Moving);
			ClickManager.Instance.Lock(UIModeTypes.GatingSystem, listExceptions);

			NavigationUIManager.Instance.HidePanel();

			// let the gate know that the player has entered the room
			Gate gate = (Gate)activeGates[enteringPartitionNumber];
			gate.GreetPlayer();

			// also, move the player to a specific location
			MovePlayer(enteringPartitionNumber);

			// we neeed to listen to when the player is done moving to handle other gate related stuff
			ListenForMovementFinished(true);
		}
		else {
			// if the pet is leaving a gated room, destroy the fire UI and stop listening for a callback
			ListenForMovementFinished(false);
		}

		// if they are entering a non-gated room from a gated room, show that ui and unlock click manager
		if(isGateLeavingActive && !isGateEnteringActive)
			EnableUI();

	}

	/// <summary>
	/// Damages the gate.
	/// </summary>
	/// <returns><c>true</c>, if gate was destroyed, <c>false</c> otherwise.</returns>
	/// <param name="gateID">Gate ID.</param>
	/// <param name="damage">Damage amount.</param>
	public bool DamageGate(string gateID) {
		// check to make sure the gate exists
		if(!DataManager.Instance.GameData.GatingProgress.GatingProgress.ContainsKey(gateID)) {
			Debug.LogError("Something trying to access a non-existant gate " + gateID);
			return true;
		}

		// check to make sure the gate is active
		if(!DataManager.Instance.GameData.GatingProgress.IsGateActive(gateID)) {
			Debug.LogError("Something trying to damage an inactive gate " + gateID);
			return true;
		}

		// otherwise, calculate and save the new hp
		int hp = DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID];
		hp -= damage;
		DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID] = hp;

		// then return whether or not the gate has been destroyed
		bool isDestroyed = hp <= 0;

		// Fire event to notify any UI that GatinProgress data may have been changed
		if(OnDamageGate != null) {
			OnDamageGate(this, EventArgs.Empty);
		}

		if(isDestroyed) {
			// Fire event to notify gate with gateID has been destroyed
			if(OnDestroyedGate != null) {
				OnDestroyedGate(this, null);
			}

			// Recalculate the latest unlocked gate
			GetLatestUnlockedGate();
		}

		return isDestroyed;
	}


	/// <summary>
	/// Shows the no fire notification.
	/// </summary>
	public void IndicateNoFire() {
		// if inhaler is ready to be used prompt user to use inhaler
		if(PlayPeriodLogic.Instance.CanUseEverydayInhaler()) {
			PetSpeechAI.Instance.ShowInhalerMsg();
		}
		// check if there's fire orb in the inventory
		else {
			// if there's a fire orb in the inventory prompt the user to use it
			InventoryItem fireOrb = InventoryManager.Instance.GetItemInInventory("Usable1");
			if(fireOrb != null) {
			}
			// if not tell user to buy flame crystal
			else {
				FlameCrystalNotification();
			}
		}
	}

	/// <summary>
	/// In app purchase notification. Prompt the user to either wait for inhaler or
	/// buy flame crystal from the store with gems
	/// </summary>
	private void FlameCrystalNotification() {
		//Debug.LogError("NEW NOTIFICATION HERE");
		PetSpeechAI.Instance.ShowOutOfFireMsg();

	}

	/// <summary>
	/// Listens for movement finished.
	/// </summary>
	/// <param name="isFinished">If set to <c>true</c> is finished.</param>
	private void ListenForMovementFinished(bool isFinished) {
		if(isFinished) {
			PetMovement.Instance.OnReachedDest += PetReachedDest;
		}
		else {
			PetMovement.Instance.OnReachedDest -= PetReachedDest;
		}
	}


	public void CanNowBlowFire() {
		PetInteractionManager.Instance.isInteractable = false;

		//once pet is in position for fire breathing. Idle animations need to be turn off
		//otherwise it might break the breathe fire animation
		PetAnimationManager.Instance.DisableIdleAnimation();

		// if the pet is happy and healthy, add the fire button
		PetHealthStates healthState = DataManager.Instance.GameData.Stats.GetHealthState();
		PetMoods moodState = DataManager.Instance.GameData.Stats.GetMoodState();

		if(healthState == PetHealthStates.Healthy && moodState == PetMoods.Happy) {
			ArrivedShowFireButton();

			//if can't breathe fire show message
			bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();
			if(!canBreatheFire) {
				IndicateNoFire();
			}
		}
		else {
			ShowUnhealthyNoFireSpeech();
		}
	}
	/// <summary>
	/// Pets the reached destination. It is critical this function is only
	/// called if the pet is entering a gated room.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void PetReachedDest(object inhasender, EventArgs args) {
		if(OnReachedGate != null) {
			OnReachedGate(this, EventArgs.Empty);
		}

		PetInteractionManager.Instance.isInteractable = false;

		//once pet is in position for fire breathing. Idle animations need to be turn off
		//otherwise it might break the breathe fire animation
		PetAnimationManager.Instance.DisableIdleAnimation();

		// if the pet is happy and healthy, add the fire button
		PetHealthStates healthState = DataManager.Instance.GameData.Stats.GetHealthState();
		PetMoods moodState = DataManager.Instance.GameData.Stats.GetMoodState();

		if(healthState == PetHealthStates.Healthy && moodState == PetMoods.Happy) {
			ArrivedShowFireButton();

			//if can't breathe fire show message
			bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();
			if(!canBreatheFire) {
				IndicateNoFire();
			}
		}
		else {
			ShowUnhealthyNoFireSpeech();
		}

		// regardless, stop listening for the callback now that we've received it
		ListenForMovementFinished(false);
	}

	/// <summary>
	/// Shows the no fire notification. Calling this function assumes the player
	/// cannot breathe fire
	/// </summary>
	private void ShowUnhealthyNoFireSpeech() {
		PetHealthStates healthState = DataManager.Instance.GameData.Stats.GetHealthState();
		PetMoods moodState = DataManager.Instance.GameData.Stats.GetMoodState();

		if(healthState != PetHealthStates.Healthy) {
			PetSpeechAI.Instance.ShowNoFireSickMsg();
		}
		else if(moodState != PetMoods.Happy) {
			PetSpeechAI.Instance.ShowNoFireHungryMsg();
		}
	}

	/// <summary>
	/// Shows the fire button.
	/// </summary>
	private void ArrivedShowFireButton() {
		// the pet has reached its destination (in front of the monster) so show the fire UI
		FireButtonManager.Instance.Activate();

		// get the gate in this room
		Gate gate = activeGates[scriptPan.currentLocalPartition];
		if(gate) {
			FireButtonManager.Instance.CurrentGate = gate;
		}
		else {
			Debug.LogError("Destination callback being called for non-gated room");
		}
	}

	//TODO: need fix up
	private void ReturnToGatingSystemUIMode(object sender, EventArgs args) {
		ClickManager.Instance.ReleaseLock();
		StoreUIManager.OnShortcutModeEnd -= ReturnToGatingSystemUIMode;
	}
	
	private void EnableUI() {
		ClickManager.Instance.ReleaseLock();
		NavigationUIManager.Instance.ShowPanel();

		PetInteractionManager.Instance.isInteractable = true;

		//turn pet idle animation back on before leaving the gated room
		PetAnimationManager.Instance.EnableIdleAnimation();
	}

	/// <summary>
	/// Moves the player to the appropriate location in relation to the gate in the incoming room.
	/// If you're wondering, there is some semi-duplicate code in DoneAttacking script.
	/// I did that because I didn't want to use quite want to use the same paths so I could
	/// easily tell when the player was entering the room and when they were already in it.
	/// </summary>
	private void MovePlayer(int roomPartition) {
		// then get the id of the gate and get that gate object from our list of active gates
		if(activeGates.ContainsKey(roomPartition)) {
			Gate gate = activeGates[roomPartition];

			// get the position the player should approach
			Vector3 playerPosition = gate.GetPlayerPosition();

			// phew...now tell the player to move
			PetMovement.Instance.MovePet(playerPosition);
		}
	}
}

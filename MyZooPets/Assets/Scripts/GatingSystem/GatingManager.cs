using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gating manager. This manager is in charge of behavior related to
/// the smoke monster and blocking access for the
/// player.
/// </summary>

public class DestroyedGateEventArgs : EventArgs{
	public string DestroyedGateID{ get; set; }
	public string MiniPetID{ get; set; }
}

public class GatingManager : Singleton<GatingManager>{
	//=======================Events========================
	public EventHandler<EventArgs> OnReachedGate;   // when the player gets to the gate
	public static EventHandler<EventArgs> OnDamageGate; // When player damages the gate
	public static EventHandler<DestroyedGateEventArgs> OnDestroyedGate; // When a specific gate has been destroyed
	//=====================================================
    
	public string currentArea; // area that this manager is in
	public Vector3 startingLocation; // starting location for the gates -- might differ from area to area

	/// <summary>
	/// The starting screen position. The Gates' position is decided by percentage of screen.
	/// need a base point to calculate the actual screen position for all the gates
	/// before converting them to world point
	/// </summary>
	public Vector3 startingScreenPosition;
	private PanToMoveCamera scriptPan; // the pan to movement script; it's got constants we need...
	private Hashtable activeGates = new Hashtable(); // hash of active gates that the manager is currently managing

	public Hashtable ActiveGates{
		get{ return activeGates; }
	}

	void Start(){		
		// set pan script
		scriptPan = CameraManager.Instance.GetPanScript();
		
		// see if the gating system is enabled
		if(!DataManager.Instance.GameData.GatingProgress.IsEnabled())
			return;
		
		// listen for partition changing event
		scriptPan.OnPartitionChanged += EnteredRoom;
		
		// prior to spawning gates, we need to do some work to see if any recurring gates need to be refreshed
		RecurringGateCheck();
		
		// now spawn the gates
		SpawnGates();
	}
		
	/// <summary>
	/// Recurrings the gate check.
	/// Some gates recur -- that is, if they have been
	/// destroyed, after a set amount of time, the gate
	/// will be refreshed for the player to clear again.
	/// Note that this function just sets up the data for
	/// the SpawnGates() function to work with.
	/// </summary>
	private void RecurringGateCheck(){
		// loop through all gates...if the gate is inactive (destroyed) but is marked as recurring, and the player can breath fire,
		// the gate should be refreshed.  Note that this is a fairly crude way of deciding if the gate should be refreshed or not,
		// but it works for our initial design of the gate...can be changed later
		Hashtable gates = DataLoaderGate.GetAreaGates(currentArea);
		foreach(DictionaryEntry entry in gates){
			ImmutableDataGate dataGate = (ImmutableDataGate)entry.Value;
			
			bool isRecurring = dataGate.IsRecurring();
			bool isGateActive = DataManager.Instance.GameData.GatingProgress.IsGateActive(dataGate.GetGateID());
			bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();
			
			if(isRecurring && !isGateActive && canBreatheFire)
				DataManager.Instance.GameData.GatingProgress.RefreshGate(dataGate);
		}
	}

	/// <summary>
	/// Spawns the gates.
	/// </summary>
	private void SpawnGates(){
		Hashtable hashGates = DataLoaderGate.GetAreaGates(currentArea);
		foreach(DictionaryEntry entry in hashGates){
			ImmutableDataGate dataGate = (ImmutableDataGate)entry.Value;
			
			// if the gate is activate, spawn the monster at an offset 
			bool isGateActive = DataManager.Instance.GameData.GatingProgress.IsGateActive(dataGate.GetGateID());
			if(isGateActive){
				int startingPartition = scriptPan.currentPartition;	// room the player is in
				float roomPartitionOffset = scriptPan.partitionOffset; // the distance between each room
				int partitionCountFromStartingPartition = dataGate.GetPartition() - startingPartition;	// the distance between the starting room and this gate's room
				float distanceFromStartingPartition = partitionCountFromStartingPartition * roomPartitionOffset; // offset of the gate

				// how much screen space should the gate be moved by
				float screenOffset = Screen.width * dataGate.GetScreenPercentage();
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
				
				string gateID = dataGate.GetGateID();
				scriptGate.Init(gateID, dataGate.GetMonster(), maxScreenSpace);
				
				// hash the gate based on the room, for easier access
				int partition = dataGate.GetPartition();
				activeGates[partition] = scriptGate;
			}
		}		
	}

	/// <summary>
	/// When player clears the gate.
	/// </summary>
	public void GateCleared(){
		// enable the player to do stuff in the room
		EnableUI();
	}

	/// <summary>
	/// Determines whether this instance is in gated room.
	/// </summary>
	/// <returns><c>true</c> if this instance is in gated room; otherwise, <c>false</c>.</returns>
	public bool IsInGatedRoom(){
		int currentPartition = scriptPan.currentPartition;
		bool isGated = HasActiveGate(currentArea, currentPartition);
		
		return isGated;
	}

	/// <summary>
	/// Determines whether partition has active gate
	/// </summary>
	/// <returns><c>true</c> if there is active gate the specified partition; otherwise, <c>false</c>.</returns>
	/// <param name="partition">Partition.</param>
	public bool HasActiveGate(int partition){
		bool hasGate = HasActiveGate(currentArea, partition);
		return hasGate;
	}

	/// <summary>
	/// Determines whether area roomPartition has active gate.
	/// </summary>
	/// <returns><c>true</c> if there is active gate at the specified area roomPartition; otherwise, <c>false</c>.</returns>
	/// <param name="area">Area.</param>
	/// <param name="roomPartition">Room partition.</param>
	public bool HasActiveGate(string area, int roomPartition){
		bool isActive = false;
		
		ImmutableDataGate data = DataLoaderGate.GetData(area, roomPartition);
		if(data != null) 
			isActive = DataManager.Instance.GameData.GatingProgress.IsGateActive(data.GetGateID());
		
		return isActive;
	}
	
	/// <summary>
	/// Determines whether the player can enter room.
	/// </summary>
	/// <returns><c>true</c> if player can enter room ; otherwise, <c>false</c>.</returns>
	/// <param name="currentRoom">Current room.</param>
	/// <param name="eSwipeDirection">E swipe direction.</param>
	public bool CanEnterRoom(int currentRoom, RoomDirection swipeDirection){
		// early out if click manager is tweening
		if(ClickManager.Instance.IsTweeningUI())
			return false;
		
		// start off optimistic
		bool isAllowed = true;
		
		// if there is an active gate in this room, check to see if it is blocking the direction the player is trying to go in
		ImmutableDataGate dataGate = DataLoaderGate.GetData(currentArea, currentRoom);
		if(dataGate != null && 
			DataManager.Instance.GameData.GatingProgress.IsGateActive(dataGate.GetGateID()) && 
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
	public void EnteredRoom(object sender, PartitionChangedArgs args){
		int leavingPartitionNumber = args.nOld;
		int enteringPartitionNumber = args.nNew;
		
		bool isGateLeavingActive = HasActiveGate(currentArea, leavingPartitionNumber);
		bool isGateEnteringActive = HasActiveGate(currentArea, enteringPartitionNumber);
		
		if(isGateEnteringActive){
			// if the player is entering a gated room, hide some ui and lock the click manager
			List<ClickLockExceptions> listExceptions = new List<ClickLockExceptions>();
			listExceptions.Add(ClickLockExceptions.Moving);
			ClickManager.Instance.Lock(UIModeTypes.GatingSystem, listExceptions);

			NavigationUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
//			InventoryUIManager.Instance.HidePanel();
			
			// let the gate know that the player has entered the room
			Gate gate = (Gate)activeGates[enteringPartitionNumber];
			gate.GreetPlayer();
			
			// also, move the player to a specific location
			MovePlayer(enteringPartitionNumber);
			
			// we neeed to listen to when the player is done moving to handle other gate related stuff
			ListenForMovementFinished(true);
		}
		else{
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
	public bool DamageGate(string gateID, int damage){
		// check to make sure the gate exists
		if(!DataManager.Instance.GameData.GatingProgress.GatingProgress.ContainsKey(gateID)){
			Debug.LogError("Something trying to access a non-existant gate " + gateID);
			return true;
		}
		
		// check to make sure the gate is active
		if(!DataManager.Instance.GameData.GatingProgress.IsGateActive(gateID)){
			Debug.LogError("Something trying to damage an inactive gate " + gateID);
			return true;
		}

		// otherwise, calculate and save the new hp
		int hp = DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID];
		hp = Mathf.Max(hp - damage, 0);
		DataManager.Instance.GameData.GatingProgress.GatingProgress[gateID] = hp;
		
		// then return whether or not the gate has been destroyed
		bool isDestroyed = hp <= 0;

		// Fire event to notify any UI that GatinProgress data may have been changed
		if(OnDamageGate != null)
			OnDamageGate(this, EventArgs.Empty);

		// Fire event to notify gate with gateID has been destroyed
		if(isDestroyed){
			if(OnDestroyedGate != null){
				DestroyedGateEventArgs args = new DestroyedGateEventArgs();
				
				args.DestroyedGateID = gateID;
				args.MiniPetID = DataLoaderGate.GetData(gateID).GetMiniPetID();

				OnDestroyedGate(this, args);
			}
		}
			
		return isDestroyed;
	}	

	/// <summary>
	/// Listens for movement finished.
	/// </summary>
	/// <param name="isFinished">If set to <c>true</c> is finished.</param>
	private void ListenForMovementFinished(bool isFinished){
		if(isFinished)
			PetMovement.Instance.OnReachedDest += PetReachedDest;
		else
			PetMovement.Instance.OnReachedDest -= PetReachedDest;			
	}

	/// <summary>
	/// Pets the reached destination. It is critical this function is only
	/// called if the pet is entering a gated room.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void PetReachedDest(object sender, EventArgs args){
		
		// process any callbacks for when the pet reaches a gate
		if(OnReachedGate != null)
			OnReachedGate(this, EventArgs.Empty);		
	
		//Check if version is Lite. spawn cross promo ads if so
		bool tutDone = DataManager.Instance.GameData.Tutorial.AreTutorialsFinished();
		if(tutDone && VersionManager.IsLite())
			GateLiteLogic();
		else
			GateProLogic();
		
		// regardless, stop listening for the callback now that we've received it
		ListenForMovementFinished(false);
	}

	private void GateLiteLogic(){
		LgCrossPromo.ShowInterstitial(LgCrossPromo.LAST_GATE);
	}

	private void GateProLogic(){
		// if the pet is happy and healthy, add the fire button
		PetHealthStates eState = DataManager.Instance.GameData.Stats.GetHealthState();
		PetMoods eMood = DataManager.Instance.GameData.Stats.GetMoodState();
		bool bCanBreath = DataManager.Instance.GameData.PetInfo.CanBreathFire();

		if(eState == PetHealthStates.Healthy && eMood == PetMoods.Happy) 
			ShowFireButton();
		else{
			// otherwise, we want to show the tutorial explaining why the fire button isn't there (if it hasn't been shown)	
			ShowNoFireNotification();
		}
	}

	/// <summary>
	/// Shows the no fire notification. Calling this function assumes the player
	/// cannot breathe fire
	/// </summary>
	private void ShowNoFireNotification(){
		//use thought bubble instead. try to stay away from notification

		PetHealthStates eState = DataManager.Instance.GameData.Stats.GetHealthState();
		PetMoods eMood = DataManager.Instance.GameData.Stats.GetMoodState();
		
		if(eState != PetHealthStates.Healthy){

			PetSpeechAI.Instance.ShowNoFireSickMsg();
		}
		else if(eMood != PetMoods.Happy){

			PetSpeechAI.Instance.ShowNoFireHungryMsg();
		}
		else{
//			if(PlayPeriodLogic.Instance.CanUseRealInhaler()){
//				PetSpeechAI.Instance.ShowInhalerMsg();
//			}
//			else{
//				//TODO: Missing logic here. If the user already has fire orb need a diff message
//
//				PopupNotificationNGUI.HashEntry okButtonCallback = delegate(){
//					StoreUIManager.OnShortcutModeEnd += ReturnToGatingSystemUIMode;
//
//					ClickManager.Instance.Lock(UIModeTypes.Store);
//					StoreUIManager.Instance.OpenToSubCategory("Items", isShortCut: true);
//				};
//
//				Hashtable notificationEntry = new Hashtable();
//				notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.InhalerRecharging);
//				notificationEntry.Add(NotificationPopupFields.Button1Callback, okButtonCallback);
//				
//				NotificationUIManager.Instance.AddToQueue(notificationEntry);
//				
//			}
		
//		    				PetSpeechAI.Instance.ShowOutOfFireMsg();
			//TODO: enable FireOrbMsg once it's ready to integrate
		}
	}

	//TODO: need fix up
	private void ReturnToGatingSystemUIMode(object sender, EventArgs args){
		ClickManager.Instance.ReleaseLock();

		StoreUIManager.OnShortcutModeEnd -= ReturnToGatingSystemUIMode;
	} 

	/// <summary>
	/// Shows the fire button.
	/// </summary>
	public void ShowFireButton(){
		// the pet has reached its destination (in front of the monster) so show the fire UI
		GameObject resourceFireButton = Resources.Load(ButtonMonster.FIRE_BUTTON) as GameObject;
		GameObject goFireButton = LgNGUITools.AddChildWithPosition(GameObject.Find("Anchor-Center"), resourceFireButton);

		// Find the position of the pet and transform that position into NGUI screen space.
		// The fire button will always be spawned at the pet's location
		GameObject petLocation = GameObject.Find("Pet_LWF");
		Vector3 fireButtonLoc = CameraManager.Instance.WorldToScreen(CameraManager.Instance.cameraMain, 
		                                                             petLocation.transform.position);
		fireButtonLoc = CameraManager.Instance.TransformAnchorPosition(fireButtonLoc, 
		                                                          InterfaceAnchors.BottomLeft, 
		                                                               InterfaceAnchors.Center);


		// if ( TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive() )
		// 	strConstant = "FireLoc_Tutorial";
	
		// set location of the button based on if it is a tutorial or not
//		Vector3 fireButtonLoc = Constants.GetConstant<Vector3>("FireLoc_Normal");
		goFireButton.transform.localPosition = fireButtonLoc;
		
		// rename the button so that other things can find it
		goFireButton.name = ButtonMonster.FIRE_BUTTON;
		
		// get the gate in this room
		Gate gate = (Gate)activeGates[scriptPan.currentPartition];
		if(gate){
			// this is a bit hackey, but the actual fire button is in a child because we need to make a better pivot
			Transform transButton = goFireButton.transform.Find("ButtonParent/Button");
			ButtonMonster script = transButton.gameObject.GetComponent<ButtonMonster>();
			script.SetGate(gate);
		}
		else
			Debug.LogError("Destination callback being called for non gated room");		
	}

	/// <summary>
	/// Enables the UI.
	/// </summary>
	private void EnableUI(){
		ClickManager.Instance.ReleaseLock();
		NavigationUIManager.Instance.ShowPanel();
		EditDecosUIManager.Instance.ShowNavButton();		
//		InventoryUIManager.Instance.ShowPanel();
	}
	
	//---------------------------------------------------
	// MovePlayer()
	// Moves the player to the appropriate location in
	// relation to the gate in the incoming room.
	// If you're wondering, there is some semi-duplicate
	// code in DoneAttacking script.  I did that because
	// I didn't want to use quite want to use the same
	// paths so I could easily tell when the player was
	// entering the room and when they were already in it.
	//---------------------------------------------------	
	private void MovePlayer(int roomPartition){
		// then get the id of the gate and get that gate object from our list of active gates
		Gate gate = (Gate)activeGates[roomPartition];
		
		// get the position the player should approach
		Vector3 playerPosition = gate.GetPlayerPosition();
		
		// phew...now tell the player to move
		PetMovement.Instance.MovePet(playerPosition);		
	}
	

}

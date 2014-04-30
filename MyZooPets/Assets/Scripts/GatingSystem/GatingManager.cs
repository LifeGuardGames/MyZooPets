using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GatingManager
// This manager is in charge of behavior related to
// the smoke monster and blocking access for the
// player.
//---------------------------------------------------

public class GatingManager : Singleton<GatingManager>{
	//=======================Events========================
	public EventHandler<EventArgs> OnReachedGate;   // when the player gets to the gate
	//=====================================================
    
	public string currentArea; // area that this manager is in
	public Vector3 startingLocation; // starting location for the gates -- might differ from area to area

	/// <summary>
	/// The starting screen position. The Gates' position is decided by percentage of screen
	/// need a base point to calculate the actual screen position for all the gates
	/// before converting them to world point
	/// </summary>
	public Vector3 startingScreenPosition; 

	private PanToMoveCamera scriptPan; // the pan to movement script; it's got constants we need...
	private Hashtable hashActiveGates = new Hashtable(); // hash of active gates that the manager is currently managing
	
	public string GetArea(){
		return currentArea;	
	}
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
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
		
	//---------------------------------------------------
	// RecurringGateCheck()
	// Some gates recur -- that is, if they have been
	// destroyed, after a set amount of time, the gate
	// will be refreshed for the player to clear again.
	// Note that this function just sets up the data for
	// the SpawnGates() function to work with.
	//---------------------------------------------------	
	private void RecurringGateCheck(){
		// loop through all gates...if the gate is inactive (destroyed) but is marked as recurring, and the player can breath fire,
		// the gate should be refreshed.  Note that this is a fairly crude way of deciding if the gate should be refreshed or not,
		// but it works for our initial design of the gate...can be changed later
		Hashtable hashGates = DataLoaderGate.GetAreaGates(currentArea);
		foreach(DictionaryEntry entry in hashGates){
			ImmutableDataGate dataGate = (ImmutableDataGate)entry.Value;
			
			bool isRecurring = dataGate.IsRecurring();
			bool isGateActive = DataManager.Instance.GameData.GatingProgress.IsGateActive(dataGate.GetGateID());
			bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();
			
			if(isRecurring && !isGateActive && canBreatheFire)
				DataManager.Instance.GameData.GatingProgress.RefreshGate(dataGate);
		}
	}
	
	//---------------------------------------------------
	// SpawnGates()
	// Creates all gates that are alive in the save data.
	//---------------------------------------------------	
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
				string strPrefab = dataGate.GetMonster().GetResourceKey();
				GameObject prefab = Resources.Load(strPrefab) as GameObject;
				GameObject goGate = Instantiate(prefab, gateLocation, Quaternion.identity) as GameObject;
				Gate scriptGate = goGate.GetComponent<Gate>();
				
				string gateID = dataGate.GetGateID();
				scriptGate.Init(gateID, dataGate.GetMonster(), maxScreenSpace);
				
				// hash the gate based on the room, for easier access
				int partition = dataGate.GetPartition();
				hashActiveGates[partition] = scriptGate;
			}
		}		
	}
	
	//---------------------------------------------------
	// GateCleared()
	// When the player clears a gate.
	//---------------------------------------------------		
	public void GateCleared(){
		// enable the player to do stuff in the room
		EnableUI();
	}
	
	//---------------------------------------------------
	// IsInGatedRoom()
	// Returns whether or not the player is currently
	// in a gated room.
	//---------------------------------------------------	
	public bool IsInGatedRoom(){
		int currentPartition = scriptPan.currentPartition;
		bool isGated = DataLoaderGate.HasActiveGate(currentArea, currentPartition);
		
		return isGated;
	}
	
	//---------------------------------------------------
	// HasActiveGate()
	// Returns if the incoming partition has a gate in it.
	// Note this assumes the area that this gating manager
	// is in.
	//---------------------------------------------------		
	public bool HasActiveGate(int partition){
		bool hasGate = DataLoaderGate.HasActiveGate(currentArea, partition);
		return hasGate;
	}

	//---------------------------------------------------
	// CanEnterRoom()
	// Returns whether the player can enter the incoming
	// room from the incoming direction.
	//---------------------------------------------------	
	public bool CanEnterRoom(int currentRoom, RoomDirection eSwipeDirection){
		// early out if click manager is tweening
		if(ClickManager.Instance.IsTweeningUI())
			return false;
		
		// start off optimistic
		bool isAllowed = true;
		
		// if there is an active gate in this room, check to see if it is blocking the direction the player is trying to go in
		ImmutableDataGate dataGate = DataLoaderGate.GetData(currentArea, currentRoom);
		if(dataGate != null && 
		   DataManager.Instance.GameData.GatingProgress.IsGateActive(dataGate.GetGateID()) && 
		   dataGate.DoesBlock(eSwipeDirection))
			isAllowed = false;
		
		return isAllowed; 
	}

	//---------------------------------------------------
	// EnteredRoom()
	// When the player enters a room.
	// NOTE: Currently, exiting a gated room into another
	// gated room is not by design, and also not supported.	
	//---------------------------------------------------	
	public void EnteredRoom(object sender, PartitionChangedArgs args){
		int nLeaving = args.nOld;
		int nEntering = args.nNew;
		
		bool bGateLeaving = DataLoaderGate.HasActiveGate(currentArea, nLeaving);
		bool bGateEntering = DataLoaderGate.HasActiveGate(currentArea, nEntering);
		
		if(bGateEntering){
			// if the player is entering a gated room, hide some ui and lock the click manager
			List<ClickLockExceptions> listExceptions = new List<ClickLockExceptions>();
			listExceptions.Add(ClickLockExceptions.Moving);
			ClickManager.Instance.Lock(UIModeTypes.Generic, listExceptions);
			NavigationUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
			InventoryUIManager.Instance.HidePanel();
			
			// let the gate know that the player has entered the room
			Gate gate = (Gate)hashActiveGates[nEntering];
			gate.GreetPlayer();
			
			// also, move the player to a specific location
			MovePlayer(nEntering);
			
			// we neeed to listen to when the player is done moving to handle other gate related stuff
			ListenForMovementFinished(true);
		}
		else{
			// if the pet is leaving a gated room, destroy the fire UI and stop listening for a callback
			ListenForMovementFinished(false);
		}
		
		// if they are entering a non-gated room from a gated room, show that ui and unlock click manager
		if(bGateLeaving && !bGateEntering) 
			EnableUI();

	}
	
	//---------------------------------------------------
	// ListenForMovementFinished()
	// Subscribes/unsubscribes to pet movemvent callback.
	//---------------------------------------------------	
	private void ListenForMovementFinished(bool bListen){
		if(bListen)
			PetMovement.Instance.OnReachedDest += PetReachedDest;
		else
			PetMovement.Instance.OnReachedDest -= PetReachedDest;			
	}
	
	//---------------------------------------------------
	// PetReachedDest()
	// Callback for when the pet reaches moving to its
	// destination.  It is critical this function is only
	// called if the pet is entering a gated room.
	//---------------------------------------------------	
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

		if(eState == PetHealthStates.Healthy && eMood == PetMoods.Happy && bCanBreath) 
			ShowFireButton();
		else{
			// otherwise, we want to show the tutorial explaining why the fire button isn't there (if it hasn't been shown)	
			ShowNoFireNotification();
		}
	}
	
	//---------------------------------------------------
	// ShowNoFireNotification()
	// Shows the player a notification explaining why
	// they cannot breath fire at this time.
	// Calling this function assumes the player cannot
	// breath fire.
	//---------------------------------------------------		
	private void ShowNoFireNotification(){
		PetHealthStates eState = DataManager.Instance.GameData.Stats.GetHealthState();
		PetMoods eMood = DataManager.Instance.GameData.Stats.GetMoodState();
		
		string strKey;			// key of text to show
		string strImage;		// image to appear on notification
		string strAnalytics = "";	// analytics tracker
		
		if(eState != PetHealthStates.Healthy){
			// pet is not healthy enough
			strKey = "NO_FIRE_SICK";
			strImage = "Skull";
			// strAnalytics = "BreathFire:Fail:Sick";
		}
		else if(eMood != PetMoods.Happy){
			// pet is not happy enough
			strKey = "NO_FIRE_UNHAPPY";
			strImage = "Skull";
			// strAnalytics = "BreathFire:Fail:Unhappy";
		}
		else{
			// out of flame charges
			strKey = "NO_FIRE_INHALER";
			strImage = "itemInhalerMain";
			// strAnalytics = "BreathFire:Fail:NoCharges";
		}
		
		string petName = DataManager.Instance.GameData.PetInfo.PetName;	
		string message = String.Format(Localization.Localize(strKey), petName);
		// show the standard popup
		TutorialUIManager.AddStandardTutTip(NotificationPopupType.TipWithImage, 
			message, strImage, null, true, true, strAnalytics);		
	}
	
	//---------------------------------------------------
	// ShowFireButton()
	// Shows the fire button to attack the gate.
	//---------------------------------------------------		
	private void ShowFireButton(){
		// the pet has reached its destination (in front of the monster) so show the fire UI
		GameObject resourceFireButton = Resources.Load(ButtonMonster.FIRE_BUTTON) as GameObject;
		GameObject goFireButton = LgNGUITools.AddChildWithPosition(GameObject.Find("Anchor-Center"), resourceFireButton);
		
		// set location of the button based on if it is a tutorial or not
		string strConstant = "FireLoc_Normal";
		// if ( TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive() )
		// 	strConstant = "FireLoc_Tutorial";
		
		Vector3 vLoc = Constants.GetConstant<Vector3>(strConstant);
		goFireButton.transform.localPosition = vLoc;
		
		// rename the button so that other things can find it
		goFireButton.name = ButtonMonster.FIRE_BUTTON;
		
		// get the gate in this room
		Gate gate = (Gate)hashActiveGates[scriptPan.currentPartition];
		if(gate){
			// this is a bit hackey, but the actual fire button is in a child because we need to make a better pivot
			Transform transButton = goFireButton.transform.Find("Button");
			ButtonMonster script = transButton.gameObject.GetComponent<ButtonMonster>();
			script.SetGate(gate);
		}
		else
			Debug.LogError("Destination callback being called for non gated room");		
	}
	
	//---------------------------------------------------
	// EnableUI()
	// Enables the UI for the player that had previously
	// been locked.
	//---------------------------------------------------	
	private void EnableUI(){
		ClickManager.Instance.ReleaseLock();
		NavigationUIManager.Instance.ShowPanel();
		EditDecosUIManager.Instance.ShowNavButton();		
		InventoryUIManager.Instance.ShowPanel();
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
	private void MovePlayer(int nRoom){
		// then get the id of the gate and get that gate object from our list of active gates
		Gate gate = (Gate)hashActiveGates[nRoom];
		
		// get the position the player should approach
		Vector3 vPos = gate.GetPlayerPosition();
		
		// phew...now tell the player to move
		PetMovement.Instance.MovePet(vPos);		
	}
	

}

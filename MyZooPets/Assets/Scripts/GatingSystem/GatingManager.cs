﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// GatingManager
// This manager is in charge of behavior related to
// the smoke monster and blocking access for the
// player.
//---------------------------------------------------

public class GatingManager : Singleton<GatingManager> {
	// area that this manager is in
	public string strArea;
	public string GetArea() {
		return strArea;	
	}
	
	// starting location for the gates -- might differ from area to area
	public Vector3 vStartingLoc;
	
	// the pan to movement script; it's got constants we need...
	public PanToMoveCamera scriptPan;
	
	// the player's pet animator -- maybe not the best place for this
	public PetAnimator scriptPetAnimator;
	public PetAnimator GetPlayerPetAnimator() {
		return scriptPetAnimator;	
	}
	
	// hash of active gates that the manager is currently managing
	private Hashtable hashActiveGates = new Hashtable();
	
	//---------------------------------------------------
	// Start()
	//---------------------------------------------------		
	void Start() {
		// listen for partition changing event
		scriptPan.OnPartitionChanged += EnteredRoom;
		
		// once the manager is loaded, spawn all appropriate, active gates
		Hashtable hashGates = DataGateLoader.GetAreaGates( strArea );
		foreach ( DictionaryEntry entry in hashGates ) {
			DataGate dataGate = (DataGate) entry.Value;
			
			// if the gate is activate, spawn the monster at an offset 
			bool bActive = DataManager.Instance.GameData.GatingProgress.IsGateActive( dataGate.GetGateID() );
			if ( bActive ) {
				int nStartingRoom = 0;										// room the player is starting in
				float fDistance = scriptPan.partitionOffset;				// the distance between each room
				int nDistance = dataGate.GetPartition() - nStartingRoom;	// the distance between the starting room and this gate's room
				float fOffset = nDistance * fDistance;						// offset of the gate
				
				// get the position of the gate by adding the offset to the starting location MOVE_DIR
				Vector3 vPos = vStartingLoc;
				vPos.x += fOffset;
				
				// create the gate at the location and set its id
				string strPrefab = dataGate.GetMonster().GetResourceKey();
				GameObject prefab = Resources.Load( strPrefab ) as GameObject;
				GameObject goGate = Instantiate( prefab, vPos, Quaternion.identity ) as GameObject;
				Gate scriptGate = goGate.GetComponent<Gate>();
				
				string strID = dataGate.GetGateID();
				scriptGate.Init( strID, dataGate.GetMonster() );
				
				// hash the gate based on the room, for easier access
				int nRoom = dataGate.GetPartition();
				hashActiveGates[nRoom] = scriptGate;
			}
		}
	}
	
	//---------------------------------------------------
	// GateCleared()
	// When the player clears a gate.
	//---------------------------------------------------		
	public void GateCleared() {
		// enable the player to do stuff in the room
		EnableUI();
	}
	
	//---------------------------------------------------
	// IsInGatedRoom()
	// Returns whether or not the player is currently
	// in a gated room.
	//---------------------------------------------------	
	public bool IsInGatedRoom() {
		int nRoom = scriptPan.currentPartition;
		bool bGated = DataGateLoader.HasActiveGate( strArea, nRoom );
		
		return bGated;
	}
	
	//---------------------------------------------------
	// HasActiveGate()
	// Returns if the incoming partition has a gate in it.
	// Note this assumes the area that this gating manager
	// is in.
	//---------------------------------------------------		
	public bool HasActiveGate( int nPartition ) {
		bool bHasGate = DataGateLoader.HasActiveGate( strArea, nPartition );
		return bHasGate;
	}
	
	//---------------------------------------------------
	// EnteredRoom()
	// When the player enters a room.
	// NOTE: Currently, exiting a gated room into another
	// gated room is not by design, and also not supported.	
	//---------------------------------------------------	
	public void EnteredRoom( object sender, PartitionChangedArgs args ) {
		int nLeaving = args.nOld;
		int nEntering = args.nNew;
		
		bool bGateLeaving = DataGateLoader.HasActiveGate( strArea, nLeaving );
		bool bGateEntering = DataGateLoader.HasActiveGate( strArea, nEntering );
		
		if ( bGateEntering ) {
			// if the player is entering a gated room, hide some ui and lock the click manager
			List<ClickLockExceptions> listExceptions = new List<ClickLockExceptions>();
			listExceptions.Add( ClickLockExceptions.Moving );
			ClickManager.Instance.ClickLock( listExceptions );
			NavigationUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
			
			// let the gate know that the player has entered the room
			Gate gate = (Gate) hashActiveGates[nEntering];
			gate.GreetPlayer();
			
			// also, move the player to a specific location
			MovePlayer( nEntering );
			
			// we neeed to listen to when the player is done moving to handle other gate related stuff
			ListenForMovementFinished( true );
		}
		else {
			// if the pet is leaving a gated room, destroy the fire UI and stop listening for a callback
			ListenForMovementFinished( false );
		}
		
		// if they are entering a non-gated room from a gated room, show that ui and unlock click manager
		if ( bGateLeaving && !bGateEntering ) 
			EnableUI();

	}
	
	//---------------------------------------------------
	// ListenForMovementFinished()
	// Subscribes/unsubscribes to pet movemvent callback.
	//---------------------------------------------------	
	private void ListenForMovementFinished( bool bListen ) {
		if ( bListen )
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
	private void PetReachedDest( object sender, EventArgs args ) {
		// if the pet is happy and healthy, add the fire button
		PetHealthStates eState = DataManager.Instance.GameData.Stats.GetHealthState();
		PetMoods eMood = DataManager.Instance.GameData.Stats.GetMoodState();
		if ( eState == PetHealthStates.Healthy && eMood == PetMoods.Happy ) 
			ShowFireButton();
		else {
			// otherwise, we want to show the tutorial explaining why the fire button isn't there (if it hasn't been shown)	
		}
		
		// regardless, stop listening for the callback now that we've received it
		ListenForMovementFinished( false );
	}
	
	//---------------------------------------------------
	// ShowFireButton()
	// Shows the fire button to attack the gate.
	//---------------------------------------------------		
	private void ShowFireButton() {
		// the pet has reached its destination (in front of the monster) so show the fire UI
		GameObject resourceFireButton = Resources.Load( "FireButton" ) as GameObject;
		GameObject goFireButton = LgNGUITools.AddChildWithPosition( GameObject.Find("Anchor-Center"), resourceFireButton );	
		
		// get the gate in this room
		Gate gate = (Gate) hashActiveGates[scriptPan.currentPartition];
		if ( gate ) {
			ButtonMonster script = goFireButton.GetComponent<ButtonMonster>();
			script.SetGate( gate );
		}
		else
			Debug.Log("Destination callback being called for non gated room");		
	}
	
	//---------------------------------------------------
	// EnableUI()
	// Enables the UI for the player that had previously
	// been locked.
	//---------------------------------------------------	
	private void EnableUI() {
		ClickManager.Instance.ReleaseClickLock();
		NavigationUIManager.Instance.ShowPanel();
		EditDecosUIManager.Instance.ShowNavButton();		
	}
	
	//---------------------------------------------------
	// MovePlayer()
	// Moves the player to the appropriate location in
	// relation to the gate in the incoming room.
	//---------------------------------------------------	
	private void MovePlayer( int nRoom ) {
		// then get the id of the gate and get that gate object from our list of active gates
		Gate gate = (Gate) hashActiveGates[nRoom];
		
		// get the position the player should approach
		Vector3 vPos = gate.GetPlayerPosition();
		
		// phew...now tell the player to move
		PetMovement.Instance.MovePet( vPos );		
	}
	
	//---------------------------------------------------
	// CanEnterRoom()
	// Returns whether the player can enter the incoming
	// room from the incoming direction.
	//---------------------------------------------------	
	public bool CanEnterRoom( int nCurrentRoom, RoomDirection eSwipeDirection ) {
		// early out if click manager is tweening
		if ( ClickManager.Instance.IsTweeningUI() )
			return false;
		
		// start off optimistic
		bool bOK = true;
		
		// if there is an active gate in this room, check to see if it is blocking the direction the player is trying to go in
		DataGate dataGate = DataGateLoader.GetData( strArea, nCurrentRoom );
		if ( dataGate != null && DataManager.Instance.GameData.GatingProgress.IsGateActive( dataGate.GetGateID() ) && dataGate.DoesBlock( eSwipeDirection ) )
			bOK = false;
		
		return bOK; 
	}
}

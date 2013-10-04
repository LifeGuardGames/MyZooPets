using UnityEngine;
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
		// once the manager is loaded, spawn all appropriate, active gates
		Hashtable hashGates = DataGateLoader.GetAreaGates( strArea );
		foreach ( DictionaryEntry entry in hashGates ) {
			DataGate dataGate = (DataGate) entry.Value;
			
			// if the gate is activate, spawn the monster at an offset 
			bool bActive = DataManager.Instance.GameData.GatingProgress.IsGateActive( dataGate.GetGateID() );
			if ( bActive ) {
				int nStartingRoom = scriptPan.currentPartition;				// room the player is starting in
				float fDistance = scriptPan.partitionOffset;				// the distance between each room
				int nDistance = dataGate.GetPartition() - nStartingRoom;	// the distance between the starting room and this gate's room
				float fOffset = nDistance * fDistance;						// offset of the gate
				
				// get the position of the gate by adding the offset to the starting location
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
	// EnteredRoom()
	// When the player enters a room.
	//---------------------------------------------------	
	public void EnteredRoom( int nLeaving, int nEntering ) {
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
		}
		else if ( bGateLeaving && !bGateEntering ) {
			// if they are entering a non-gated room from a gated room, show that ui and unlock click manager
			EnableUI();
		}
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
		// first get the data
		DataGate data = DataGateLoader.GetData( strArea, nRoom );
		
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
		// start off optimistic
		bool bOK = true;
		
		// if there is an active gate in this room, check to see if it is blocking the direction the player is trying to go in
		DataGate dataGate = DataGateLoader.GetData( strArea, nCurrentRoom );
		if ( dataGate != null && DataManager.Instance.GameData.GatingProgress.IsGateActive( dataGate.GetGateID() ) && dataGate.DoesBlock( eSwipeDirection ) )
			bOK = false;
		
		return bOK; 
	}
}

using UnityEngine;
using System.Collections;

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
	
	//---------------------------------------------------
	// EnteredRoom()
	// When the player enters a room.
	//---------------------------------------------------	
	public void EnteredRoom( int nRoom ) {
		
	}
	
	//---------------------------------------------------
	// CanEnterRoom()
	// Returns whether the player can enter the incoming
	// room.
	//---------------------------------------------------	
	public bool CanEnterRoom( int nCurrentRoom, RoomDirection eSwipeDirection ) {
		// start off optimistic
		bool bOK = true;
		
		// if there is an active gate in this room, check to see if it is blocking the direction the player is trying to go in
		DataGate dataGate = DataGateLoader.GetData( strArea, nCurrentRoom );
		if ( dataGate != null && DataManager.Instance.GatingProgress.IsGateActive( dataGate.GetGateID() ) && dataGate.DoesBlock( eSwipeDirection ) )
			bOK = false;
		
		return bOK; 
	}
}

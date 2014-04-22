using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DataGate
// Individual piece of gate data as loaded from xml.
// This is considered immutable.
//---------------------------------------------------

public class DataGate {
	
	private string strID; // id of the gate
	private string strArea; // location of the gate
	private int nPartition; // partition id of the gate
	private string strMonsterID; // id of the monster at this gate
	private RoomDirection eSwipeDirection; // the swipe direction that this monster is blocking
	private string[] arrayUnlocks; // list of wellapad unlocks removing this makes available
	private bool bRecurring; // is this gate recurring? i.e. comes back to life after a set amount of time
	private string strItemBoxID; // item box id this gate leaves behind once destroyed

	public string GetGateID() {
		return strID;	
	}
	
	public string GetArea() {
		return strArea;	
	}
	
	public int GetPartition() {
		return nPartition;	
	}
	
	public DataMonster GetMonster() {
		return DataMonsterLoader.GetData( strMonsterID );	
	}
	
	public bool DoesBlock( RoomDirection eSwipeDirection ) {
		return this.eSwipeDirection == eSwipeDirection;
	}
	
	public string[] GetTaskUnlocks() {
		return arrayUnlocks;	
	}
	
	public bool IsRecurring() {
		return bRecurring;	
	}
	
	public string GetItemBoxID() {
		return strItemBoxID;	
	}
	
	public DataGate( string id, Hashtable hashData, Hashtable hashElements, string strError ) {
		strID = id;	

		// get location
		strArea = HashUtils.GetHashValue<string>( hashData, "Location", null, strError );
		
		// get partition
		nPartition = int.Parse( HashUtils.GetHashValue<string>( hashData, "Partition", "0", strError ) );
		
		// get monster
		strMonsterID = HashUtils.GetHashValue<string>( hashData, "Monster", null, strError );
		
		// get whether or not the gate recurs
		bRecurring = bool.Parse( HashUtils.GetHashValue<string>( hashData, "Recurring", "false" ) );
		
		// get an item box id gate leaves behind (if any)
		strItemBoxID = XMLUtils.GetString( hashElements["ItemBox"] as IXMLNode );
		
		// get the direction the gate is blocking
		eSwipeDirection = (RoomDirection) System.Enum.Parse( typeof(RoomDirection), 
														HashUtils.GetHashValue<string>( hashData, "Blocking", null, strError ) );
		
		// get list of wellapad unlocks
		string strUnlocks = XMLUtils.GetString( hashElements["TaskUnlocks"] as IXMLNode );
		arrayUnlocks = strUnlocks.Split( ","[0] );
		
		//Debug.Log("Loading gate " + strID + " in loc " + strArea + " in partition " + nPartition + " and monster " + strMonsterID);
	}
}

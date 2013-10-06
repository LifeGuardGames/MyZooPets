using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DataGate
// Individual piece of gate data as loaded from xml.
// This is considered immutable.
//---------------------------------------------------

public class DataGate {
	
	// id of the gate
	private string strID;
	public string GetGateID() {
		return strID;	
	}
	
	// location of the gate
	private string strArea;
	public string GetArea() {
		return strArea;	
	}
	
	// partition id of the gate
	private int nPartition;
	public int GetPartition() {
		return nPartition;	
	}
	
	// id of the monster at this gate
	private string strMonsterID;
	public DataMonster GetMonster() {
		return DataMonsterLoader.GetData( strMonsterID );	
	}
	
	// the swipe direction that this monster is blocking
	private RoomDirection eSwipeDirection;
	public bool DoesBlock( RoomDirection eSwipeDirection ) {
		return this.eSwipeDirection == eSwipeDirection;
	}
	
	public DataGate( string id, Hashtable hashData, string strError ) {
		strID = id;	

		// get location
		strArea = HashUtils.GetHashValue<string>( hashData, "Location", null, strError );
		
		// get partition
		nPartition = int.Parse( HashUtils.GetHashValue<string>( hashData, "Partition", "0", strError ) );
		
		// get monster
		strMonsterID = HashUtils.GetHashValue<string>( hashData, "Monster", null, strError );
		
		// get the direction the gate is blocking
		eSwipeDirection = (RoomDirection) System.Enum.Parse( typeof(RoomDirection), 
														HashUtils.GetHashValue<string>( hashData, "Blocking", null, strError ) );
		
		//Debug.Log("Loading gate " + strID + " in loc " + strArea + " in partition " + nPartition + " and monster " + strMonsterID);
	}
}

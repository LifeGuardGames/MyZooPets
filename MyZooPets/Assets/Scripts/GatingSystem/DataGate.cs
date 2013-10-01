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
	private string strLocation;
	public string GetLocation() {
		return strLocation;	
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
	
	public DataGate( string id, Hashtable hashData, string strError ) {
		strID = id;	

		// get location
		strLocation = HashUtils.GetHashValue<string>( hashData, "Location", null, strError );
		
		// get partition
		nPartition = int.Parse( HashUtils.GetHashValue<string>( hashData, "Partition", "0", strError ) );
		
		// get monster
		strMonsterID = HashUtils.GetHashValue<string>( hashData, "Monster", null, strError );
		
		//Debug.Log("Loading gate " + strID + " in loc " + strLocation + " in partition " + nPartition + " and monster " + strMonsterID);
	}
}

using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DataMonster
// Individual piece of monster data as loaded from xml.
// This is considered immutable.
//---------------------------------------------------

public class DataMonster {

	private string strID; // id of the monster
	private int nHP; // hp of the monster
	private string strResourceKey; // key for the monster, used to buid resources

	public string GetMonsterID() {
		return strID;	
	}
	
	public int GetMonsterHealth() {
		return nHP;	
	}
	
	public string GetResourceKey() {
		return strResourceKey;	
	}
	
	public DataMonster( string id, Hashtable hashData, string strError ) {
		strID = id;	

		// get monster hp
		nHP = int.Parse( HashUtils.GetHashValue<string>( hashData, "Health", "100", strError ) );
		
		// get prefab that this monster spawns
		strResourceKey = HashUtils.GetHashValue<string>( hashData, "Key", "SmokeMonster", strError );		
		
		//Debug.Log("Loading monster " + strID + " with hp " + nHP);
	}
}

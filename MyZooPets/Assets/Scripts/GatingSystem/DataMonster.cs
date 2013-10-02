using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DataMonster
// Individual piece of monster data as loaded from xml.
// This is considered immutable.
//---------------------------------------------------

public class DataMonster {

	// id of the monster
	private string strID;
	public string GetMonsterID() {
		return strID;	
	}
	
	// hp of the monster
	private int nHP;
	public int GetMonsterHealth() {
		return nHP;	
	}
	
	public DataMonster( string id, Hashtable hashData, string strError ) {
		strID = id;	

		// get monster hp
		nHP = int.Parse( HashUtils.GetHashValue<string>( hashData, "Health", "100", strError ) );
		
		//Debug.Log("Loading monster " + strID + " with hp " + nHP);
	}
}

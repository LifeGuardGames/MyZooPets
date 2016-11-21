using UnityEngine;
using System.Collections;

//---------------------------------------------------
// NinjaDataEntry
// Within a group, an entry is a single type of 
// spawning of triggers.
//---------------------------------------------------

public class NinjaDataEntry {
	// group type
	private NinjaPatterns ePattern;
	public NinjaPatterns GetPattern() {
		return ePattern;
	}
	
	// # of normal triggers
	private int nTriggers;
	public int GetTriggers() {
		return nTriggers;
	}
	
	// # of bombs in the entry
	private int nBombs;
	public int GetBombs() {
		return nBombs;
	}

	// # of powerUps in the entry
	private int nPowUp;
	public int GetPowUp() {
		return nPowUp;
	}
	
	// time this entry should go off
	private float fTime;
	public float GetTime() {
		return fTime;
	}

	public NinjaDataEntry( Hashtable hashAttr, string strError ) {
		// pattern this entry spawns
		string strPattern = HashUtils.GetHashValue<string>( hashAttr, "Pattern", "Separate", strError );
		ePattern = (NinjaPatterns) System.Enum.Parse( typeof( NinjaPatterns ), strPattern );
		
		// number of objects in this entry
		nTriggers = int.Parse( HashUtils.GetHashValue<string>( hashAttr, "Triggers", "0" ) );
		
		// # of bombs in the entry
		nBombs = int.Parse( HashUtils.GetHashValue<string>( hashAttr, "Bombs", "0" ) );

		// # of bombs in the entry
		nPowUp = int.Parse( HashUtils.GetHashValue<string>( hashAttr, "PowerUp", "0" ) );

		// time this entry should be spawned
		fTime = float.Parse( HashUtils.GetHashValue<string>( hashAttr, "Time", "0", strError ) );
	}
}

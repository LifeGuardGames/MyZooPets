using UnityEngine;
using System.Collections;

//---------------------------------------------------
// XpRewardBonus
// Immutable piece of data for xp rewards. A bonus
// is stored within a range.  A range may have any
// number (including 0) bonuses. Any number of bonuses
// within a range may be met to increase the xp
// reward of that range.
//---------------------------------------------------

public class XpRewardBonus {
	// key for this bonus, used for determining incoming hash data (NOT unique)
	private string strKey;
	
	// amount required for this bonus to be valid
	private float fAmount;
	
	// xp bonus granted
	private float fBonus;
	
	public XpRewardBonus( IXMLNode node, string strError ) {
		// get hash of data
		Hashtable hashData = XMLUtils.GetAttributes(node);
		
		strKey = HashUtils.GetHashValue<string>( hashData, "Key", "Score", strError );
		
		fAmount = float.Parse( HashUtils.GetHashValue<string>( hashData, "Amount", "1000", strError ) );
		
		fBonus = float.Parse( HashUtils.GetHashValue<string>( hashData, "Bonus", "0", strError ) );
		
		// to save me from myself...
		if ( fBonus >= 1 ) {
			Debug.Log(strError + "Bonus is >= 1...I really don't think you meant this...making it .01 instead");
			fBonus = .01f;
		}
	}
	
	//---------------------------------------------------
	// GetBonus()
	// Given the incoming bonus data, returns what kind
	// of bonus should be awarded (will be 0 if the data
	// does not meet the bonus).
	//---------------------------------------------------	
	public float GetBonus( Hashtable hashBonusData ) {
		float fBonus = 0;
		
		// loop through the data and see if anything matches
		foreach ( DictionaryEntry entry in hashBonusData ) {
			string strKey = (string) entry.Key;
			
			// if the keys match, dig deeper
			if ( strKey == this.strKey ) {
				string strAmount = (string) entry.Value;				
				float fAmount =	float.Parse( strAmount );
				
				// if the amount meets or exceeds the amount for this bonus...huzzah!
				if ( fAmount >= this.fAmount )
					fBonus += this.fBonus;
			}
		}
		
		return fBonus;
	}
}

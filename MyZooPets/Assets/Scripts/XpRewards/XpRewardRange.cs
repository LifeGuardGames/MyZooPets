using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//---------------------------------------------------
// XpRewardRange
// An xp reward range is a piece of immutable data in
// an xp reward.  Within an xp reward, a range acts as
// the highest level gate keeper, looking at the pet's
// level to see how the reward should be structured.
//---------------------------------------------------

public class XpRewardRange {
	// min and max levels of this range
	private int nMinLevel;
	public int GetMinLevel() {
		return nMinLevel;
	}
	
	private int nMaxLevel;
	public int GetMaxLevel() {
		return nMaxLevel;	
	}
	
	// base reward for this range
	private float fReward;
	
	// list of bonuses in this range
	private List<XpRewardBonus> listBonuses = new List<XpRewardBonus>();
	
	public XpRewardRange( IXMLNode node, string strError ) {
		// create list of bonuses for incoming node
		List<IXMLNode> listBonusNodes = XMLUtils.GetChildrenList(node);
		
		for ( int i = 0; i < listBonusNodes.Count; ++i ) {
			XpRewardBonus bonus = new XpRewardBonus( listBonusNodes[i], strError );
			listBonuses.Add( bonus );
		}
		
		// get hash of data
		Hashtable hashData = XMLUtils.GetAttributes(node);	
		
		// get the min and max level
		nMinLevel = int.Parse( HashUtils.GetHashValue<string>( hashData, "Min", "1", strError ) );
		nMaxLevel = int.Parse( HashUtils.GetHashValue<string>( hashData, "Max", Enum.GetNames(typeof(Level)).Length.ToString() ) );
	
		// get the base reward
		fReward = float.Parse( HashUtils.GetHashValue<string>( hashData, "Reward", "0", strError ) );
	}
	
	//---------------------------------------------------
	// IsInBetween()
	// A little helper function to see if the incoming 
	// level is within this range's min/max
	//---------------------------------------------------	
	public bool IsInBetween( int nLevel ) {
		bool bInBetween = false;
		
		int nMin = GetMinLevel();
		int nMax = GetMaxLevel();
		
		if ( nLevel == nMin || nLevel == nMax || ( nLevel > nMin && nLevel < nMax ) )
			bInBetween = true;
		
		return bInBetween;
	}	

	//---------------------------------------------------
	// GetPercentage()
	// Returns the % xp to be rewarded by this range
	// given the incoming bonus data.
	//---------------------------------------------------		
	public float GetPercentage( Hashtable hashBonusData ) {
		// this range rewards a base amount of xp even without any bonuses
		float fXP = fReward;
		
		// now loop through the bonuses and see if any are met
		for ( int i = 0; i < listBonuses.Count; ++i ) {
			XpRewardBonus bonus = listBonuses[i];
			fXP += bonus.GetBonus( hashBonusData );
		}
		
		return fXP;
	}
}

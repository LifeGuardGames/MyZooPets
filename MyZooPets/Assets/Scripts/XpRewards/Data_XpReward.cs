using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// Data_XpReward
// Immutable data for an xp reward.
//---------------------------------------------------

public class Data_XpReward{
	// id of this reward
	private string strID;

	public string GetID(){
		return strID;	
	}
	
	// list of ranges within this reward
	private List<XpRewardRange> listRanges = new List<XpRewardRange>();
	
	public Data_XpReward(string strID, IXMLNode node, string strError){
		this.strID = strID;
		
		// create a list of ranges from the incoming node
		List<IXMLNode> listRangeNodes = XMLUtils.GetChildrenList(node);
		
		for(int i = 0; i < listRangeNodes.Count; ++i){
			XpRewardRange range = new XpRewardRange(listRangeNodes[i], strError);	
			
			// check the range to make sure it is legit before adding it to our list
			if(CheckRange(strError, range))
				listRanges.Add(range);
		}
	}

	//---------------------------------------------------
	// CalculateXP()
	// Based on the incoming hash bonus data, this function
	// will calculate how much xp the pet should get.
	//---------------------------------------------------
	public int CalculateXP(Hashtable hashBonusData){
		// if the pet is already at max level, then just return 0
		if(LevelLogic.Instance.IsAtMaxLevel())
			return 0;		
		
		// first, we want to get the pet's level
		int nLevel = (int)LevelLogic.Instance.CurrentLevel;		
		
		// now, get the range for that level
		XpRewardRange range = GetRange(nLevel);
		
		// if the range exists, the get % of xp earned
		float fXP = 0;
		if(range != null) 
			fXP = range.GetPercentage(hashBonusData);
		
		// now get the total xp a pet of this level requires
		int nTotalXP = 0;
		ImmutableDataPetLevel dataLevel = DataLoaderPetLevels.GetLevel(nLevel + 1);
		if(dataLevel != null)
			nTotalXP = dataLevel.LevelUpCondition;
		
		// multiple this total by percentage to be awarded -- this is our total xp
		int nXP = Mathf.RoundToInt(nTotalXP * fXP);
		
		//Debug.Log("Awarding " + nXP + " xp(" + fXP + " of " + nTotalXP +")");
		
		return nXP;
	}
	
	//---------------------------------------------------
	// GetRange()
	// Returns the range data for the incoming level.
	//---------------------------------------------------	
	private XpRewardRange GetRange(int nLevel){
		// the correct range given the incoming level
		XpRewardRange rangeCorrect = null;
		
		// loop through and search for the correct range...yeah I could break out of it when it's found, but I always forget how/when break works
		for(int i = 0; i < listRanges.Count; ++i){
			XpRewardRange range = listRanges[i];
			if(range.IsInBetween(nLevel))
				rangeCorrect = range;
		}
		
		if(rangeCorrect == null)
			Debug.LogError("Attempted to get range data for " + GetID() + " for level " + nLevel + " but it did not exist...");
		
		return rangeCorrect;
	}
	
	//---------------------------------------------------
	// CheckRange()
	// Checks the incoming range to make sure it doesn't
	// overlap with the already loaded ranges.
	//---------------------------------------------------	
	private bool CheckRange(string strError, XpRewardRange rangeNew){
		bool bOK = true;
		
		// min and max levels of new range
		int nMinNew = rangeNew.GetMinLevel();
		int nMaxNew = rangeNew.GetMaxLevel();
		
		// if the min is below 1 or greater than the max, something is wrong
		if(nMinNew < 1 || nMinNew > nMaxNew)
			bOK = false;
		
		// loop through all the existing ranges and see if the min or max of this new range fall between them
		for(int i = 0; i < listRanges.Count; ++i){
			XpRewardRange range = listRanges[i];
			
			if(range.IsInBetween(nMinNew) || range.IsInBetween(nMaxNew))
				bOK = false;
		}
		
		if(!bOK)
			Debug.LogError(strError + "Something going wrong in range data");
		
		return bOK;
	}
}

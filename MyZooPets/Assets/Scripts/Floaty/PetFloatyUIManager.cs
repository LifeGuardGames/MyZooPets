using UnityEngine;
using System.Collections;

public class PetFloatyUIManager : Singleton<PetFloatyUIManager> {
    public GameObject petFloatyPosition;
	
	//-------------------------------------------------------
    // CreateStatsFloaty()
    // Use the FloatyUtil class to spawn the floaty image text
    // on top of the pet's head, FOR STATS ONLY!
    //-------------------------------------------------------
	public void CreateStatsFloaty(int deltaPoints, int deltaHealth, int deltaMood, int deltaStars){
		if(petFloatyPosition != null && petFloatyPosition.activeInHierarchy){

			Debug.LogWarning("FLOATY SPAWN HERE");
			/*
		Hashtable option = new Hashtable();

		option.Add("parent", petFloatyPosition);

		if(deltaPoints != 0){
			string strDeltaPoints = (deltaPoints > 0) ? "+" + deltaPoints : deltaPoints.ToString();
			option.Add("spritePoints", "iconStar");
			option.Add("deltaPoints", strDeltaPoints);
		}
		if(deltaHealth != 0){
			string strDeltaHealth = (deltaHealth > 0) ? "+" + deltaHealth : deltaHealth.ToString();
			option.Add("spriteHealth", "iconHeart");
			option.Add("deltaHealth", strDeltaHealth);
		}
		if(deltaMood != 0){
			string strDeltaMood = (deltaMood > 0) ? "+" + deltaMood : deltaMood.ToString();
			option.Add("spriteHunger", "iconHunger");
			option.Add("deltaMood", strDeltaMood);
		}
		if(deltaStars != 0){
			string strDeltaStars = (deltaStars > 0) ? "+" + deltaStars : deltaStars.ToString();
			option.Add("spriteStars", "iconCoin");
			option.Add("deltaStars", strDeltaStars);
		}

		FloatyUtil.SpawnFloatyStats(option);
		*/
	}
}

//-------------------------------------------------------
// CreateFloaty()
// Use the FloatyUtil class to spawn the floaty image text
// on top of the pet's head, general single picture
//-------------------------------------------------------
private void CreateFloaty(int deltaValue, string spriteName) {
	string strDeltaValue = "";

	if(deltaValue > 0) {
		strDeltaValue = "+" + deltaValue;
	}
	else {
		strDeltaValue = "" + deltaValue;
	}

	Debug.LogWarning("FLOATY SPAWN HERE");
	/*
	Hashtable option = new Hashtable();
	option.Add("parent", petFloatyPosition);
	option.Add("text", strDeltaValue);
	option.Add("spriteName", spriteName);

	FloatyUtil.SpawnFloatyImageText(option);
	*/
		}
	}

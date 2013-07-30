using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BadgeGUI : MonoBehaviour {

	public List<GameObject> LevelList = new List<GameObject>();
	public UIAtlas badgeAtlas;

	// Use this for initialization
	void Start (){

		// Temprary check for badges
		if(LevelList.Count < BadgeLogic.Instance.badges.Count){
			Debug.LogError("Temporary implementation badges has wrong size");
		}

		foreach(Badge badge in BadgeLogic.Instance.badges){
			// Ghetto parse for badge level
			//print(parseLevelsBadge(badge.name));
			int levelNumber = parseLevelsBadge(badge.name);
			if(badge.IsAwarded){
				LevelList[levelNumber].GetComponent<UISprite>().spriteName = "badgeLevel" + levelNumber;

				// Display the tier if applicable
				if(badge.Tier != BadgeTier.Null || badge.Tier != null){
					UISprite tier = NGUITools.AddSprite(LevelList[levelNumber], badgeAtlas, "badgeAddon" + badge.Tier.ToString());
					tier.transform.localScale = new Vector3(183f, 233f, 1f);
					tier.transform.localPosition = new Vector3(50f, 50f, 0);
				}
			}
		}
	}

	// Parses the level of the badge
	private int parseLevelsBadge(string badgeName){
		return int.Parse(badgeName.Substring(badgeName.IndexOf(" ") + 1));
	}
}

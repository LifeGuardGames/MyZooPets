using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BadgeGUI : MonoBehaviour {

	//======================Event=============================
    public static event EventHandler<EventArgs> OnBadgeBoardClosed;
    //=======================================================

	public GUISkin defaultSkin;
	public Texture2D backButton;
	public GUIStyle blankButtonStyle;

	public GameObject badgeBoard;
	private bool isActive = false;

	public List<GameObject> LevelList = new List<GameObject>(); //list of badge gameobjects
																//index of this list correlates to the index
																//from BadgeLogic.Instance.badges
	public UIAtlas badgeAtlas;

	// Use this for initialization
	void Start (){

		// Temprary check for badges
		if(LevelList.Count != BadgeLogic.Instance.badges.Count){
			Debug.LogError("Temporary implementation badges has wrong size");
		}

		foreach(Badge badge in BadgeLogic.Instance.LevelBadges){
			int levelNumber = badge.ID;
			if(badge.IsAwarded){
				LevelList[levelNumber].transform.Find("badgeSprite").GetComponent<UISprite>().spriteName = "badgeLevel" + levelNumber;
					
				// Display the tier if applicable
				if(badge.Tier != BadgeTier.Null){
					 UISprite tier = NGUITools.AddSprite(LevelList[levelNumber], badgeAtlas, "badgeAddon" + badge.Tier.ToString());

					//TO-DO s, scale incorrect
					 tier.transform.localScale = new Vector3(34f, 50f, 1f);
					 tier.transform.localPosition = new Vector3(40f, -40f, 0);
				}
			}
		}
	}

	void OnGUI(){
		GUI.skin = defaultSkin;
		if(isActive && !ClickManager.isClickLocked){ // checking isClickLocked because trophy shelf back button should not be clickable if there is a notification
        	if(GUI.Button(new Rect(10, 10, backButton.width, backButton.height), backButton, blankButtonStyle)){
        		if(OnBadgeBoardClosed != null){
        			OnBadgeBoardClosed (this, EventArgs.Empty);
    			}else{
    				Debug.LogError("OnBadgeBoardClosed is null");
    			}
				isActive = false;
				badgeBoard.collider.enabled = true;
			}
		}
	}

	// Parses the level of the badge
	private int parseLevelsBadge(string badgeName){
		return int.Parse(badgeName.Substring(badgeName.IndexOf(" ") + 1));
	}

	// When a badge is clicked. zoom in on the badge and display detail information
	public void BadgeClicked(GameObject go){
		Debug.Log(go.name);
	}

	public void BadgeBoardClicked(){
		if(!isActive){
			isActive = true;
			badgeBoard.collider.enabled = false;
		}
	}
}

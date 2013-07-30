using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BadgeGUI : MonoBehaviour {

	//======================Event=============================
    public static event EventHandler<EventArgs> OnBadgeBoardClosed;
    //=======================================================

	public GameObject cameraMoveObject;
	private CameraMove cameraMove;

	public GUISkin defaultSkin;
	public Texture2D backButton;
	public GUIStyle blankButtonStyle;

	public GameObject badgeBoard;
	private bool isActive = false;

	public List<GameObject> LevelList = new List<GameObject>();
	public UIAtlas badgeAtlas;

	// Use this for initialization
	void Start (){
		cameraMove = cameraMoveObject.GetComponent<CameraMove>();

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

	void OnGUI(){
		GUI.skin = defaultSkin;
		if(isActive && !ClickManager.isClickLocked){ // checking isClickLocked because trophy shelf back button should not be clickable if there is a notification
        	if(GUI.Button(new Rect(10, 10, backButton.width, backButton.height), backButton, blankButtonStyle)){
        		if(OnBadgeBoardClosed != null) OnBadgeBoardClosed (this, EventArgs.Empty);
				isActive = false;
				badgeBoard.collider.enabled = true;
			}
		}
	}

	// Parses the level of the badge
	private int parseLevelsBadge(string badgeName){
		return int.Parse(badgeName.Substring(badgeName.IndexOf(" ") + 1));
	}

	public void BadgeClicked(GameObject go){
		Debug.Log(go.name);
	}

	public void BadgeBoardClicked(){
		if(!isActive){
			isActive = true;
			cameraMove.ZoomToggle(ZoomItem.BadgeBoard);
			badgeBoard.collider.enabled = false;
		}
	}
}

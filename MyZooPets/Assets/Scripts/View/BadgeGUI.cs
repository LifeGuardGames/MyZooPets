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
	public GameObject descriptionObject;
	public GameObject badgeGUISpawnBase;	// Parent to clone badges and zoom out

	private bool isActive = false;

	private GameObject bgPanel;

	public List<GameObject> LevelList = new List<GameObject>(); //list of badge gameobjects
																//index of this list correlates to the index
																//from BadgeLogic.Instance.LevelBadges
	public UIAtlas badgeLevelAtlas1;
	public UIAtlas badgeLevelAtlas2;
	public UIAtlas badgeLevelAtlas3;
	public UIAtlas badgeCommonAtlas;	// Holds all the low-res badges and common objects
	public UIAtlas badgeExtraAtlas;		// Holds the tier badges for zoomed display

	//TODO-s refactor this dam script!!!!
	// Use this for initialization
	void Start (){
		foreach(Badge badge in BadgeLogic.Instance.LevelBadges){
			int levelNumber = badge.ID;
			BadgeMetadata meta = LevelList[levelNumber].AddComponent<BadgeMetadata>();

			if(badge.ID <= 7){
				meta.atlasName = "BadgeLevelAtlas1";
			}
			else if(badge.ID <= 15){
				meta.atlasName = "BadgeLevelAtlas2";
			}
			else{
				meta.atlasName = "BadgeLevelAtlas3";
			}

			meta.title = badge.name;
			meta.description = badge.description;

			//Debug.Log(meta.title + " " + meta.description);

			if(badge.IsAwarded){
				LevelList[levelNumber].transform.Find("badgeSprite").GetComponent<UISprite>().spriteName = "badgeLevel" + levelNumber;

				// Display the tier if applicable
				if(badge.Tier != BadgeTier.Null){
					UISprite tier = NGUITools.AddSprite(LevelList[levelNumber], badgeCommonAtlas, "badgeAddon" + badge.Tier.ToString());
					tier.gameObject.name = "tier";

					//TO-DO s, scale incorrect
					tier.transform.localScale = new Vector3(39f, 50f, 1f);
					tier.transform.localPosition = new Vector3(40f, -40f, 0);
				}
			}
			else{
				// Activate the dark version of the badge
				LevelList[levelNumber].transform.Find("badgeSprite").GetComponent<UISprite>().spriteName = "badgeLevel" + levelNumber + "Dark";
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

	public void disableBackButton(){
		isActive = false;
	}

	// // Parses the level of the badge
	// private int parseLevelsBadge(string badgeName){
	// 	return int.Parse(badgeName.Substring(badgeName.IndexOf(" ") + 1));
	// }

	// When a badge is clicked. Zoom in on the badge and display detail information
	public void BadgeClicked(GameObject go){
		disableBackButton();

		BadgeMetadata meta = go.GetComponent<BadgeMetadata>();

		if(meta != null){

			// Fine the appropriate atlas // TODO-s load resource dynamically?
			UIAtlas activeAtlas = null;
			if(meta.atlasName == badgeLevelAtlas1.name){
				activeAtlas = badgeLevelAtlas1;
			}
			else if(meta.atlasName == badgeLevelAtlas2.name){
				activeAtlas = badgeLevelAtlas2;
			}
			else if(meta.atlasName == badgeLevelAtlas3.name){
				activeAtlas = badgeLevelAtlas3;
			}

			GameObject titleGO = GameObject.Find("Label_Title") as GameObject;

			UILabel titleLabel = titleGO.GetComponent<UILabel>();
			titleLabel.text = meta.title;
			Debug.Log(titleLabel.text);

			GameObject descriptionGO = GameObject.Find("Label_Description") as GameObject;
			UILabel descriptionLabel = descriptionGO.GetComponent<UILabel>();
			descriptionLabel.text = meta.description;

			// Spawn BG with collider
			// Parent object needs to be 0, 0px;
			UISprite bgSprite = NGUITools.AddSprite(descriptionObject, badgeCommonAtlas, "box30");
			bgSprite.type = UISprite.Type.Sliced;
			bgSprite.color = new Color(0f, 0f, 0f, 0.6f);
			bgSprite.depth = 50;
			bgPanel = bgSprite.gameObject;
			bgPanel.transform.localScale = new Vector3(3000f, 3000f, 1);
			bgPanel.transform.localPosition = new Vector3(0f, 0f ,0f);
			BoxCollider collider = bgPanel.AddComponent<BoxCollider>();
			collider.size = new Vector3(3000f, 3000f, 1f);
	
			// Spawn cloned badge
			GameObject spriteObject = GameObject.Find(go.name + "/badgeSprite");
			UISprite originalSprite = spriteObject.GetComponent<UISprite>();
	
			UISprite badgeSprite = NGUITools.AddSprite(badgeGUISpawnBase, activeAtlas, originalSprite.spriteName);
			Vector3 position = UIUtility.Instance.mainCameraWorld2Screen(go.transform.position);
			badgeSprite.transform.localPosition = new Vector3(position.x, position.y, -1f);
			badgeSprite.transform.localScale = new Vector3(100f, 100f, 0f);
			badgeSprite.depth = 103;
	
			LeanTween.moveLocal(badgeSprite.gameObject, new Vector3(400, 400f, -1f), 0.4f);
			LeanTween.scale(badgeSprite.gameObject, new Vector3(512f, 512, 0f), 0.4f);
	
	
	
			GameObject tierObject = GameObject.Find(go.name + "/tier");

			if(tierObject != null){
				UISprite originalTier = tierObject.GetComponent<UISprite>();
				// TODO refactor into function, used twice
				UISprite tierSprite = NGUITools.AddSprite(badgeGUISpawnBase, badgeExtraAtlas, originalTier.spriteName);
				tierSprite.transform.localPosition = new Vector3(position.x + 40f, position.y - 40f, -1.1f);
				tierSprite.transform.localScale = new Vector3(39f, 50f, 1f);
				tierSprite.depth = 104;
	
		//		LeanTween.delayedCall(badgeSprite.gameObject ,5.0f,"loadtips", new object[]{"onCompleteTarget", this} );
		//		LeanTween.delayedCall(tierSprite.gameObject ,5.0f,"loadtips", new object[]{"onCompleteTarget", this} );
				// TODO-s error thrown here...
				LeanTween.moveLocal(tierSprite.gameObject, new Vector3(600f, 200f, -1.1f), 0.4f);
				LeanTween.scale(tierSprite.gameObject, new Vector3(170f, 250, 0f), 0.4f);
			}
	
			OpenDescription();

		}
		else{
			Debug.LogError("No Metadata attached on badge");
		}
	}

	public void OpenDescription(){
		descriptionObject.GetComponent<MoveTweenToggle>().Show();
	}

	public void CloseDescription(){
		descriptionObject.GetComponent<MoveTweenToggle>().Hide();
		badgeGUISpawnBase.transform.DestroyChildren();

		Destroy(bgPanel);

		BadgeBoardClicked();
	}

	public void BadgeBoardClicked(){
		if(!isActive){
			isActive = true;
			badgeBoard.collider.enabled = false;
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BadgeGUI : MonoBehaviour {
	//======================Event=============================
    public static event EventHandler<EventArgs> OnBadgeBoardClosed;
    //=======================================================
	public GameObject backButtonPrefab;
	private GameObject backButtonReference;
	private bool isActive = false;

	public GameObject badgeBoard;
	public GameObject descriptionObject;
	public GameObject badgeGUISpawnBase;	// Parent to clone badges in (anchor-center) when zoomed in
	private GameObject bgPanel;

	// List of badge gameobjects
	public List<GameObject> LevelList = new List<GameObject>();	// Index of this list correlates to the index from BadgeLogic.Instance.LevelBadges

	public UIAtlas badgeCommonAtlas;		// Holds ALL the low-res badges and common objects
	public UIAtlas badgeExtraAtlas;			// Holds tier (gold/silver/bronze) medals for zoomed display

	// High Definition badges go here for closeup
	// NOTE: 512x512 px, zero padding to fit extra row
	public UIAtlas badgeLevelAtlas1;
	public UIAtlas badgeLevelAtlas2;
	public UIAtlas badgeLevelAtlas3;

	void Start(){
		BadgeLogic.OnNewBadgeAdded += UpdateLevelBadges;
		UpdateLevelBadges(null, EventArgs.Empty);
	}

	void OnDestroy(){
		BadgeLogic.OnNewBadgeAdded -= UpdateLevelBadges;
	}

	//Event Listener that updates the Level badges UI when new badges are added or
	//when badges UI are loaded for the first time
	private void UpdateLevelBadges(object senders, EventArgs arg){
		// Level Badges
		foreach(Badge badge in BadgeLogic.Instance.LevelBadges){
			int levelNumber = badge.ID;

			// Populate metadata script in object
			BadgeMetadata meta = LevelList[levelNumber].AddComponent<BadgeMetadata>();
			meta.title = badge.name;
			meta.description = badge.description;

			if(badge.ID <= 7){
				meta.atlasName = "BadgeLevelAtlas1";
			}
			else if(badge.ID <= 15){
				meta.atlasName = "BadgeLevelAtlas2";
			}
			else{
				meta.atlasName = "BadgeLevelAtlas3";
			}

			// Decide which sprite to use
			if(badge.IsAwarded){
				LevelList[levelNumber].transform.Find("badgeSprite").GetComponent<UISprite>().spriteName = "badgeLevel" + levelNumber;

				// Display the tier if applicable
				if(badge.Tier != BadgeTier.Null){
					UISprite tier = NGUITools.AddSprite(LevelList[levelNumber], badgeCommonAtlas, "badgeAddon" + badge.Tier.ToString());
					tier.gameObject.name = "tier";
					tier.transform.localScale = new Vector3(39f, 50f, 1f);
					tier.transform.localPosition = new Vector3(40f, -40f, 0);
				}
			}
			else{	// Dark version of the badge
				LevelList[levelNumber].transform.Find("badgeSprite").GetComponent<UISprite>().spriteName = "badgeLevel" + levelNumber + "Dark";
			}
		}
	}

	// When a badge is clicked. Zoom in on the badge and display detail information
	public void BadgeClicked(GameObject go){
		DisableBackButton();

		BadgeMetadata meta = go.GetComponent<BadgeMetadata>();

		if(meta != null){
			// Find the appropriate atlas TODO-s load resource dynamically?
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

			GameObject descriptionGO = GameObject.Find("Label_Description") as GameObject;
			UILabel descriptionLabel = descriptionGO.GetComponent<UILabel>();
			descriptionLabel.text = meta.description;

			// Spawn BG with collider
			// NOTE: Parent object needs to be 0, 0px, lower left corner;
			UISprite bgSprite = NGUITools.AddSprite(descriptionObject, badgeCommonAtlas, "box30");
			bgSprite.type = UISprite.Type.Sliced;
			bgSprite.color = new Color(0f, 0f, 0f, 0.6f);
			bgSprite.depth = 50;
			bgPanel = bgSprite.gameObject;	// Get reference to delete later
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
				UISprite tierSprite = NGUITools.AddSprite(badgeGUISpawnBase, badgeExtraAtlas, originalTier.spriteName);
				tierSprite.transform.localPosition = new Vector3(position.x + 40f, position.y - 40f, -1.1f);
				tierSprite.transform.localScale = new Vector3(39f, 50f, 1f);
				tierSprite.depth = 104;
	
				// TODO-s error thrown here...
				LeanTween.moveLocal(tierSprite.gameObject, new Vector3(600f, 200f, -1.1f), 0.4f);
				LeanTween.scale(tierSprite.gameObject, new Vector3(170f, 250, 0f), 0.4f);
			}

			// Show description panel
			descriptionObject.GetComponent<MoveTweenToggle>().Show();
		}
		else{
			Debug.LogError("No Metadata attached on badge");
		}
	}

	public void CloseDescription(){
		descriptionObject.GetComponent<MoveTweenToggle>().Hide();
		badgeGUISpawnBase.transform.DestroyChildren();
		Destroy(bgPanel);

		// Enable back button for badge board
		BadgeBoardClicked();
	}

	public void DisableBackButton(){
		isActive = false;
	}

	//When the badge board is clicked and zoomed into
	public void BadgeBoardClicked(){
		if(!isActive){
			isActive = true;
			badgeBoard.collider.enabled = false;

			backButtonReference = NGUITools.AddChild(this.gameObject, backButtonPrefab);
			backButtonReference.transform.localPosition = new Vector3(-595f, 330, 0);
			UIButtonMessage messageScript = backButtonReference.GetComponent<UIButtonMessage>();
			messageScript.target = this.gameObject;
			messageScript.functionName = "BadgeBoardBackButtonClicked";
		}
	}

	//The back button on the left top corner is clicked to zoom out of the badge board
	public void BadgeBoardBackButtonClicked(){
		if(isActive && !ClickManager.isClickLocked){
			if(OnBadgeBoardClosed != null){
    			OnBadgeBoardClosed (this, EventArgs.Empty);
			}else{
				Debug.LogError("OnBadgeBoardClosed is null");
			}
			isActive = false;
			badgeBoard.collider.enabled = true;

			if(backButtonReference != null){
				Destroy(backButtonReference);
			}
			else{
				Debug.LogError("No back button to delete");
			}
		}
	}

}

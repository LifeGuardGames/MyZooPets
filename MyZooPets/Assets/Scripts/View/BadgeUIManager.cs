using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BadgeUIManager : SingletonUI<BadgeUIManager> {
	public AnimationClip pulseClip;
	public GameObject backButtonPrefab;
	public GameObject badgeBoard;
	public GameObject badgeBoardBadges;
	public GameObject descriptionObject;
	public GameObject badgeGUISpawnBase;	// Parent to clone badges in (anchor-center) when zoomed in
	public UIAtlas badgeCommonAtlas;		// Holds ALL the low-res badges and common objects
	public UIAtlas badgeExtraAtlas;			// Holds tier (gold/silver/bronze) medals for zoomed display
	// High Definition badges go here for closeup
	// NOTE: 512x512 px, zero padding to fit extra row
	public UIAtlas badgeLevelAtlas1;
	public UIAtlas badgeLevelAtlas2;
	public UIAtlas badgeLevelAtlas3;
	public List<GameObject> LevelList = new List<GameObject>();	// Index of this list correlates to the index from BadgeLogic.Instance.LevelBadges
	public CameraMove cameraMove;
	
	private bool firstClick = true;
	private GameObject lastClickedBadge;
	private GameObject backButtonReference;
	private bool isActive = false;
	private GameObject badgeBackdrop;

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
		foreach(BadgeUIData badge in BadgeLogic.Instance.LevelBadges){
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
	
	public void BadgeClicked(GameObject go){
		// First time clicking, not showing description
		if(firstClick){
			firstClick = false;
			BadgeMetadata meta = go.GetComponent<BadgeMetadata>();
			descriptionObject.transform.FindChild("L_Title").gameObject.GetComponent<UILabel>().text = (meta != null) ? meta.title : "";
			descriptionObject.transform.FindChild("L_Description").gameObject.GetComponent<UILabel>().text = (meta != null) ? meta.description : "";
			ShowDescriptionPanel();
		}
		
		// Remove the animation component in the last badge and assign new reference
		if(lastClickedBadge != null){
			Destroy(lastClickedBadge.GetComponent<Animation>());
			lastClickedBadge.transform.localScale = Vector3.one;
		}
		lastClickedBadge = go;
		
		// Play pulsing animation in current badge
		Animation anim = go.AddComponent<Animation>();
		anim.AddClip(pulseClip, "scaleUpDown");
		anim.Play("scaleUpDown");
		
		// Hide callback, show last badge info
		TweenToggle toggle = descriptionObject.GetComponent<PositionTweenToggle>();
		toggle.HideTarget = gameObject;
		toggle.HideFunctionName = "RepopulateAndShowDescriptionPanel";
		HideDescriptionPanel();
	}
	
	// Callback for finished hide description, populate panel with new info and show
	private void RepopulateAndShowDescriptionPanel(){
		BadgeMetadata meta = lastClickedBadge.GetComponent<BadgeMetadata>();
		descriptionObject.transform.FindChild("L_Title").gameObject.GetComponent<UILabel>().text = (meta != null) ? meta.title : "";
		descriptionObject.transform.FindChild("L_Description").gameObject.GetComponent<UILabel>().text = (meta != null) ? meta.description : "";
		ShowDescriptionPanel();
	}
	
	private void ShowDescriptionPanel(){
		descriptionObject.GetComponent<PositionTweenToggle>().Show();
	}
	
	private void HideDescriptionPanel(){
		descriptionObject.GetComponent<PositionTweenToggle>().Hide();
	}

	public void DisableBackButton(){
		isActive = false;
	}

	//When the badge board is clicked and zoomed into
	protected override void _OpenUI(){		
		if(!isActive){
			cameraMove.ZoomToggle(ZoomItem.BadgeBoard);
			
			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
			
			isActive = true;
			badgeBoard.collider.enabled = false;
			firstClick = true;

			backButtonReference = NGUITools.AddChild(badgeBoardBadges, backButtonPrefab);
			backButtonReference.transform.localPosition = new Vector3(-595f, 330, 0);

			UIButtonMessage messageScript = backButtonReference.GetComponent<UIButtonMessage>();
			messageScript.target = this.gameObject;
			messageScript.functionName = "CloseUI";
		}
	}

	//The back button on the left top corner is clicked to zoom out of the badge board
	protected override void _CloseUI(){
		if(isActive && !ClickManager.Instance.isClickLocked){
			isActive = false;
			HideDescriptionPanel();
			badgeBoard.collider.enabled = true;
			
			cameraMove.ZoomOutMove();
	
			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			EditDecosUIManager.Instance.ShowNavButton();

			if(D.Assert(backButtonReference != null, "No back button to delete"))
				Destroy(backButtonReference);
		}
	}

}

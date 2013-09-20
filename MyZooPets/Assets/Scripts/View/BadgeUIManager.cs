using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BadgeUIManager : SingletonUI<BadgeUIManager> {
	public AnimationClip pulseClip;
	public GameObject backButton;
	public GameObject badgeBoard;
	public GameObject descriptionObject;
	public GameObject badgePrefab;
	public GameObject badgeBase;
	public UIAtlas badgeCommonAtlas;		// Holds ALL the low-res badges and common objects
	public UIAtlas badgeExtraAtlas;			// Holds tier (gold/silver/bronze) medals for zoomed display
	public CameraMove cameraMove;
	
	private bool firstClick = true;
	private GameObject lastClickedBadge;
	private GameObject backButtonReference;
	private bool isActive = false;

	void Start(){
		BadgeLogic.OnNewBadgeUnlocked += UnlockBadge;
		InitBadges();
	}

	void OnDestroy(){
		BadgeLogic.OnNewBadgeUnlocked -= UnlockBadge;
	}

	private void InitBadges(){
		List<Badge> badges = BadgeLogic.Instance.AllBadges;

		//Can't decide whether is query should be in BadgeLogic or not
		//Because it's using anonymous type I thought it's better to place it with
		//the query execution(foreach loop)
		var query = 
			from badge in badges
			group badge by badge.Type into badgeGroup
			select new{
				Key = badgeGroup.Key,
				Elements = 
					from badge2 in badgeGroup
					orderby badge2.UnlockCondition ascending
					select badge2
			};

		foreach(var group in query){
			foreach(var badge in group.Elements){
				GameObject badgeGO = NGUITools.AddChild(badgeBase, badgePrefab);
				badgeGO.name = badge.ID;
				badgeGO.GetComponent<UIButtonMessage>().target = this.gameObject;

				//TO DO: Update this after you have all the art for badges
				if(badge.IsUnlocked){

				}else{

				}
				badgeGO.transform.Find("badgeSprite").GetComponent<UISprite>().spriteName = "badgeBlank";

			}
		}

		badgeBase.GetComponent<UIGrid>().Reposition();
	}

	//Event Listener that updates the Level badges UI when a new badge is unlocked
	private void UnlockBadge(object senders, BadgeLogic.BadgeEventArgs arg){
		Badge badge = arg.UnlockedBadge;
		Transform badgeTrans = badgeBase.transform.Find(badge.ID);

		//TO DO: Update this after you have all the art for badges
		if(badge.IsUnlocked){

		}else{

		}

		if(badgeTrans != null)
			badgeTrans.Find("badgeSprite").GetComponent<UISprite>().spriteName = "badgeBlank";

		badgeBase.GetComponent<UIGrid>().Reposition();
	}
	
	public void BadgeClicked(GameObject go){
		Badge clickedBadge = BadgeLogic.Instance.GetBadge(go.name);

		// First time clicking, not showing description
		if(firstClick){
			firstClick = false;
			descriptionObject.transform.FindChild("L_Title").gameObject.GetComponent<UILabel>().text = (clickedBadge != null) ? clickedBadge.Name : "";
			descriptionObject.transform.FindChild("L_Description").gameObject.GetComponent<UILabel>().text = (clickedBadge != null) ? clickedBadge.Description : "";
			ShowDescriptionPanel();
		}
		
		if(lastClickedBadge != go){
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
	}
	
	// Callback for finished hide description, populate panel with new info and show
	private void RepopulateAndShowDescriptionPanel(){
		Badge clickedBadge = BadgeLogic.Instance.GetBadge(lastClickedBadge.name);
		descriptionObject.transform.FindChild("L_Title").gameObject.GetComponent<UILabel>().text = (clickedBadge != null) ? clickedBadge.Name : "";
		descriptionObject.transform.FindChild("L_Description").gameObject.GetComponent<UILabel>().text = (clickedBadge != null) ? clickedBadge.Description : "";
		ShowDescriptionPanel();
		descriptionObject.GetComponent<PositionTweenToggle>().HideTarget = null;
		descriptionObject.GetComponent<PositionTweenToggle>().HideFunctionName = null;
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
			
			backButton.SetActive(true);
		}
	}

	//The back button on the left top corner is clicked to zoom out of the badge board
	protected override void _CloseUI(){
		if(isActive && !ClickManager.Instance.isClickLocked){
			HideDescriptionPanel();
			if(lastClickedBadge != null){
				Destroy(lastClickedBadge.GetComponent<Animation>());
				lastClickedBadge.transform.localScale = Vector3.one;
			}
			lastClickedBadge = null;
			
			isActive = false;
			badgeBoard.collider.enabled = true;
			
			cameraMove.ZoomOutMove();
	
			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			EditDecosUIManager.Instance.ShowNavButton();

			if(D.Assert(backButton != null, "No back button to delete"))
				backButton.SetActive(false);
		}
	}

}

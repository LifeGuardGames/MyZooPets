using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BadgeBoardUIManager : SingletonUI<BadgeBoardUIManager> {
	public AnimationClip pulseClip;
	public GameObject backButton;
	public GameObject badgeBoard;
	public GameObject descriptionObject;
	public GameObject badgePrefab;
	public GameObject badgeBase;
	public UIAtlas badgeCommonAtlas;		// Holds ALL the low-res badges and common objects
	// public UIAtlas badgeExtraAtlas;			// Holds tier (gold/silver/bronze) medals for zoomed display
	
	private bool firstClick = true;
	private GameObject lastClickedBadge;
	private GameObject backButtonReference;
	private bool isActive = false;
	
	// related to zooming into the badge board
	public float fZoomTime;
	public Vector3 vOffset;
	public Vector3 vRotation;

	protected override void Start(){
		base.Start();
		BadgeLogic.OnNewBadgeUnlocked += UnlockBadge;
		InitBadges();
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		BadgeLogic.OnNewBadgeUnlocked -= UnlockBadge;
	}

	//When the scene starts Initialize all the badges once
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
				string textureName = "";
				//badgeGO.GetComponent<UIButtonMessage>().target = this.gameObject;

				//TO DO: Update this after you have all the art for badges
				if(badge.IsUnlocked){
					textureName = badge.TextureName;
				}else{
					textureName = badge.TextureName + "Dark";
				}
				badgeGO.transform.Find("badgeSprite").GetComponent<UISprite>().spriteName = textureName;
			}
		}

		badgeBase.GetComponent<UIGrid>().Reposition();
	}

	//Event Listener that updates the Level badges UI when a new badge is unlocked
	private void UnlockBadge(object senders, BadgeLogic.BadgeEventArgs arg){
		Badge badge = arg.UnlockedBadge;
		Transform badgeTrans = badgeBase.transform.Find(badge.ID);

		//TODO: Update this after you have all the art for badges
		// if(badge.IsUnlocked){

		// }else{

		// }

		if(badgeTrans != null)
			badgeTrans.Find("badgeSprite").GetComponent<UISprite>().spriteName = badge.TextureName;

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
			anim.AddClip(pulseClip, "scaleUpDownBadge");
			anim.Play("scaleUpDownBadge");
			
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
			// zoom into the board
			Vector3 vPos = badgeBoard.transform.position + vOffset;
			CameraManager.Instance.ZoomToTarget( vPos, vRotation, fZoomTime, null );
			
			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
			
			isActive = true;
			badgeBoard.collider.enabled = false;
			firstClick = true;
			
			backButton.SetActive(true);
		}
	}

	//The back button on the left top corner is clicked to zoom out of the badge board
	protected override void _CloseUI(){
		if(isActive){
			HideDescriptionPanel();
			if(lastClickedBadge != null){
				Destroy(lastClickedBadge.GetComponent<Animation>());
				lastClickedBadge.transform.localScale = Vector3.one;
			}
			lastClickedBadge = null;
			
			isActive = false;
			badgeBoard.collider.enabled = true;
			
			CameraManager.Instance.ZoomOutMove();
	
			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			RoomArrowsUIManager.Instance.ShowPanel();

			if(D.Assert(backButton != null, "No back button to delete"))
				backButton.SetActive(false);
		}
	}
}

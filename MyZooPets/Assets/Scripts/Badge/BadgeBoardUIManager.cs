using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BadgeBoardUIManager : SingletonUI<BadgeBoardUIManager> {
	public AnimationClip pulseClip;
	public GameObject badgeBoard;
	public GameObject badgePrefab;
	public GameObject badgeBase;
	public string blankBadgeTextureName = "64pxTest";

	public TweenToggleDemux descriptionDemux;
	public UISprite descriptionBadgeSprite;
	public UILabel descriptionBadgeTitle;
	public UILabel descriptionBadgeInfo;

	private bool firstClick = true;
	private GameObject lastClickedBadge;
	private bool isActive = false;

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Badge;
	}

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

				//TODO: Update this after you have all the art for badges
				if(badge.IsUnlocked){
					textureName = badge.TextureName;
				}else{
					textureName = blankBadgeTextureName;
				}
				badgeGO.transform.Find("badgeSprite").GetComponent<UISprite>().spriteName = textureName;
			}
		}

		UIGrid grid = badgeBase.GetComponent<UIGrid>();
		grid.sorted = true;
		grid.Reposition();
	}

	//Event Listener that updates the Level badges UI when a new badge is unlocked
	private void UnlockBadge(object senders, BadgeLogic.BadgeEventArgs arg){
		// Show the badge board and show the get badge animation
		StartCoroutine(UnlockBadgeHelper(arg.UnlockedBadge));
	}

	private IEnumerator UnlockBadgeHelper(Badge badge){
		Transform badgeTrans = badgeBase.transform.Find(badge.ID);
		if(badgeTrans != null){
			badgeTrans.Find("AnimParent").GetComponent<Animation>().Play();
		}
		else{
			Debug.LogError("Badge " + badge.ID + " not found to unlock");
		}
		yield return 0;
	}

	private void UnlockBadgeHelperShowSprite(){
		// TODO need way to get the specific sprite!!!!
		//			badgeTrans.Find("badgeSprite").GetComponent<UISprite>().spriteName = badge.TextureName;
	}

	public void BadgeClicked(GameObject go){
		Badge clickedBadge = BadgeLogic.Instance.GetBadge(go.name);
		descriptionBadgeSprite.spriteName = blankBadgeTextureName; //TODO
		descriptionBadgeTitle.text = clickedBadge.Name;
		descriptionBadgeInfo.text = clickedBadge.Description;
		Debug.Log("Badge clicked");
		ShowDescriptionPanel();
	}

	/// <summary>
	/// Clicked anywhere when showing the info, return to normal badge display
	/// </summary>
	public void BadgeInfoClicked(){
		HideDescriptionPanel();
	}
	
	private void ShowDescriptionPanel(){
		descriptionDemux.GetComponent<TweenToggleDemux>().Show();
	}
	
	private void HideDescriptionPanel(){
		descriptionDemux.GetComponent<TweenToggleDemux>().Hide();
	}

	protected override void _OpenUI(){
		if(!isActive){
			GetComponent<TweenToggleDemux>().Show();

			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();

			isActive = true;
			badgeBoard.collider.enabled = false;
		}
	}

	protected override void _CloseUI(){
		if(isActive){
			HideDescriptionPanel();
			GetComponent<TweenToggleDemux>().Hide();

			isActive = false;
			badgeBoard.collider.enabled = true;

			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			RoomArrowsUIManager.Instance.ShowPanel();
		}
	}
}

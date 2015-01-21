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
	public string blankBadgeTextureName = "badge64Blank";

	public TweenToggleDemux descriptionDemux;
	public UISprite descriptionBadgeSprite;
	public UILabel descriptionBadgeTitle;
	public UILabel descriptionBadgeInfo;

	private bool firstClick = true;
	private GameObject lastClickedBadge;
	private bool isActive = false;

	private Queue<Badge> badgeUnlockQueue;	// This is the queue that will pop recently unlocked badges
	private bool isQueueAnimating = false;
	private GameObject currentAnimatingObject;	// Aux used for animation sprite swapping
	private Badge currentAnimatingBadge;		// Aux used for animation information

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Badge;
	}

	protected override void Start(){
		base.Start();
		badgeUnlockQueue = new Queue<Badge>();
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
				badgeGO.transform.Find("AnimParent/badgeSprite").GetComponent<UISprite>().spriteName = blankBadgeTextureName;
			}
		}

		UIGrid grid = badgeBase.GetComponent<UIGrid>();
		grid.sorted = true;
		grid.Reposition();
	}

	//Event Listener that updates the Level badges UI when a new badge is unlocked
	private void UnlockBadge(object senders, BadgeLogic.BadgeEventArgs arg){
		// Populate the unlocked badge into the unlock queue
		badgeUnlockQueue.Enqueue(arg.UnlockedBadge);

		StartCoroutine(TryPopBadgeQueue());
	}

	// Show the badge board pop queue board if any
	private IEnumerator TryPopBadgeQueue(){
		// Check to see if it is in process of animating already
		if(!isQueueAnimating){
			if(badgeUnlockQueue.Count != 0){

				// If the badge board is not opened already, open the UI and wait a while
				if(!BadgeBoardUIManager.Instance.IsOpen()){
					OpenUI();
					yield return new WaitForSeconds(1f);
				}

				// Time to animate, lock the queue animation check
				isQueueAnimating = true;
				Badge unlockingBadge = badgeUnlockQueue.Dequeue();
				Transform badgeGOTransform = badgeBase.transform.Find(currentAnimatingBadge.Name);

				if(badgeGOTransform != null){
					BadgeController badgeController = badgeGOTransform.gameObject.GetComponent<BadgeController>();
					badgeController.PlayUnlockAnimation();
				}
				else{
					Debug.LogWarning("Can not find badge name: " + currentAnimatingBadge.Name);
					CloseUI();	// Try to fail gracefully
				}
			}
		}
	}

	/// <summary>
	/// Called when a badge animation is done, check the queue again if still needs popping
	/// </summary>
	public IEnumerator BadgeAnimationDone(){
		yield return new WaitForSeconds(1f);

		isQueueAnimating = false;	// Release the animation lock
		StartCoroutine(TryPopBadgeQueue());	// Fire off next in queue try

		// Ending queue check, all animations and popping finished
		if(badgeUnlockQueue.Count == 0){
			CloseUI();
		}
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

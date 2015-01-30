﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BadgeBoardUIManager : SingletonUI<BadgeBoardUIManager> {

	public static EventHandler<EventArgs> OnBadgeUIAnimationDone;

	public AnimationClip pulseClip;
	public GameObject badgeBoard;
	public GameObject badgePrefab;
	public GameObject badgeBase;
	public string blankBadgeTextureName = "badge64Blank";

	public TweenToggleDemux descriptionDemux;
	public UISprite descriptionBadgeSprite;
	public UILabel descriptionBadgeTitle;
	public UILabel descriptionBadgeInfo;
	public ParticleSystem slamParticle;

	private bool firstClick = true;
	private GameObject lastClickedBadge;
	private bool isActive = false;

	private Queue<Badge> badgeUnlockQueue;	// This is the queue that will pop recently unlocked badges

	private bool isQueueAnimating = false;
	public bool IsBadgeBoardUIAnimating{	// Scenes will poll this to see if they need to wait
		get{
			return isQueueAnimating;
		}
	}

	public delegate void Callback();
	public Callback FinishedAnimatingCallback;

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
				badgeGO.GetComponent<BadgeController>().Init(badge.IsUnlocked, badge.TextureName, blankBadgeTextureName);
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

		// Try to animate, lock the queue animation check so more than one calls wont go thru
		if(!isQueueAnimating){
			isQueueAnimating = true;
			StartCoroutine(TryPopBadgeQueue());
		}
	}

	// Show the badge board pop queue board if any
	private IEnumerator TryPopBadgeQueue(){
			if(badgeUnlockQueue.Count != 0){

			// If the badge board is not opened already, open the UI and wait a while
			if(!BadgeBoardUIManager.Instance.IsOpen()){
				float sceneSpecificDelay = Constants.GetConstant<float>("BadgeBoardDelay_" + Application.loadedLevelName);
				yield return new WaitForSeconds(sceneSpecificDelay);
				OpenUI();
				yield return new WaitForSeconds(1f);
			}

			Badge unlockingBadge = badgeUnlockQueue.Dequeue();
			Transform badgeGOTransform = badgeBase.transform.Find(unlockingBadge.ID);
			if(badgeGOTransform != null){
				BadgeController badgeController = badgeGOTransform.gameObject.GetComponent<BadgeController>();
				badgeController.PlayUnlockAnimation();
			}
			else{
				Debug.LogWarning("Can not find badge name: " + unlockingBadge.ID);
				CloseUI();	// Try to fail gracefully
			}
		}
	}

	/// <summary>
	/// Plays the slam particle. Call from badge reward animation event
	/// </summary>
	public void PlaySlamParticle(Vector3 position){
		slamParticle.transform.position = position;
		slamParticle.Play();
	}

	/// <summary>
	/// Called when a badge animation is done, check the queue again if still needs popping
	/// </summary>
	public IEnumerator BadgeAnimationDone(){
		yield return new WaitForSeconds(1f);
		
		// Ending queue check, all animations and popping finished
		if(badgeUnlockQueue.Count == 0){
			CloseUI();

			isQueueAnimating = false;	// Release the animation lock

			//Notify anything that is listening to animation done
			if(OnBadgeUIAnimationDone != null){
				OnBadgeUIAnimationDone(this, EventArgs.Empty);
			}

			// Launch any finished callback
			if(FinishedAnimatingCallback != null){
				FinishedAnimatingCallback();
			}
		}

		StartCoroutine(TryPopBadgeQueue());	// Fire off next in queue try
	}

	public void BadgeClicked(GameObject go){
		// Get the information from the populated controller
		Badge clickedBadge = BadgeLogic.Instance.GetBadge(go.name);
		descriptionBadgeSprite.spriteName = BadgeLogic.Instance.IsBadgeUnlocked(clickedBadge.ID) ? clickedBadge.TextureName : blankBadgeTextureName;
		descriptionBadgeTitle.text = clickedBadge.Name;
		descriptionBadgeInfo.text = clickedBadge.Description;
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
			HUDUIManager.Instance.HidePanel();

			if(NavigationUIManager.Instance != null){
				NavigationUIManager.Instance.HidePanel();
			}
			if(InventoryUIManager.Instance != null){
				InventoryUIManager.Instance.HidePanel();
			}
			if(RoomArrowsUIManager.Instance != null){
				RoomArrowsUIManager.Instance.HidePanel();
			}

			isActive = true;

			if(badgeBoard != null){
				badgeBoard.collider.enabled = false;
			}
		}
	}

	protected override void _CloseUI(){
		if(isActive){
			HideDescriptionPanel();
			GetComponent<TweenToggleDemux>().Hide();

			isActive = false;

			if(badgeBoard != null){
				badgeBoard.collider.enabled = true;
			}

			//Show other UI Objects
			HUDUIManager.Instance.ShowPanel();

			if(NavigationUIManager.Instance != null){
				NavigationUIManager.Instance.ShowPanel();
			}
			if(InventoryUIManager.Instance != null){
				InventoryUIManager.Instance.ShowPanel();
			}
			if(RoomArrowsUIManager.Instance != null){
				RoomArrowsUIManager.Instance.ShowPanel();
			}
		}
	}
}

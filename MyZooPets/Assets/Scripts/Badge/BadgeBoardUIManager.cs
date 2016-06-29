using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BadgeBoardUIManager : SingletonUI<BadgeBoardUIManager> {

	public static EventHandler<EventArgs> OnBadgeUIAnimationDone;
	public BadgePopController badgePopController;		// Used for unlocking badges
	public GameObject bedroomBadgeBoard;
	public GameObject badgePrefab;
	public GridLayoutGroup badgeGrid;
	public GameObject badgeExitButton;

	public TweenToggleDemux descriptionDemux;
	public Image descBadgeSprite;
	public Text descBadgeTitle;
	public Text descBadgeInfo;

	private string blankBadgeTextureName = "badgeBlank";
	private GameObject lastClickedBadge;
	private bool isActive = false;

	private Queue<ImmutableDataBadge> badgeUnlockQueue;	// This is the queue that will pop recently unlocked badges

	private bool isQueueAnimating = false;
	public bool IsBadgeBoardUIAnimating{	// Scenes will poll this to see if they need to wait
		get{ return isQueueAnimating; }
	}

	private bool isWaitingInRewardQueue = false;
	public bool IsWaitingInRewardQueue{		// If it is waiting in reward queue waiting to be animated, used for controlling unlocking multiple badges
		get{ return isWaitingInRewardQueue; }
	}

	public delegate void Callback();
	public Callback FinishedAnimatingCallback;
	private bool isOpenedAsReward = false;	// Check if it is badge board clicked or from reward manager

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Badge;
	}

	protected override void Start(){
		base.Start();
		badgeUnlockQueue = new Queue<ImmutableDataBadge>();
		BadgeManager.OnNewBadgeUnlocked += UnlockBadge;
		InitBadges();
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		BadgeManager.OnNewBadgeUnlocked -= UnlockBadge;
	}

	//When the scene starts Initialize all the badges once
	private void InitBadges(){
		List<ImmutableDataBadge> badges = BadgeManager.Instance.AllBadges;

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
				GameObject badgeGO = GameObjectUtils.AddChildGUI(badgeGrid.gameObject, badgePrefab);
				badgeGO.name = badge.ID;
				badgeGO.GetComponent<BadgeController>().Init(badge);
			}
		}
	}

	//Event Listener that updates the Level badges UI when a new badge is unlocked
	private void UnlockBadge(object senders, BadgeManager.BadgeEventArgs arg){
		// Populate the unlocked badge into the unlock queue
		badgeUnlockQueue.Enqueue(arg.UnlockedBadge);

		// Check for waiting in reward queue because there might be more badges coming in during waiting so only want to enqueue
		if(!isWaitingInRewardQueue){
			isWaitingInRewardQueue = true;

			RewardQueueData.GenericDelegate funtion = delegate(){
				// Try to animate, lock the queue animation. might be more badges coming in during animation so only want to enqueue
				if(!isQueueAnimating){
					isQueueAnimating = true;
					StartCoroutine(TryPopBadgeQueue());
				}
			};
			RewardManager.Instance.AddToRewardQueue(funtion);
		}
	}

	// Show the badge board pop queue board if any
	private IEnumerator TryPopBadgeQueue(){
		if(badgeUnlockQueue.Count != 0){
			// If the badge board is not opened already, open the UI and wait a while
			if(!IsOpen()){
				float sceneSpecificDelay = Constants.GetConstant<float>("BadgeBoardDelay_" + SceneUtils.CurrentScene);
				yield return new WaitForSeconds(sceneSpecificDelay);

				OpenUI();
				yield return new WaitForSeconds(1f);
			}

			ImmutableDataBadge unlockingBadge = badgeUnlockQueue.Dequeue();
			Transform badgeGOTransform = badgeGrid.transform.Find(unlockingBadge.ID);
			if(badgeGOTransform != null){
				badgePopController.InitializeAndPlay(badgeGOTransform.gameObject);
			}
			else{
				Debug.LogWarning("Can not find badge name: " + unlockingBadge.ID);
				CloseUI();	// Try to fail gracefully
			}
		}
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
			isWaitingInRewardQueue = false;		// It is animating so no longer waiting in reward queue
			
			// Launch any finished callback
			if(FinishedAnimatingCallback != null){
				FinishedAnimatingCallback();
			}
		}
		else{
			StartCoroutine(TryPopBadgeQueue());	// Fire off next in queue try
		}
	}

	public void BadgeClicked(GameObject go){
		// Get the information from the populated controller
		AudioManager.Instance.PlayClip("BadgeClicked");
		ImmutableDataBadge clickedBadge = DataLoaderBadges.GetData(go.name);
		descBadgeSprite.sprite = SpriteCacheManager.GetBadgeSprite(BadgeManager.Instance.IsBadgeUnlocked(clickedBadge.ID) ? clickedBadge.ID : null);
		descBadgeTitle.text = clickedBadge.Name;
		descBadgeInfo.text = clickedBadge.Description;
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
			if(SceneUtils.CurrentScene == SceneUtils.BEDROOM){
				PetMovement.Instance.StopMoving();
			}
			AudioManager.Instance.PlayClip("subMenu");

			// Disable the exit button if the badge is animating from reward manager
			badgeExitButton.SetActive(isQueueAnimating ? false : true);
			isOpenedAsReward = isQueueAnimating;

			GetComponent<TweenToggleDemux>().Show();

			//FirstInteraction.Instance.SetString("Badges");

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

			if(bedroomBadgeBoard != null){
				bedroomBadgeBoard.GetComponent<Collider>().enabled = false;
			}
		}
	}

	protected override void _CloseUI(){
		if(isActive){
			HideDescriptionPanel();
			GetComponent<TweenToggleDemux>().Hide();

			isActive = false;

			if(bedroomBadgeBoard != null){
				bedroomBadgeBoard.GetComponent<Collider>().enabled = true;
			}

			CloseUIOpenNext(UIModeTypes.None);
		}
	}

	private void CloseFinishHelper(){
		if(isOpenedAsReward){
			//Notify anything that is listening to animation done
			if(OnBadgeUIAnimationDone != null){
				OnBadgeUIAnimationDone(this, EventArgs.Empty);
			}
		}
	}

	public void OnExitButton() {
		CloseUI();
	}
}

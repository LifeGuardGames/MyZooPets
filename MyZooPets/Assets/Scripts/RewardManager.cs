using System;
using UnityEngine;

/// <summary>
/// Reward manager.
// TODO persistent??
/// This is a class that is required across all scenes where it controls the UI order for
/// Stats reward, badge reward, and fire crystal reward
/// This is done through a queue that makes sure one is done animating before the other is called.
/// </summary>
public class RewardManager : Singleton<RewardManager> {

	public static EventHandler<EventArgs> OnAllRewardsDone;

	private bool isRewardingActive = false;
	public bool IsRewardingActive{
		get{ return isRewardingActive; }
	}

	private bool isDoAnimationDoneCheck = false;	// This aux bool checks for animating -> finish animating to call event

	/// <summary>
	/// Assign some callbacks that will launch try next reward when the components are done animating
	/// </summary>
	void Awake(){
		FireCrystalUIManager.OnFireCrystalUIAnimationDone += TryNextReward;
		BadgeBoardUIManager.OnBadgeUIAnimationDone += TryNextReward;
		HUDAnimator.OnStatsAnimationDone += TryNextReward;
	}

	void OnDestroy(){
		FireCrystalUIManager.OnFireCrystalUIAnimationDone -= TryNextReward;
		BadgeBoardUIManager.OnBadgeUIAnimationDone -= TryNextReward;
		HUDAnimator.OnStatsAnimationDone -= TryNextReward;
	}

	void Start(){
		if(!isRewardingActive){		
			// Check the static queue to see if anything is there on level load
			TryNextReward();
		}
	}

	/// <summary>
	/// Adds new reward request to queue.
	/// </summary>
	/// <param name="notificationEntry">Notification entry.</param>
	public void AddToRewardQueue(RewardQueueData.GenericDelegate functionToCall){
		RewardQueueData.AddReward(functionToCall);
		
		if(!isRewardingActive){
//			Debug.Log("Try next reward----");
			TryNextReward();
		}else{
//			Debug.Log("Reward queue FULL----");
		}
	}

	public void TryNextReward(object sender, EventArgs args){
		TryNextReward();
	}

	public void TryNextReward(){
		if(!RewardQueueData.IsEmpty()){
			isRewardingActive = true;
			isDoAnimationDoneCheck = true;
			RewardQueueData.GenericDelegate functionToCall = RewardQueueData.PopReward();
			functionToCall();
		}
		else{   // End condition here
			isRewardingActive = false;

			// Called only when it has animated and finished
			if(isDoAnimationDoneCheck){
				isDoAnimationDoneCheck = false;
//				Debug.Log("ALL REWARDS DONE");
				if(OnAllRewardsDone != null){
					OnAllRewardsDone(this, EventArgs.Empty);
				}
			}
		}
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 200, 100, 100), "Fire reward")){
//
//			RewardQueueData.GenericDelegate gen = delegate(){
//				StatsController.Instance.ChangeStats(deltaStars: 100);
//			};
//			RewardQueueData.AddReward(gen);
//		}

//		if(GUI.Button(new Rect(200, 100, 100, 100), "Fire reward2")){
//			
//			RewardQueueData.GenericDelegate gen = delegate(){
//				BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Inhaler, 5, false);
//			};
//			RewardQueueData.AddReward(gen);
//		}

//		if(GUI.Button(new Rect(300, 100, 100, 100), "Fire reward2")){
//			FireCrystalManager.Instance.RewardShards(100);
//		}

//		if(GUI.Button(new Rect(400, 100, 100, 100), "POP")){
//			RewardQueueData.PopReward();
//		}
//	}
}

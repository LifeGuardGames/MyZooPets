using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Reward manager.
/// This is a class that is persistent across all scenes where it controls the UI order for
/// Stats reward, badge reward, and fire crystal reward
/// This is done through a queue that makes sure one is done animating before the other is called.
/// </summary>

public class RewardManager : Singleton<RewardManager> {

	public static EventHandler<EventArgs> OnAllRewardsDone;

	private bool isRewardingActive = false;
	private bool isAnimationDoneCheck = false;	// This aux bool checks for animating -> finish animating to call event

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

		Debug.Log("ADD TO REWARD QUEUE");
		RewardQueueData.AddReward(functionToCall);
		
		if(!isRewardingActive){
			Debug.Log(" Reward queue empty");
			TryNextReward();
		}else{
			Debug.Log(" Reward queue FULL");
		}
	}

	public void TryNextReward(object sender, EventArgs args){
		Debug.Log("TRY NEXT REWARD CALLBACK");
		TryNextReward();
	}

	public void TryNextReward(){
		Debug.Log("TRYING NEXT");
		if(!RewardQueueData.IsEmpty()){
			isRewardingActive = true;
			isAnimationDoneCheck = true;
			RewardQueueData.GenericDelegate functionToCall = RewardQueueData.PopReward();
			functionToCall();
		}
		else{	// End condition here
			Debug.Log("DONE");
			isRewardingActive = false;

			// Called only when it has animated and finished
			if(isAnimationDoneCheck){
				isAnimationDoneCheck = false;
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
//
//		if(GUI.Button(new Rect(200, 100, 100, 100), "Fire reward2")){
//			
//			RewardQueueData.GenericDelegate gen = delegate(){
//				BadgeLogic.Instance.CheckSeriesUnlockProgress(BadgeType.Inhaler, 5, false);
//			};
//			RewardQueueData.AddReward(gen);
//		}
//
//		if(GUI.Button(new Rect(300, 100, 100, 100), "Fire reward2")){
//			RewardQueueData.GenericDelegate gen = delegate(){
//				FireCrystalManager.Instance.RewardShards(100);
//			};
//			RewardQueueData.AddReward(gen);
//		}
//
//		if(GUI.Button(new Rect(400, 100, 100, 100), "POP")){
//			RewardQueueData.PopReward();
//		}
//	}
}

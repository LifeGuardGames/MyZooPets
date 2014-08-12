﻿using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Script to control Minipet and contains the basic properties of a minipet.
/// Should attach this script to the highest parent in the minipet prefab
/// </summary>
public class MiniPet : MonoBehaviour {
	public MiniPetAnimationManager animationManager;
	public ParticleSystem bubbleParticle;
	public ParticleSystem dirtyParticle;
	public MiniPetSpeechAI miniPetSpeechAI;

	private string id; //pet id
	private string name;
	private float currentDistanceInCentimeters = 0;
	private float targetDistanceInCentimetersForCleanGesture = 300; //clean gestures will be recognized after the finger moved 300cm (both x and y position)

	private float tickleTimer = 0;
	private float timeBeforeTickleAnimationStops = 3f; //tickle animation will be stopped in 3 seconds


	void Start(){
		MiniPetHUDUIManager.Instance.OnManagerOpen += ShouldPauseIdleAnimations;
		MiniPetHUDUIManager.OnLevelUpAnimationCompleted += LevelUpEventHandler;
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;
		MiniPetManager.MiniPetStatusUpdate += UpdateAnimation;

		MiniPetManager.Instance.CheckToRefreshMiniPetStatus(id);
		RefreshMiniPetUIState();
	}
	
	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
		if(MiniPetHUDUIManager.Instance)
			MiniPetHUDUIManager.Instance.OnManagerOpen -= ShouldPauseIdleAnimations;
		MiniPetHUDUIManager.OnLevelUpAnimationCompleted -= LevelUpEventHandler;
		MiniPetManager.MiniPetStatusUpdate -= UpdateAnimation;
	}

	void Update(){

		//count down starts if tickling animation is playing.
		if(animationManager.IsTickling()){
			tickleTimer += Time.deltaTime;

			//turn tickling animation off after certain time
			if(tickleTimer > timeBeforeTickleAnimationStops){
				tickleTimer = 0;
				animationManager.StopTickling();
			}
		}
	}

	void OnApplicationPause(bool isPaused){
		if(!isPaused){
			MiniPetManager.Instance.CheckToRefreshMiniPetStatus(id);
			RefreshMiniPetUIState();
		}
			
	}

	void OnTap(TapGesture gesture){
		bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();
		if(!isUIOpened){
			Vector3 positionOffset = new Vector3(3, 4, -11);
			Vector3 position = this.transform.position + positionOffset;
			Vector3 rotation = new Vector3(12, 0, 0);

			ClickManager.Instance.Lock(mode: UIModeTypes.MiniPet);
			CameraManager.Instance.ZoomToTarget(position, rotation, 1f, this.gameObject);
		}
		else{
			string colliderName = gesture.Selection.collider.name;
			
			if(colliderName == this.gameObject.name){

				//if tickling animation is still playing reset timer
				if(animationManager.IsTickling()){
					tickleTimer = 0;
				}
				else{
					animationManager.StartTickling();
					MiniPetManager.Instance.SetTickle(id, true);
					MiniPetManager.Instance.IsFirstTimeTickling = false;
				}
			}
		}
	}

	/// <summary>
	/// Raises the drag event.
	/// Note: This needs to be here so it catches the OnDrag event sent out by UICamera
	/// which belongs to NGUI. Finger Gesture also send out the same event so they
	/// need to be specified otherwise error will be thrown
	/// </summary>
	/// <param name="delta">Delta.</param>
	void OnDrag(Vector2 delta){}

	void OnDrag(DragGesture gesture){
		bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();
		if(!isUIOpened) return;

		switch(gesture.Phase){
		case ContinuousGesturePhase.Started:
			bubbleParticle.Play();

			MoveBubbleParticleWithUserTouch(gesture);
			break;
		case ContinuousGesturePhase.Updated:
			MoveBubbleParticleWithUserTouch(gesture);

			float totalMoveXInCentimeters = Mathf.Abs(gesture.TotalMove.Centimeters().x);
			float totalMoveYInCentimeters = Mathf.Abs(gesture.TotalMove.Centimeters().y);

			currentDistanceInCentimeters += (totalMoveXInCentimeters + totalMoveYInCentimeters);

			//if clean gesture is recognized. stop dirty particle and play happy animation
			if(currentDistanceInCentimeters >= targetDistanceInCentimetersForCleanGesture){
				MiniPetManager.Instance.SetCleaned(id, true);
				MiniPetManager.Instance.IsFirstTimeCleaning = false;
				dirtyParticle.Stop();
				animationManager.Cheer();
				currentDistanceInCentimeters = 0;
			}

			break;
		case ContinuousGesturePhase.Ended:
			bubbleParticle.Stop();
			break;
		}
	}

	/// <summary>
	/// Pass in the immutable data so this specific MiniPet instantiate can be instantiated
	/// with the proper information.
	/// </summary>
	/// <param name="data">ImmutableDataMiniPet.</param>
	public void Init(ImmutableDataMiniPet data){
		this.id = data.ID;
		this.name = data.Name;
	}

	private void ShouldPauseIdleAnimations(object sender, UIManagerEventArgs args){
		if(args.Opening)
			animationManager.IsRunningIdleAnimations = false;
		else
			animationManager.IsRunningIdleAnimations = true;
	}

	/// <summary>
	/// Updates the animation when minipet status is also updated
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void UpdateAnimation(object sender, MiniPetManager.StatusUpdateEventArgs args){
		MiniPetManager.UpdateStatuses status = args.UpdateStatus;
	
		if(status == MiniPetManager.UpdateStatuses.Tickle){
			RefreshMiniPetUIState();
		}
	}

	private void MoveBubbleParticleWithUserTouch(DragGesture gesture){
		bool isDraggingOnMP = gesture.Raycast.Hit3D.collider &&
			gesture.Raycast.Hit3D.collider.name == this.gameObject.name;

		if(isDraggingOnMP){
			//z position is constant
			Vector3 touchPositionInWorld = 
				Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x, gesture.Position.y, 10));
			
			bubbleParticle.transform.position = touchPositionInWorld;
		}
		else{
			bubbleParticle.Stop();
		}
	}

	private void RefreshMiniPetUIState(){
		//check if pet is sad and dirty
		bool isTickled = MiniPetManager.Instance.IsTickled(id);
		bool isCleaned = MiniPetManager.Instance.IsCleaned(id);
		
		if(!isTickled)
			animationManager.Sad();
		else
			animationManager.NotSad();
		
		if(!isCleaned){
			dirtyParticle.Play();
		}
		else{
			dirtyParticle.Stop();
		}
	}
	
	private void CameraMoveDone() {
		ClickManager.Instance.ReleaseLock();
		MiniPetHUDUIManager.Instance.SelectedMiniPetID = id;
		MiniPetHUDUIManager.Instance.SelectedMiniPetName = name;
		MiniPetHUDUIManager.Instance.SelectedMiniPetGameObject = this.gameObject;
		MiniPetHUDUIManager.Instance.OpenUI();
	}

	/// <summary>
	/// When item is dropped on MP do the necessary check and carry out the action.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		bool isLevelUpAnimationLockOn = MiniPetHUDUIManager.Instance.IsLevelUpAnimationLockOn;
		bool isUIOpened = MiniPetHUDUIManager.Instance.IsOpen();

		if(args.TargetCollider.name == this.gameObject.name && 
		   !isLevelUpAnimationLockOn && isUIOpened){

			string invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);

			//check if minipet needs food
			if(MiniPetManager.Instance.CanModifyFoodXP(id)){
				//use item if so
				args.IsValidTarget = true;
				
				//notify inventory logic that this item is being used
				InventoryLogic.Instance.UseMiniPetItem(invItemID);
				MiniPetManager.Instance.IncreaseFoodXP(id);

				animationManager.Eat();
			}
			else{
				bool isTickled = MiniPetManager.Instance.IsTickled(id);
				bool isCleaned = MiniPetManager.Instance.IsCleaned(id);
				bool isMaxLevel = MiniPetManager.Instance.IsMaxLevel(id);

				if(!isTickled){
					miniPetSpeechAI.ShowSadMsg();
				}
				else if(!isCleaned){
					miniPetSpeechAI.ShowDirtyMsg();
				}
				else if(isMaxLevel){
					miniPetSpeechAI.ShowMaxLevelMsg();
				}
				else{}
			}
		}
	}

	private void LevelUpEventHandler(object sender, EventArgs args){
		if(MiniPetHUDUIManager.Instance.SelectedMiniPetID == id){
//			// spawn the item to be coming out of this box
//			GameObject droppedStatPrefab = Resources.Load("DroppedStat") as GameObject;
//			GameObject droppedItem = Instantiate(droppedStatPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
//			DroppedObjectStat droppedObjectStat = droppedItem.GetComponent<DroppedObjectStat>();
//			
//			droppedObjectStat.Init(HUDElementType.Gems, 1);
//			droppedObjectStat.modeTypes.Add(UIModeTypes.MiniPet);
//			
//			// set the position of the newly spawned item to be wherever this item box is
//			Vector3 position = gameObject.transform.position;
////			position.y -= 8; //drop the stat underneath the smoke monster
//			droppedItem.transform.position = position;
//			
//			// make the item "burst" out
//			droppedObjectStat.Burst();
		}
	}
}

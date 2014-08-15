using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PartitionChangedArgs : EventArgs{
	public int oldPartition;
	public int newPartition;

	public PartitionChangedArgs(int oldPartition, int newPartition){
		this.oldPartition = oldPartition;
		this.newPartition = newPartition;
	}
}

//---------------------------------------------------
// PanToMoveCamera
//    Attach this script to main camera.
//    Panning left or right will move the camera x position right or left, respectively.
//    Swiping Left or right will snap the camera x position right or left, respectively.
//---------------------------------------------------

public class PanToMoveCamera : MonoBehaviour{
	//=======================Events========================
	public EventHandler<PartitionChangedArgs> OnPartitionChanged;   // when the partition has changed (and the camera has finished moving)
	public EventHandler<PartitionChangedArgs> OnPartitionChanging;  // when the partition is changing (i.e. camera is still moving)
	//========================================================
	
	public int numOfPartitions = 4; //number of partitions allowed
	public int firstPartition = -1; //Set this to negative numbers if you want to open a partition
	//on the left of the starting partition(always 0)
	public int lastPartition = 2;
	public float partitionOffset = 80.0f; //How big each partition is in world position
	public int currentPartition = 0;
	public float minNormalizedPanDistance = 0.05f; //min normalized panning distance
	public float maxSwipeTime = 0.3f; //Swipe gesture needs to be faster than maxSwipeTime
	public float panDistanceToChange = 0.5f;    // distance to pan before the camera will snap to the next partition

	private Vector2 startTouchPos; //Position of touch when finger touches the screen
	private Vector2 currentTouchPos; //Position of touch right now
	private float startTime; //Time at when finger touches screen
	private RoomDirection panDirection; //direction of the last finger gesture
	private float normalizedTouchPosX; //0 ~ 1. 0.1 is 10% of the screen of any width
	private bool touchCancelled = false; //True: touch shouldn't be handled
	private Camera nguiCamera;
	private Camera mainCamera;

	// Use this for initialization
	void Start(){
		//Move camera to the last saved partition
		LoadSceneData sceneData = DataManager.Instance.SceneData;
		if(sceneData != null)
		if(sceneData.LastScene == Application.loadedLevelName)
			SetCameraToPartition(sceneData.LastCameraPartition);
	}

	
	///////////////////////////////////////////
	// SnapCamera()
	// Snaps the camera to the current partition.
	///////////////////////////////////////////		
	private void SnapCamera(int oldPartition){
		float moveTo = partitionOffset * currentPartition;
		
		// if the camera is actually already in this position, don't bother doing anything	
		if(gameObject.transform.position.x == moveTo)
			return;
		
		// prepare the hashtables for the camera snap callback
		Hashtable optional = new Hashtable();
		optional.Add("onComplete", "OnCameraSnapped");
		optional.Add("onCompleteTarget", gameObject);
		
		Hashtable completeParamHash = new Hashtable();
		completeParamHash.Add("Old", oldPartition);			
		optional.Add("onCompleteParam", completeParamHash);
		optional.Add("ease", LeanTweenType.easeOutQuad);
			
		LeanTween.moveX(gameObject, moveTo, 0.35f, optional);
		normalizedTouchPosX = 0;		
	}

	/// <summary>
	/// Callback for when the camera is done snapping
	/// </summary>
	/// <param name="hash">Hash.</param>
	private void OnCameraSnapped(Hashtable hash){
		int oldPartition = (int)hash["Old"];
		
		// if we were snapping back, don't send anything
		if(oldPartition == currentPartition)
			return;

		// camera is done snapping, so send the partition changed callback
		if(OnPartitionChanged != null)
			OnPartitionChanged(this, new PartitionChangedArgs(oldPartition, currentPartition));		
	}

	/// <summary>
	/// Changes the partition.
	/// </summary>
	/// <param name="targetPartition">Target partition.</param>
	private void ChangePartition(int targetPartition){
		// check to make sure the move is legal (i.e. within bounds)
		if(targetPartition >= firstPartition && targetPartition <= lastPartition){
			int oldPartition = currentPartition;
			currentPartition = targetPartition;
			
			// the partition changed, so snap the camera
			SnapCamera(oldPartition);
			
			// also send a callback that the partition is in the process of changing
			if(OnPartitionChanging != null)
				OnPartitionChanging(this, new PartitionChangedArgs(oldPartition, currentPartition));			
		}
	}	
    
	/// <summary>
	/// The user has attempted to initiate a change in
	/// partition; this function makes sure that it is
	/// a legal move.
	/// </summary>
	/// <param name="targetPartition">Target partition.</param>
	/// <param name="panDirection">Pan direction.</param>
	/// <param name="swipeTime">Swipe time.</param>
	private bool CanMoveToPartition(int targetPartition, RoomDirection panDirection, float swipeTime){	
		bool retVal = true;
			
		// then check to make sure the gating manager is okay with the move
		if(GatingManager.Instance.CanEnterRoom(currentPartition, panDirection) == false)
			retVal = false;
			
		// also check to make sure that the HUD animator is not animating
		if(HUDUIManager.Instance && HUDUIManager.Instance.hudAnimator && 
			HUDUIManager.Instance.hudAnimator.AreSpawnedSprites())
			retVal = false;
		
		// if the user is in deco mode and the room they are moving to has an active gate, illegal move
		if(EditDecosUIManager.Instance && EditDecosUIManager.Instance.IsOpen() && 
			GatingManager.Instance.HasActiveGate(targetPartition))
			retVal = false;
		
		// if the shop is open, no movement allowed
		if(StoreUIManager.Instance && StoreUIManager.Instance.IsOpen())
			retVal = false;
		
		// if we get here, the move is valid
		return retVal;
	}

	//This method can only be used in GameTutorial_SmokeIntro
	//It doesn't check click manager because we need the user to swipe left during
	//the tutorial. 
	public void TutorialSwipeLeft(){
		if(CanMoveToPartition(GetTargetPartition(1, RoomDirection.Left), RoomDirection.Left, -1)){
			ChangePartition(GetTargetPartition(1, RoomDirection.Left));
		}
	}

	public void MoveOneRoomToRight(){
		if(CanMoveToPartition(GetTargetPartition(1, RoomDirection.Left), RoomDirection.Left, -1)){
			ChangePartition(GetTargetPartition(1, RoomDirection.Left));
		}
	}

	public void MoveOneRoomToLeft(){
		if(CanMoveToPartition(GetTargetPartition(1, RoomDirection.Right), RoomDirection.Right, -1)){
			ChangePartition(GetTargetPartition(1, RoomDirection.Right));
		}
	}

	public bool CanDecoModeMoveToRight(){
		int targetPartition = GetTargetPartition(1, RoomDirection.Left);
		bool retVal = true;

		if(EditDecosUIManager.Instance && EditDecosUIManager.Instance.IsOpen() && 
			GatingManager.Instance.HasActiveGate(targetPartition))
				retVal = false;

		return retVal;
	}
	
	private void CheckArrowKeys(){
		// do a check here to see if the clickmanager can respond to movement, if it can't, don't move
		if(!ClickManager.Instance.CanRespondToTap(mainCamera.gameObject, ClickLockExceptions.Moving))
			return;
			
		if(Input.GetKeyDown(KeyCode.RightArrow)){
			if(CanMoveToPartition(GetTargetPartition(1, RoomDirection.Left), RoomDirection.Left, -1)){
				ChangePartition(GetTargetPartition(1, RoomDirection.Left));
			}
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow)){
			if(CanMoveToPartition(GetTargetPartition(1, RoomDirection.Right), RoomDirection.Right, -1)){
				ChangePartition(GetTargetPartition(1, RoomDirection.Right));       
			}
		}
	}

	/// <summary>
	/// Gets the target partition.
	/// Given a direction and a distance, what
	/// partition is the target?
	/// </summary>
	/// <returns>The target partition.</returns>
	/// <param name="moves">Moves.</param>
	/// <param name="swipeDirection">Swipe direction.</param>
	private int GetTargetPartition(int moves, RoomDirection swipeDirection){
		int change = swipeDirection == RoomDirection.Left ? moves : -moves;
		int target = currentPartition + change;
		
		return target;
	}

    
	/// <summary>
	/// Sets the camera to partition.
	/// </summary>
	/// <param name="partition">Partition.</param>
	private void SetCameraToPartition(int partition){
		currentPartition = partition;
		float cameraX = partition * partitionOffset;
		transform.position = new Vector3(cameraX, transform.position.y, transform.position.z);
	}
}
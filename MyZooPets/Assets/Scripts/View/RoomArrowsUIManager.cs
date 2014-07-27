using UnityEngine;
using System;
using System.Collections;

public class RoomArrowsUIManager : Singleton<RoomArrowsUIManager> {

	public TweenToggleDemux roomArrowsDemux;
	public TweenToggle leftArrowTween;
	public TweenToggle rightArrowTween;

	void Start(){
		CameraManager.Instance.GetPanScript().OnPartitionChanged += ShowPanel;
		Invoke("ShowPanel", 0.5f);
	}

	void OnDestroyed(){
		CameraManager.Instance.GetPanScript().OnPartitionChanged -= ShowPanel;
	}

	private void ShowPanel(object sender, EventArgs args){
		ShowPanel();
	}
	
	// Shows both arrows
	public void ShowPanel(){
		int currentPartition = CameraManager.Instance.GetPanScript().currentPartition;
		int firstPartition = CameraManager.Instance.GetPanScript().firstPartition;
		int lastPartition = CameraManager.Instance.GetPanScript().lastPartition;

		if(currentPartition == firstPartition){
			ShowRightArrow();
		}
		else if(currentPartition == lastPartition){
			ShowLeftArrow();
		}
		else{
			//check for gating
			bool isEnabled = Constants.GetConstant<bool>("GatingEnabled");
			bool canEnterRightRoom = GatingManager.Instance.CanEnterRoom(currentPartition, RoomDirection.Left);
			if(!isEnabled || canEnterRightRoom)
				ShowBothArrows();
			else
				ShowLeftArrow();
		}
	}

	// Hides both arrows
	public void HidePanel(){
		leftArrowTween.Hide();
		rightArrowTween.Hide();
	}

	public void ShowBothArrows(){
		leftArrowTween.Show();
		rightArrowTween.Show();
	}

	// Shows left arrow
	public void ShowLeftArrow(){
		leftArrowTween.Show();
		rightArrowTween.Hide();
	}

	// Shows right arrow
	public void ShowRightArrow(){
		rightArrowTween.Show();
		leftArrowTween.Hide();
	}

	public void RightArrowClicked(GameObject sender){
		CameraManager.Instance.GetPanScript().RightRoom();
	}

	public void LeftArrowClicked(GameObject sender){
		CameraManager.Instance.GetPanScript().LeftRoom();
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Show all")){
//			ShowPanel();
//		}
//		else if(GUI.Button(new Rect(200, 100, 100, 100), "Hide all")){
//			HidePanel();
//		}
//		else if(GUI.Button(new Rect(300, 100, 100, 100), "Show Left")){
//			ShowLeftArrow();
//		}
//		else if(GUI.Button(new Rect(400, 100, 100, 100), "Show Right")){
//			ShowRightArrow();
//		}
//	}
}

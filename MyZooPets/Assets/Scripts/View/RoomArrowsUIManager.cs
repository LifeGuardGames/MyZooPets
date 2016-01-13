using UnityEngine;
using System;
using System.Collections;

public class RoomArrowsUIManager : Singleton<RoomArrowsUIManager> {

	public TweenToggleDemux roomArrowsDemux;
	public TweenToggle leftArrowTween;
	public TweenToggle rightArrowTween;
	public LgButton rightArrowButtonScript;

	void Start(){
		CameraManager.Instance.PanScript.OnPartitionChanged += ShowPanel;


		Invoke("ShowPanel", 0.5f);
	}

	void OnDestroyed(){
		CameraManager.Instance.PanScript.OnPartitionChanged -= ShowPanel;
	}

	// For use in tutorial only
	public LgButton GetRightArrowReference(){
		return rightArrowButtonScript;
	}

	public void ShowPanel(object sender, EventArgs args){
		ShowPanel();
	}
	
	// Shows both arrows
	public void ShowPanel(){
		//hacky... need to refactor TutorialManager so that it's child class can also be an instance
//		bool isFlameCrystalTutorialDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_FLAME_CRYSTAL);
//		if(TutorialManager.Instance.IsTutorialActive() || !isFlameCrystalTutorialDone){
//			return;
//		};

		if(TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive()) return;

		PanToMoveCamera panScript = CameraManager.Instance.PanScript;
		int currentLocalPartition = panScript.currentLocalPartition;
		int firstPartition = panScript.firstPartition;
		int lastPartition = panScript.lastPartition;
		bool isEnabled = Constants.GetConstant<bool>("GatingEnabled"); //check for gating

		//deco mode specific checks
		if(DecoInventoryUIManager.Instance && DecoInventoryUIManager.Instance.IsOpen()){
			//first partition
			if(currentLocalPartition == firstPartition){
				if(panScript.CanDecoModeMoveToRight())
					ShowRightArrow();
			}
			//last partition
			else if(currentLocalPartition == lastPartition){
				ShowLeftArrow();
			}
			//in between partitions
			else{
				if(!isEnabled || panScript.CanDecoModeMoveToRight())
					ShowBothArrows();
				else
					ShowLeftArrow();
			}
		}
		//regular mode (all non deco) checks
		else{
			if(currentLocalPartition == firstPartition){
				ShowRightArrow();
			}
			else if(currentLocalPartition == lastPartition){
				ShowLeftArrow();
			}
			else{

				bool canEnterRightRoom = GatingManager.Instance.CanEnterRoom(currentLocalPartition, RoomDirection.Left);
				
				if(!isEnabled || canEnterRightRoom){
					ShowBothArrows();
				}
				else
					ShowLeftArrow();
			}
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
		CameraManager.Instance.PanScript.MoveOneRoomToRight();
	}

	public void LeftArrowClicked(GameObject sender){
		CameraManager.Instance.PanScript.MoveOneRoomToLeft();
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

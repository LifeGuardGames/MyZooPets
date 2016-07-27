using UnityEngine;
using UnityEngine.UI;
using System;

public class RoomArrowsUIManager : Singleton<RoomArrowsUIManager> {

	public TweenToggleDemux roomArrowsDemux;
	public TweenToggle leftArrowTween;
	public TweenToggle rightArrowTween;
	public Button rightArrowObject;
	private int endOfHouseParition = 4;
	private int currentPartition;

	void Start(){
		CameraManager.Instance.PanScript.OnPartitionChanged += ShowPanel;
		Invoke("ShowPanel", 0.5f);
	}

	void OnDestroyed(){
		CameraManager.Instance.PanScript.OnPartitionChanged -= ShowPanel;
	}

	// For use in tutorial only
	public Button GetRightArrowReference(){
		return rightArrowObject;
	}

	public void ShowPanel(object sender, EventArgs args){
		ShowPanel();
	}
	
	// Shows both arrows
	public void ShowPanel(){
		if(TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive()) return;

		PanToMoveCamera panScript = CameraManager.Instance.PanScript;
		int currentLocalPartition = panScript.currentLocalPartition;
		currentPartition = currentLocalPartition;
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
		if(rightArrowTween.gameObject.transform.childCount > 0) {
			rightArrowTween.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = SpriteCacheManager.GetSprite("navArrowRight");
		}
		//rightArrowTween.Show();
	}

	// Shows left arrow
	public void ShowLeftArrow(){
		leftArrowTween.Show();
		if(currentPartition == endOfHouseParition) {
			rightArrowTween.Hide();
		}
		else {
			if(rightArrowTween.gameObject.transform.childCount > 0) {
				rightArrowTween.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = SpriteCacheManager.GetSprite("navArrowRightInactive");
			}
		}

	}

	// Shows right arrow
	public void ShowRightArrow(){
		rightArrowTween.Show();
		leftArrowTween.Hide();
	}

	public void OnRightArrowClicked(){
		CameraManager.Instance.PanScript.MoveOneRoomToRight();
	}

	public void OnLeftArrowClicked(){
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

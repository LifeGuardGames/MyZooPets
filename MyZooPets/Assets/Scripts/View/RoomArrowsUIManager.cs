using UnityEngine.UI;
using UnityEngine;
using System;

public class RoomArrowsUIManager : Singleton<RoomArrowsUIManager> {
	public TweenToggleDemux roomArrowsDemux;
	public TweenToggle leftArrowTween;
	public TweenToggle rightArrowTween;
	public Image rightArrowSprite;
	public Sprite rightArrowActiveRef;
	public Sprite rightArrowInactiveRef;

	public Button rightArrowObject;
	public Button RightArrowObject {
		get { return rightArrowObject; }
	}

	void Start() {
		CameraManager.Instance.PanScript.OnPartitionChanged += ShowPanel;
		Invoke("ShowPanel", 0.5f);
	}

	void OnDestroyed() {
		CameraManager.Instance.PanScript.OnPartitionChanged -= ShowPanel;
	}

	public void ShowPanel(object sender, EventArgs args) {
		ShowPanel();
	}

	// Shows both arrows
	public void ShowPanel() {
		if(TutorialManager.Instance && TutorialManager.Instance.IsTutorialActive()) {
			return;
		}

		PanToMoveCamera panScript = CameraManager.Instance.PanScript;
		int currentLocalPartition = panScript.currentLocalPartition;
		int firstPartition = panScript.firstPartition;
		int lastPartition = panScript.lastPartition;
		bool isEnabled = Constants.GetConstant<bool>("GatingEnabled"); //check for gating

		//deco mode specific checks
		if(DecoModeUIManager.Instance && DecoModeUIManager.Instance.IsOpen) {
			//first partition
			if(currentLocalPartition == firstPartition) {
				if(panScript.CanDecoModeMoveToRight()) {
					ShowRightArrowOnly();
				}
			}
			//last partition
			else if(currentLocalPartition == lastPartition) {
				ShowLeftArrowOnly();
			}
			//in between partitions
			else {
				if(!isEnabled || panScript.CanDecoModeMoveToRight()) {
					ShowBothArrows(false);
				}
				else {
					ShowLeftArrowOnly();
				}
			}
		}
		//regular mode (all non deco) checks
		else {
			if(currentLocalPartition == firstPartition) {
				ShowRightArrowOnly();
			}
			else if(currentLocalPartition == lastPartition) {
				ShowLeftArrowOnly();
			}
			else {
				bool canEnterRightRoom = GatingManager.Instance.CanEnterRoom(currentLocalPartition, RoomDirection.Left);
				ShowBothArrows(!canEnterRightRoom);
			}
		}
	}

	public void HidePanel() {
		leftArrowTween.Hide();
		rightArrowTween.Hide();
	}

	private void ShowBothArrows(bool isGatingRoom) {
		rightArrowSprite.sprite = isGatingRoom ? rightArrowInactiveRef : rightArrowActiveRef;
        rightArrowTween.Show();
		leftArrowTween.Show();
	}

	private void ShowLeftArrowOnly() {
		rightArrowTween.Hide();
		leftArrowTween.Show();
	}

	public void ShowRightArrowOnly() {
		rightArrowSprite.sprite = rightArrowActiveRef;
		rightArrowTween.Show();
		leftArrowTween.Hide();
	}

	public void OnRightArrowClicked() {
		CameraManager.Instance.PanScript.MoveOneRoomToRight();
	}

	public void OnLeftArrowClicked() {
		CameraManager.Instance.PanScript.MoveOneRoomToLeft();
	}
}

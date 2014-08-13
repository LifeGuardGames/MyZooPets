using UnityEngine;
using System;
using System.Collections;

public class MiniPetHUDUIManager : SingletonUI<MiniPetHUDUIManager> {
	public static EventHandler<EventArgs> OnLevelUpAnimationCompleted;

	public UILabel nameLabel;
	public UILabel levelLabel;
	public UILabel progressLabel;
	public UISlider levelSlider;
	public GameObject tickleCheckBox;
	public GameObject cleanCheckBox;
	public Animation levelUpAnimation;
	public GameObject tutorialParent;
	public GameObject petReference;

	private GameObject cleaningTutorialObject;
	private GameObject ticklingTutorialObject;

	/// <summary>
	/// Gets or sets a value indicating whether feeding is lock because lv up
	/// animation is happening right now.
	/// </summary>
	/// <value><c>true</c> if this instance is level up animation lock on; otherwise, <c>false</c>.</value>
	public bool IsLevelUpAnimationLockOn {get; set;}

	/// <summary>
	/// Gets or sets the selected mini pet ID. Need to be set before the HUD
	/// panel is opened. 
	/// </summary>
	/// <value>The selected mini pet ID.</value>
	public string SelectedMiniPetID {get; set;}
	public string SelectedMiniPetName {get; set;}
	public GameObject SelectedMiniPetGameObject {get; set;}

	void Awake(){
		eModeType = UIModeTypes.MiniPet;
		IsLevelUpAnimationLockOn = false;
	}

	/// <summary>
	/// Level up animation completed. 
	/// Actually increase the level after level up animation is done.
	/// </summary>
	public void LevelUpAnimationCompleted(){
		MiniPetManager.Instance.IncreaseCurrentLevelAndResetCurrentFoodXP(SelectedMiniPetID);
		IsLevelUpAnimationLockOn = false;

		if(OnLevelUpAnimationCompleted != null)
			OnLevelUpAnimationCompleted(this, EventArgs.Empty);
	}

	protected override void _OpenUI(){
		this.GetComponent<TweenToggleDemux>().Show();
		MiniPetManager.MiniPetStatusUpdate += RefreshUI;
		RefreshUI(this, new MiniPetManager.StatusUpdateEventArgs());

		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		HUDUIManager.Instance.HidePanel();
		EditDecosUIManager.Instance.HideNavButton();
		InventoryUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.HidePanel();
		PetAnimationManager.Instance.DisableAnimation();
	}

	protected override void _CloseUI(){
		this.GetComponent<TweenToggleDemux>().Hide();
		MiniPetManager.MiniPetStatusUpdate -= RefreshUI;

		//Show other UI Objects
		NavigationUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		EditDecosUIManager.Instance.ShowNavButton();
		RoomArrowsUIManager.Instance.ShowPanel();
		PetAnimationManager.Instance.EnableAnimation();

		if(cleaningTutorialObject != null)
			Destroy(cleaningTutorialObject);

		if(ticklingTutorialObject != null)
			Destroy(ticklingTutorialObject);

		CameraManager.Instance.ZoomOutMove();
	}

	/// <summary>
	/// Refreshs the UI whenever MP data have been updated
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void RefreshUI(object sender, MiniPetManager.StatusUpdateEventArgs args){
		bool isTickled = MiniPetManager.Instance.IsTickled(SelectedMiniPetID);
		bool isCleaned = MiniPetManager.Instance.IsCleaned(SelectedMiniPetID);

		tickleCheckBox.SetActive(isTickled);
		cleanCheckBox.SetActive(isCleaned);

		nameLabel.text = SelectedMiniPetName;
		UpdateLevelUI();

		switch(args.UpdateStatus){
		case MiniPetManager.UpdateStatuses.LevelUp:
			levelUpAnimation.Play();
			IsLevelUpAnimationLockOn = true;
			break;
		case MiniPetManager.UpdateStatuses.FirstTimeCleaning:
			CheckForTicklingTutorial();
			break;
		case MiniPetManager.UpdateStatuses.FirstTimeTickling:
			if(ticklingTutorialObject != null)
				Destroy(ticklingTutorialObject.gameObject);
			break;
		}

		CheckForCleaningTutorial();
	}

	private void UpdateLevelUI(){
		int currentLevel = (int) MiniPetManager.Instance.GetCurrentLevel(SelectedMiniPetID);
		int currentFoodXP = MiniPetManager.Instance.GetCurrentFoodXP(SelectedMiniPetID);
		int nextLevelUpCondition = MiniPetManager.Instance.GetNextLevelUpCondition(SelectedMiniPetID);
		
		levelLabel.text = currentLevel.ToString();
		
		// update level slider
		if(nextLevelUpCondition != -1){
			levelSlider.sliderValue = (float) currentFoodXP / (float) nextLevelUpCondition;
			progressLabel.text = currentFoodXP + "/" + nextLevelUpCondition;
		}
		else{
			progressLabel.text = "Max Level";
		}
	}

	private void CheckForTicklingTutorial(){
		if(cleaningTutorialObject != null){
			Destroy(cleaningTutorialObject.gameObject);
		}
		
		//check if tickling tutorial needs to be started
		bool isFirstTimeTickling = MiniPetManager.Instance.IsFirstTimeTickling;
		bool isFirstTimeCleaning = MiniPetManager.Instance.IsFirstTimeCleaning;
		if(!isFirstTimeCleaning && isFirstTimeTickling){
			//spawn tutorial here
			if(ticklingTutorialObject == null){
				GameObject ticklingTutorial = (GameObject) Resources.Load("PressTut");
				
				Vector3 selectedMiniPetLocation = CameraManager.Instance.WorldToScreen(CameraManager.Instance.cameraMain, 
				                                                                       SelectedMiniPetGameObject.transform.position);
				selectedMiniPetLocation = CameraManager.Instance.TransformAnchorPosition(selectedMiniPetLocation, 
				                                                                         InterfaceAnchors.BottomLeft, 
				                                                                         InterfaceAnchors.Center);
				selectedMiniPetLocation.z = 0;
				ticklingTutorialObject = NGUITools.AddChild(tutorialParent, ticklingTutorial);
				ticklingTutorialObject.transform.localPosition = selectedMiniPetLocation;
			}
		}
	}
	
	/// <summary>
	/// Check if cleaning tutorial needs to be spawned.
	/// </summary>
	private void CheckForCleaningTutorial(){
		bool isFirstTimeCleaning = MiniPetManager.Instance.IsFirstTimeCleaning;

		if(isFirstTimeCleaning){
			if(cleaningTutorialObject == null){
				GameObject cleaningTutorial = (GameObject) Resources.Load("SwirlTut");
				
				Vector3 selectedMiniPetLocation = CameraManager.Instance.WorldToScreen(CameraManager.Instance.cameraMain, 
				                                                                       SelectedMiniPetGameObject.transform.position);
				selectedMiniPetLocation = CameraManager.Instance.TransformAnchorPosition(selectedMiniPetLocation, 
				                                                                         InterfaceAnchors.BottomLeft, 
				                                                                         InterfaceAnchors.Center);
				selectedMiniPetLocation.z = 0;
				cleaningTutorialObject = NGUITools.AddChild(tutorialParent, cleaningTutorial);
				cleaningTutorialObject.transform.localPosition = selectedMiniPetLocation;
			}
		}
	}

	private void OpenShop(){
		ClickManager.Instance.Lock(UIModeTypes.Store);
		
		// open the shop
		StoreUIManager.OnShortcutModeEnd += CloseShop;	
		StoreUIManager.Instance.OpenToSubCategory("Food", true);
	}

	private void CloseShop(object sender, EventArgs args){
		// pop the mode we pushed earlier from the click manager
		ClickManager.Instance.ReleaseLock();
		
		StoreUIManager.OnShortcutModeEnd -= CloseShop;
	}
}

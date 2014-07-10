using UnityEngine;
using System;
using System.Collections;

public class MiniPetHUDUIManager : SingletonUI<MiniPetHUDUIManager> {

	public UILabel nameLabel;
	public UILabel levelLabel;
	public UILabel progressLabel;
	public UISlider levelSlider;
	public GameObject tickleCheckBox;
	public GameObject cleanCheckBox;
	public Animation levelUpAnimation;
	public GameObject tutorialParent;

	private GameObject cleaningTutorialObject;

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
	/// Actually increase the level after level up animation is done
	/// </summary>
	public void LevelUpAnimationCompleted(){
		MiniPetManager.Instance.IncreaseCurrentLevelAndResetCurrentFoodXP(SelectedMiniPetID);
		IsLevelUpAnimationLockOn = false;
	}

	protected override void _OpenUI(){
		this.GetComponent<TweenToggleDemux>().Show();
		MiniPetManager.MiniPetStatusUpdate += RefreshUI;
		RefreshUI(this, new MiniPetManager.MiniPetStatusUpdateEventArgs());

		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		HUDUIManager.Instance.HidePanel();
		EditDecosUIManager.Instance.HideNavButton();
		InventoryUIManager.Instance.ShowPanel();
	}

	protected override void _CloseUI(){
		this.GetComponent<TweenToggleDemux>().Hide();
		MiniPetManager.MiniPetStatusUpdate -= RefreshUI;

		//Show other UI Objects
		NavigationUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		EditDecosUIManager.Instance.ShowNavButton();

		CameraManager.Instance.ZoomOutMove();
	}

	private void RefreshUI(object sender, MiniPetManager.MiniPetStatusUpdateEventArgs args){
		bool isTickled = MiniPetManager.Instance.IsTickled(SelectedMiniPetID);
		bool isCleaned = MiniPetManager.Instance.IsCleaned(SelectedMiniPetID);
		bool isFirstTimeCleaning = MiniPetManager.Instance.IsFirstTimeCleaning(SelectedMiniPetID);

		tickleCheckBox.SetActive(isTickled);
		cleanCheckBox.SetActive(isCleaned);

		nameLabel.text = SelectedMiniPetName;

		int currentLevel = (int) MiniPetManager.Instance.GetCurrentLevel(SelectedMiniPetID);
		int currentFoodXP = MiniPetManager.Instance.GetCurrentFoodXP(SelectedMiniPetID);
		int nextLevelUpCondition = MiniPetManager.Instance.GetNextLevelUpCondition(SelectedMiniPetID);

		levelLabel.text = currentLevel.ToString();

		if(nextLevelUpCondition != -1){
			levelSlider.sliderValue = (float) currentFoodXP / (float) nextLevelUpCondition;
			progressLabel.text = currentFoodXP + "/" + nextLevelUpCondition;
		}
		else{
			progressLabel.text = "Max Level";
		}

		// level up handler
		if(args.UpdateStatus == "levelUp"){
			levelUpAnimation.Play();
			IsLevelUpAnimationLockOn = true;
		}

		// tutorial handler
		if(isFirstTimeCleaning){
			if(cleaningTutorialObject == null){
				GameObject cleaningTutorial = (GameObject) Resources.Load("SwirlTut");
				
				Vector3 selectedMiniPetLocation = CameraManager.Instance.WorldToScreen(CameraManager.Instance.cameraMain, 
				                                                                       SelectedMiniPetGameObject.transform.position);
				selectedMiniPetLocation = CameraManager.Instance.TransformAnchorPosition(selectedMiniPetLocation, 
				                                                                         InterfaceAnchors.BottomLeft, 
				                                                                         InterfaceAnchors.Center);
				selectedMiniPetLocation.z = 0;
				cleaningTutorialObject = LgNGUITools.AddChildWithPosition(tutorialParent, cleaningTutorial);
			}
		}
		else{
			if(cleaningTutorialObject != null){
				Destroy(cleaningTutorialObject.gameObject);
			}
		}
	}
}

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

	/// <summary>
	/// Gets or sets the selected mini pet ID. Need to be set before the HUD
	/// panel is opened. 
	/// </summary>
	/// <value>The selected mini pet ID.</value>
	public string SelectedMiniPetID {get; set;}
	public string SelectedMiniPetName {get; set;}

	void Awake(){
		eModeType = UIModeTypes.MiniPet;
	}

	/// <summary>
	/// Level up animation completed. 
	/// Actually increase the level after level up animation is done
	/// </summary>
	public void LevelUpAnimationCompleted(){
		MiniPetManager.Instance.IncreaseCurrentLevelAndResetCurrentFoodXP(SelectedMiniPetID);
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

		tickleCheckBox.SetActive(isTickled);
		cleanCheckBox.SetActive(isCleaned);

		nameLabel.text = SelectedMiniPetName;

		int currentLevel = (int) MiniPetManager.Instance.GetCurrentLevel(SelectedMiniPetID);
		int currentFoodXP = MiniPetManager.Instance.GetCurrentFoodXP(SelectedMiniPetID);
		int nextLevelUpCondition = MiniPetManager.Instance.GetNextLevelUpCondition(SelectedMiniPetID);

		levelLabel.text = currentLevel.ToString();
		levelSlider.sliderValue = (float) currentFoodXP / (float) nextLevelUpCondition;
		progressLabel.text = currentFoodXP + "/" + nextLevelUpCondition;

		if(args.UpdateStatus == "levelUp"){
			levelUpAnimation.Play();
		}
	}
}

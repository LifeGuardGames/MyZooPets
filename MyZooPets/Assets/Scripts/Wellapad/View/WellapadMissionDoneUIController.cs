using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// WellapadCountdown
// This script is on the Wellapad and, when the user
// has completed all available missions/tasks and the
// "Done" screen is showing, this script will update
// the time remaining until new missions are available.
//---------------------------------------------------

public class WellapadMissionDoneUIController : MonoBehaviour {
	// label to update the timer
	public UILabel labelTimer;
	public UILabel labelTimerMessage;
	public UILabel labelStartLevel;
	public UILabel labelEndLevel;
	public UILabel labelMaxLevel;
	public UIAtlas atlasBadge;
	public UIAtlas atlasBedroom;
	public UIAtlas atlasItem;
	public UISlider sliderLevel;
	public GameObject gridUnlockPredictions;
	public GameObject unlockPredictionEntryPrefab;

	// Sprite set color on max level
	public UISprite startCircle;	// For reference
	public UISprite endCircle;		// For setting
	
	// bit of a hack - if this is true, the countdown was counting down
	private bool bCounting = false;

	void Awake(){
		//pet's name
		if(labelTimerMessage != null && VersionManager.IsLite()){
			string petName = DataManager.Instance.GameData.PetInfo.PetName;
			string rawText = Localization.Localize("WELLAPAD_LITE_INHALER");
			string message = String.Format(rawText, petName);
			labelTimerMessage.text = message;
		}
	}

	void Start(){
		WellapadUIManager.Instance.OnManagerOpen += RefreshLevelProgress;
		// HUDAnimator.OnLevelUp += RefreshLevelProgressOnLevelUp;
		HUDAnimator.OnLevelUp += RefreshUnlockPredictions;

		RefreshLevelProgress();
		RefreshUnlockPredictions(this, EventArgs.Empty);
	}

	void OnDestroy(){
		if(WellapadUIManager.Instance)
			WellapadUIManager.Instance.OnManagerOpen -= RefreshLevelProgress;
			
		// HUDAnimator.OnLevelUp -= RefreshLevelProgressOnLevelUp;
		HUDAnimator.OnLevelUp -= RefreshUnlockPredictions;
	}
	
	//---------------------------------------------------
	// Update()
	//---------------------------------------------------
	void Update() {
		//stop countdown if game is lite version
		// if(VersionManager.IsLite()) return;

		// if the player can use their inhaler, there is no countdown, so bail out
		if(PlayPeriodLogic.Instance.CanUseRealInhaler){
			// okay, so the player can use their inhaler...but were we previously counting down?
			if (bCounting){
				// if we were, stop
				bCounting = false;
				
				// and then do a refresh check for the Missions 
				WellapadMissionController.Instance.RefreshCheck();
			}
			return;
		}
		
		// also bail if the wellpaid isn't open
		if(WellapadUIManager.Instance.IsOpen() == false)
			return;
		
		// if we make it here, we are counting down
		bCounting = true;
		
		// otherwise the user CAN'T use their inhaler and the wellapad is open, so there is a countdown showing
		DateTime next = PlayPeriodLogic.Instance.NextPlayPeriod;
		DateTime now = LgDateTime.GetTimeNow();
		TimeSpan left = next - now;
		
		// format the time remaining
		string strTime = string.Format("{0:D2}:{1:D2}:{2:D2}", left.Hours, left.Minutes, left.Seconds);
		
		// set the label
		string strLabel = Localization.Localize("WELLAPAD_NO_MISSIONS_2");
		labelTimer.text = String.Format(strLabel, strTime);
	}

	private void RefreshLevelProgress(){
		if(!LevelLogic.Instance.IsAtMaxLevel()){
			int nextLevelPoints = LevelLogic.Instance.NextLevelPoints();
			float points = (float) StatsController.Instance.GetStat(HUDElementType.Points);
			sliderLevel.sliderValue = points/nextLevelPoints;

			int currentLevel = (int) LevelLogic.Instance.CurrentLevel;
			labelStartLevel.text = currentLevel.ToString();
			labelEndLevel.text = LevelLogic.Instance.NextLevel.ToString();
		}else{
			labelMaxLevel.gameObject.SetActive(true);
			labelStartLevel.text = "";
			labelEndLevel.text = "";
			sliderLevel.sliderValue = 1.0f;
			endCircle.color = startCircle.color;
		}
	}
	//----------------------------------------------
	// RefreshLevelProgress
	// Update the level progress bar
	//----------------------------------------------
	private void RefreshLevelProgress(object sender, UIManagerEventArgs args){
		if(args != null && args.Opening){
			RefreshLevelProgress();	
		}
	}

	private void RefreshLevelProgressOnLevelUp(object sender, EventArgs args){
		RefreshLevelProgress();	
	}

	//----------------------------------------------
	// RefreshUnlockPredictions()
	// Update the items/badge/flame that will be unlocked for next level
	//----------------------------------------------
	private void RefreshUnlockPredictions(object sender, EventArgs args){
		foreach(Transform child in gridUnlockPredictions.transform){
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}

		Badge badge = BadgeLogic.Instance.GetBadgeUnlockAtNextLevel();
		if(badge != null){
			GameObject go = LgNGUITools.AddChildWithPosition(gridUnlockPredictions, unlockPredictionEntryPrefab);
			UISprite sprite = go.GetComponent<UISprite>();
			sprite.atlas = atlasBadge; 
			sprite.spriteName = badge.TextureName;
		}

		Skill skill = FlameLevelLogic.Instance.GetSkillUnlockAtNextLevel();
		if(skill != null)	{
			GameObject go = LgNGUITools.AddChildWithPosition(gridUnlockPredictions, unlockPredictionEntryPrefab);
			UISprite sprite = go.GetComponent<UISprite>();
			sprite.atlas = atlasBedroom; 
			sprite.spriteName = skill.TextureName;
		}

		List<Item> items = ItemLogic.Instance.GetItemsUnlockAtNextLevel();
		foreach(Item item in items){
			GameObject go = LgNGUITools.AddChildWithPosition(gridUnlockPredictions, unlockPredictionEntryPrefab);
			UISprite sprite = go.GetComponent<UISprite>();
			sprite.atlas = atlasItem;
			sprite.spriteName = item.TextureName;
		}

		gridUnlockPredictions.GetComponent<UIGrid>().Reposition();
	}
}

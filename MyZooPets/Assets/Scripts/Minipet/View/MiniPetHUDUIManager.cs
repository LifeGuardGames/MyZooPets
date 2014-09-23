using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniPetHUDUIManager : SingletonUI<MiniPetHUDUIManager> {
	public static EventHandler<EventArgs> OnLevelUpAnimationCompleted;

	public UILabel nameLabel;
	public UILabel levelLabel;
	public UILabel progressLabel;
	public UISlider levelSlider;
	public GameObject tickleCheckBox;
	public GameObject cleanCheckBox;

	public UILabel labelFeedCount;
	public UILabel labelFeed;
	public UISprite spriteFeed;

	public Animation storeButtonPulseAnim;
	public GameObject storeButtonSunbeam;

	public Animation levelUpAnimation;
	public Animation levelUpDropdown;
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

		Invoke("CheckIfFirstTimeReceivingGems", 2.5f);

		if(OnLevelUpAnimationCompleted != null)
			OnLevelUpAnimationCompleted(this, EventArgs.Empty);
	}

	/// <summary>
	/// Checks if first time receiving gems. Show intro notification if first time
	/// </summary>
	private void CheckIfFirstTimeReceivingGems(){
		bool isFirstTime = MiniPetManager.Instance.IsFirstTimeReceivingGems;
		if(isFirstTime){
			Hashtable notificationEntry = new Hashtable();

			PopupNotificationNGUI.Callback button1Function = delegate(){
				OpenItemShop();
				MiniPetManager.Instance.IsFirstTimeReceivingGems = false;
			};

			PopupNotificationNGUI.Callback button2Function = delegate(){
				MiniPetManager.Instance.IsFirstTimeReceivingGems = false;
			};

			notificationEntry.Add(NotificationPopupFields.Type, NotificationPopupType.GemIntro);
			notificationEntry.Add(NotificationPopupFields.Button1Callback, button1Function);
			notificationEntry.Add(NotificationPopupFields.Button2Callback, button2Function);
		
			NotificationUIManager.Instance.AddToQueue(notificationEntry);
		}
	}

	protected override void _OpenUI(){
		this.GetComponent<TweenToggleDemux>().Show();
		MiniPetManager.MiniPetStatusUpdate += RefreshUI;
		RefreshUI(this, new MiniPetManager.StatusUpdateEventArgs());

		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		HUDUIManager.Instance.HidePanel();
		InventoryUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.HidePanel();
		PetAnimationManager.Instance.DisableVisibility();
		PetAudioManager.Instance.EnableSound = false;
	}

	protected override void _CloseUI(){
		this.GetComponent<TweenToggleDemux>().Hide();
		CheckStoreButtonPulse();
		MiniPetManager.MiniPetStatusUpdate -= RefreshUI;

		//Show other UI Objects
		NavigationUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.ShowPanel();
		PetAnimationManager.Instance.EnableVisibility();
		PetAudioManager.Instance.EnableSound = true;

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

		RefreshFoodItemUI();

		nameLabel.text = SelectedMiniPetName;
		UpdateLevelUI();

		switch(args.UpdateStatus){
		case MiniPetManager.UpdateStatuses.LevelUp:
			levelUpAnimation.Play();
			levelUpDropdown.Play();
			IsLevelUpAnimationLockOn = true;
			break;
		case MiniPetManager.UpdateStatuses.FirstTimeCleaning:
			if(cleaningTutorialObject != null){
				Destroy(cleaningTutorialObject.gameObject);
			}

			Invoke("CheckForTicklingTutorial", 2f);

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

	/// <summary>
	/// Opens the shop. Store button calls this function
	/// </summary>
	private void OpenShop(){
		this.GetComponent<TweenToggleDemux>().Hide();

		//sometimes this function will be called in a different mode, so we need
		//to make sure the UIs are handled appropriately
		bool isModeLockEmpty = ClickManager.Instance.IsModeLockEmpty;
		if(isModeLockEmpty){
			NavigationUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
		}

		ClickManager.Instance.Lock(UIModeTypes.Store);
		Invoke("OpenFoodShopAfterWaiting", 0.2f);
	}

	/// <summary>
	/// Opens the shop after waiting. So the MiniPetHUDUI can hide first before the
	/// store UI shows up
	/// </summary>
	private void OpenFoodShopAfterWaiting(){
		StoreUIManager.OnShortcutModeEnd += CloseShop;	
		StoreUIManager.Instance.OpenToSubCategory("Food", true);
	}

	/// <summary>
	/// Opens the item shop.
	/// </summary>
	private void OpenItemShop(){
		this.GetComponent<TweenToggleDemux>().Hide();
		ClickManager.Instance.Lock(UIModeTypes.Store);
		StoreUIManager.OnShortcutModeEnd += CloseShop;
		StoreUIManager.Instance.OpenToSubCategory("Items", true);
	}

	private void CloseShop(object sender, EventArgs args){
		StoreUIManager.OnShortcutModeEnd -= CloseShop;
		ClickManager.Instance.ReleaseLock();
	
		UIModeTypes currentMode = ClickManager.Instance.CurrentMode;
		if(currentMode == UIModeTypes.MiniPet){
			this.GetComponent<TweenToggleDemux>().Show();
			HUDUIManager.Instance.HidePanel();
		}

		CheckStoreButtonPulse();
	}

	/// <summary>
	/// Refreshs the food item UI
	/// This does all the check by itself so dont worry when calling this
	/// </summary>
	public void RefreshFoodItemUI(){
		if(MiniPetManager.Instance.CanModifyFoodXP(SelectedMiniPetID)){
			int currentFoodXP = MiniPetManager.Instance.GetCurrentFoodXP(SelectedMiniPetID);
			int nextLevelUpCondition = MiniPetManager.Instance.GetNextLevelUpCondition(SelectedMiniPetID);
			labelFeedCount.text = (nextLevelUpCondition - currentFoodXP).ToString();
			
			labelFeedCount.gameObject.SetActive(true);
			labelFeed.gameObject.SetActive(true);
			Item item = ItemLogic.Instance.GetItem(MiniPetManager.Instance.GetFoodPreference(SelectedMiniPetID));
			spriteFeed.spriteName = item.TextureName;
			spriteFeed.gameObject.SetActive(true);
			spriteFeed.GetComponent<SpriteResizer>().Resize();
		}
		else{
			labelFeedCount.gameObject.SetActive(false);
			labelFeed.gameObject.SetActive(false);
			spriteFeed.spriteName = null;
			spriteFeed.gameObject.SetActive(false);
		}
		CheckStoreButtonPulse();
	}

	/// <summary>
	/// Checks the store button pulse.
	/// This does all the check by itself so dont worry when calling this
	/// </summary>
	private void CheckStoreButtonPulse(){
		Item neededItem = ItemLogic.Instance.GetItem(MiniPetManager.Instance.GetFoodPreference(SelectedMiniPetID));
		bool isNeedItem = !DataManager.Instance.GameData.Inventory.InventoryItems.ContainsKey(neededItem.ID);

		if(isNeedItem && MiniPetHUDUIManager.Instance.IsOpen()){
			storeButtonPulseAnim.Play();
			storeButtonSunbeam.SetActive(true);
		}
		else{
			storeButtonPulseAnim.Stop();
			GameObjectUtils.ResetLocalScale(storeButtonPulseAnim.gameObject);
			storeButtonSunbeam.SetActive(false);
		}
	}
}

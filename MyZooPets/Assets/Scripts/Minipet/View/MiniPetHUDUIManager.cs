using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniPetHUDUIManager : SingletonUI<MiniPetHUDUIManager> {

	public static EventHandler<EventArgs> OnLevelUpAnimationCompleted;

	public UILabel nameLabel;
	public GameObject tickleCheckBox;
	public GameObject cleanCheckBox;

	public UILabel labelFeedCount;
	public UILabel labelFeed;
	public UISprite spriteFeed;
	public TweenToggle feedParent;

	public Animation storeButtonPulseAnim;
	public GameObject storeButtonSunbeam;

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

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.MiniPet;
		IsLevelUpAnimationLockOn = false;
	}

	#region Overridden functions
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
		feedParent.Hide();
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
	#endregion

	/// <summary>
	/// Refreshs the UI whenever MP data have been updated
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void RefreshUI(object sender, MiniPetManager.StatusUpdateEventArgs args){
		//bool isTickled = MiniPetManager.Instance.IsTickled(SelectedMiniPetID);
		//bool isCleaned = MiniPetManager.Instance.IsCleaned(SelectedMiniPetID);

		//tickleCheckBox.SetActive(isTickled);
		//cleanCheckBox.SetActive(isCleaned);

		RefreshFoodItemUI();

		nameLabel.text = SelectedMiniPetName;
		UpdateLevelUI();

		switch(args.UpdateStatus){
		case MiniPetManager.UpdateStatuses.LevelUp:
			// Start all level up logic here
			CloseUI();

			levelUpDropdown.Play();
			IsLevelUpAnimationLockOn = true;

			LevelUpAnimationCompleted();
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
	//	CheckForCleaningTutorial();
	}

	private void UpdateLevelUI(){
		int nextLevelUpCondition = MiniPetManager.Instance.GetNextLevelUpCondition(SelectedMiniPetID);

		// update level slider
		if(nextLevelUpCondition != -1){
			//TODO Need this?
		}
		else{// Max level
			//TODO Design what happens here
		}
	}

	/// <summary>
	/// Level up animation completed. 
	/// Actually increase the level after level up animation is done.
	/// </summary>
	public void LevelUpAnimationCompleted(){
		MiniPetManager.Instance.IncreaseCurrentLevelAndResetCurrentXP(SelectedMiniPetID);
		IsLevelUpAnimationLockOn = false;	// Unlocked immediately... save for future use

		if(OnLevelUpAnimationCompleted != null)
			OnLevelUpAnimationCompleted(this, EventArgs.Empty);
	}

	/*private void CheckForTicklingTutorial(){
		//check if tickling tutorial needs to be started
		//bool isFirstTimeTickling = MiniPetManager.Instance.IsFirstTimeTickling;
		//bool isFirstTimeCleaning = MiniPetManager.Instance.IsFirstTimeCleaning;
		if(!isFirstTimeCleaning && isFirstTimeTickling){
			//spawn tutorial here
			if(ticklingTutorialObject == null){
				GameObject ticklingTutorial = (GameObject) Resources.Load("PressTut");
				
				Vector3 selectedMiniPetLocation = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, 
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
	*/
	/// <summary>
	/// Check if cleaning tutorial needs to be spawned.
	/// </summary>
	/*private void CheckForCleaningTutorial(){
		bool isFirstTimeCleaning = MiniPetManager.Instance.IsFirstTimeCleaning;

		if(isFirstTimeCleaning){
			if(cleaningTutorialObject == null){
				GameObject cleaningTutorial = (GameObject) Resources.Load("SwirlTut");
				
				Vector3 selectedMiniPetLocation = CameraManager.Instance.WorldToScreen(CameraManager.Instance.CameraMain, 
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
*/
	/// <summary>
	/// Opens the shop. Store button calls this function
	/// </summary>
	private void OpenShop(){
		this.GetComponent<TweenToggleDemux>().Hide();
		feedParent.Hide();

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
		StoreUIManager.Instance.OpenToSubCategory("Food", true, StoreShortcutType.MinipetUIStoreButton);
	}

	private void CloseShop(object sender, EventArgs args){
		StoreUIManager.OnShortcutModeEnd -= CloseShop;
		ClickManager.Instance.ReleaseLock();
	
		UIModeTypes currentMode = ClickManager.Instance.CurrentMode;
		if(currentMode == UIModeTypes.MiniPet){
			this.GetComponent<TweenToggleDemux>().Show();
			if(MiniPetManager.Instance.CanModifyXP(SelectedMiniPetID)){
				feedParent.Show();
			}
			HUDUIManager.Instance.HidePanel();
		}

		CheckStoreButtonPulse();
	}

	/// <summary>
	/// Refreshs the food item UI
	/// This does all the check by itself so dont worry when calling this
	/// </summary>
	public void RefreshFoodItemUI(){
		if(MiniPetManager.Instance.CanModifyXP(SelectedMiniPetID)){
			int currentFoodXP = MiniPetManager.Instance.GetCurrentXP(SelectedMiniPetID);
			int nextLevelUpCondition = MiniPetManager.Instance.GetNextLevelUpCondition(SelectedMiniPetID);
			//labelFeedCount.text = (nextLevelUpCondition - currentFoodXP).ToString();
			labelFeedCount.text = (1).ToString();
			labelFeedCount.gameObject.SetActive(true);
			labelFeed.gameObject.SetActive(true);
			Item item = ItemLogic.Instance.GetItem(MiniPetManager.Instance.GetFoodPreference(SelectedMiniPetID));
			spriteFeed.spriteName = item.TextureName;
			spriteFeed.gameObject.SetActive(true);
			spriteFeed.GetComponent<SpriteResizer>().Resize();
			feedParent.Show();
		}
		else{
			feedParent.Hide();
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

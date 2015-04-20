using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniPetHUDUIManager : SingletonUI<MiniPetHUDUIManager> {

	public static EventHandler<EventArgs> OnLevelUpAnimationCompleted;

	public UILabel nameLabel;

	public UILabel labelFeedCount;
	public UILabel labelFeed;
	public UISprite spriteFeed;

	public TweenToggle feedTweenParent;	// Tweens dependent on state of pet
	public TweenToggle storeTweenParent;
	private TweenToggle contentTweenParent;

	public Animation storeButtonPulseAnim;
	public GameObject storeButtonSunbeam;

	public GameObject contentParent;
	public GameObject tutorialParent;
	public GameObject petReference;
	private GameObject content;

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
	}

	public void OpenUIMinipetType(MiniPetTypes type, Hashtable hash){
		Debug.Log("OPENING MINIPET TYPE");
		if(!PlayPeriodLogic.Instance.IsFirstPlayPeriod() || type == MiniPetTypes.Retention){
			GameObject contentPrefab;
			switch(type){
			case MiniPetTypes.Retention:
				if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("Critical")||DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart1")||DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey("TutorialPart2")){
					contentPrefab = Resources.Load("ContentParentRetention") as GameObject;
					content = GameObjectUtils.AddChildWithPositionAndScale(contentParent, contentPrefab);
					MiniPetRetentionUIController controller = content.GetComponent<MiniPetRetentionUIController>();
					controller.Initialize();
				}
				break;
			case MiniPetTypes.GameMaster:
				if(DataManager.Instance.GameData.Wellapad.CurrentTasks.ContainsKey(hash[0].ToString())){
					contentPrefab = Resources.Load("ContentParentGameMaster") as GameObject;
					content = GameObjectUtils.AddChildWithPositionAndScale(contentParent, contentPrefab);
					MiniPetGameMasterUIController controller2 = content.GetComponent<MiniPetGameMasterUIController>();
					controller2.Initialize(hash[0].ToString());
				}
				break;
			case MiniPetTypes.Merchant:
				contentPrefab = Resources.Load("ContentParentMerchant") as GameObject;
				content = GameObjectUtils.AddChildWithPositionAndScale(contentParent, contentPrefab);
				MiniPetMerchantUIController controller3 = content.GetComponent<MiniPetMerchantUIController>();
				ItemType itemType;
				switch (hash[1].ToString()){	// Item type
				case "Decorations":
					itemType = ItemType.Decorations;
					break;
				case "Usables":
					itemType = ItemType.Usables;
					break;
				case "Foods":
					itemType = ItemType.Foods;
					break;
				case "Accessories":
					itemType = ItemType.Accessories;
					break;
				default:
					Debug.LogError("Failed to find item type : " + hash[1].ToString());
					itemType = ItemType.Decorations;
					break;
				}
				controller3.Initialize(hash[0].ToString(), false, itemType);
				break;
			default:
				Debug.LogError("No minipet type found: " + type.ToString());
				return;
			}

			contentTweenParent = content.GetComponent<TweenToggle>();
			if(IsOpen() && (contentTweenParent != null)){	// Pet just finished eating, show asap HACK
				StartCoroutine(ShowContentHelper());
			}
		}
	}

	private IEnumerator ShowContentHelper(){
		yield return 0;
		contentTweenParent.Show();
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
		//contentParent.SetActive(true);
	}

	protected override void _CloseUI(){
		this.GetComponent<TweenToggleDemux>().Hide();

		feedTweenParent.Hide();
		storeTweenParent.Hide();

		CheckStoreButtonPulse();
		MiniPetManager.MiniPetStatusUpdate -= RefreshUI;
		PetSpeechManager.Instance.BeQuiet();
		//Show other UI Objects
		NavigationUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.ShowPanel();
		PetAnimationManager.Instance.EnableVisibility();
		PetAudioManager.Instance.EnableSound = true;
		DecoInventoryUIManager.Instance.HideDecoInventory();
		if(content != null){
			Destroy(content.gameObject);
		}
		//contentParent.SetActive(false);
		CameraManager.Instance.ZoomOutMove();
	}
	#endregion

	/// <summary>
	/// Refreshes the UI whenever MP data have been updated
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void RefreshUI(object sender, MiniPetManager.StatusUpdateEventArgs args){
		if(!PlayPeriodLogic.Instance.IsFirstPlayPeriod()){
			RefreshFoodItemUI();
		}
		else{
			Debug.LogWarning("First playperiod, not showing HUD because of tutorial");
		}
		nameLabel.text = SelectedMiniPetName;
		UpdateLevelUI();

		switch(args.UpdateStatus){
		case MiniPetManager.UpdateStatuses.LevelUp:
			LevelUpAnimationCompleted();
			break;
		}
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

		if(OnLevelUpAnimationCompleted != null){
			OnLevelUpAnimationCompleted(this, EventArgs.Empty);
		}
	}

	/// <summary>
	/// Opens the shop. Store button calls this function
	/// </summary>
	private void OpenShop(){
		this.GetComponent<TweenToggleDemux>().Hide();
		feedTweenParent.Hide();
		storeTweenParent.Hide();

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
		if(content != null){
			content.GetComponent<TweenToggle>().Hide();
		}
	}

	private void CloseShop(object sender, EventArgs args){
		StoreUIManager.OnShortcutModeEnd -= CloseShop;
		ClickManager.Instance.ReleaseLock();
	
		UIModeTypes currentMode = ClickManager.Instance.CurrentMode;
		if(currentMode == UIModeTypes.MiniPet){
			this.GetComponent<TweenToggleDemux>().Show();
			if(content != null){
				content.GetComponent<TweenToggle>().Show();
			}
			if(!MiniPetManager.Instance.CanModifyXP(SelectedMiniPetID)){
				feedTweenParent.Show();
				storeTweenParent.Show();
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
		if(SelectedMiniPetID != null){
			if(!DataManager.Instance.GameData.MiniPets.GetHunger(SelectedMiniPetID)){
				nameLabel.text = SelectedMiniPetName;
				//int currentFoodXP = MiniPetManager.Instance.GetCurrentXP(SelectedMiniPetID);
				//int nextLevelUpCondition = MiniPetManager.Instance.GetNextLevelUpCondition(SelectedMiniPetID);
				//labelFeedCount.text = (nextLevelUpCondition - currentFoodXP).ToString();
				labelFeedCount.text = (1).ToString();
				labelFeedCount.gameObject.SetActive(true);
				labelFeed.gameObject.SetActive(true);
				Item item = ItemLogic.Instance.GetItem(MiniPetManager.Instance.GetFoodPreference(SelectedMiniPetID));
				spriteFeed.spriteName = item.TextureName;
				spriteFeed.gameObject.SetActive(true);
				spriteFeed.GetComponent<SpriteResizer>().Resize();

				feedTweenParent.Show();
				storeTweenParent.Show();
			}
			else{
				feedTweenParent.Hide();
				storeTweenParent.Hide();
				Debug.Log("CHECKING OPEN MINIPET TYPE");
				if(contentTweenParent != null){
					contentTweenParent.Show();
				}

				// Only hide the inventory panel if it is not the merchant
				if(MiniPetManager.Instance.GetMinipetType(SelectedMiniPetID) != MiniPetTypes.Merchant){
					InventoryUIManager.Instance.HidePanel();
				}

				labelFeedCount.gameObject.SetActive(false);
				labelFeed.gameObject.SetActive(false);
				spriteFeed.spriteName = null;
				spriteFeed.gameObject.SetActive(false);
			}
			CheckStoreButtonPulse();
		}
	}

	/// <summary>
	/// Checks the store button pulse.
	/// This does all the check by itself so dont worry when calling this
	/// </summary>
	private void CheckStoreButtonPulse(){
		Item neededItem = ItemLogic.Instance.GetItem(MiniPetManager.Instance.GetFoodPreference(SelectedMiniPetID));
		bool isNeedItem = !DataManager.Instance.GameData.Inventory.InventoryItems.ContainsKey(neededItem.ID);
		bool petFed = !DataManager.Instance.GameData.MiniPets.GetHunger(SelectedMiniPetID);

		if(isNeedItem && MiniPetHUDUIManager.Instance.IsOpen() && petFed){
			storeButtonPulseAnim.Play();
			storeButtonSunbeam.SetActive(true);
		}
		else{
			storeButtonPulseAnim.Stop();
			GameObjectUtils.ResetLocalScale(storeButtonPulseAnim.gameObject);
			storeButtonSunbeam.SetActive(false);
		}
	}

	public bool HasContent(){
		return content == null ? false : true;
	}
}

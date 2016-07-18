using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Shortcut mode types into store sub panel
public enum StoreShortcutType{
	None,
//	FlameCrystalNeededNotification,
	DecorationUIStoreButton,
	DecorationUIStoreButtonTutorial,
	MinipetUIStoreButton,
	NeedFoodPetSpeech,
	NeedFoodTutorial,
	NeedEmergencyInhalerPetSpeech,
	SickNotification,
}

public class StoreUIManager : SingletonUI<StoreUIManager>{
	public static EventHandler<EventArgs> OnShortcutModeEnd;
	public static EventHandler<EventArgs> OnDecorationItemBought;
	public GameObject grid;
	public GameObject itemStorePrefab;		//basic ui setup for an individual item
	public GameObject itemStorePrefabStats;	// a stats item entry
	public GameObject itemSpritePrefab; 	// item sprite for inventory
	public GameObject storeBasePanel; 		//Where you choose item category
	public GameObject storeSubPanel; 		//Where you choose item sub category
	public GameObject storeSubPanelBg;
	public GameObject itemArea; 			//Where the items will be display
	public GameObject tabArea; 				//Where all the tabs for sub category are
	public GameObject storeBgPanel;			// the bg of the store (sub panel and base panel)
	public GameObject backButton; 			// exit button reference
	public GameObject prevTab; 
	// store related sounds
	public string soundChangeTab;
	public string soundBuy;
	private bool isShortcutMode; 			//True: open store directly to specific item category
											//False: open the store base panel first	
	private StoreShortcutType shortcutType = StoreShortcutType.None;	// Store where shortcut was from

	private bool changePage;
	private string currentPage; //The current category. i.e food, usable, decorations
	private string currentTab; //The current sub category. only decorations have sub cat right now
	private LgTuple <string,string> secretCodeEntry;
	public PositionTweenToggle codeEntryUi;
	public PositionTweenToggle codeRedeemedUi;

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Store;
	}
	
	protected override void Start(){
		base.Start();
		// Reposition all the things nicely to stretch to the end of the screen
		secretCodeEntry = new LgTuple<string,string> ("","");
		// Position the UIPanel clipping range
		UIPanel itemAreaPanel = itemArea.GetComponent<UIPanel>();
		Vector4 oldRange = itemAreaPanel.clipRange;
		
		// The 52 comes from some wierd scaling issue.. not sure what it is but compensate now
		itemAreaPanel.transform.localPosition = new Vector3(52f, itemAreaPanel.transform.localPosition.y, 0f);
		itemAreaPanel.clipRange = new Vector4(52f, oldRange.y, (float)(CameraManager.Instance.NativeWidth), oldRange.w);
		
		// Position the grid origin to the left of the screen
		Vector3 gridPosition = grid.transform.localPosition;
		grid.transform.localPosition = new Vector3(
			(-1f * (CameraManager.Instance.NativeWidth / 2)) - itemArea.transform.localPosition.x,
			gridPosition.y, gridPosition.z);
	}

	/// <summary>
	/// Gets the exit button.
	/// </summary>
	/// <returns>The exit button.</returns>
	public GameObject GetBackButton(){
		return backButton;
	}

	/// <summary>
	/// Enables the tab area. Use in tutorial
	/// </summary>
	public void EnableTabArea(){
		foreach(Transform tabTransform in tabArea.transform){
			tabTransform.FindChild("Tab").gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Disables the tab area. Use in tutorial
	/// </summary>
	public void DisableTabArea(){
		foreach(Transform tabTransform in tabArea.transform){
			tabTransform.FindChild("Tab").gameObject.SetActive(false);
		}
	}

	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		RoomArrowsUIManager.Instance.HidePanel();
		HUDUIManager.Instance.ShowLabels();
		storeBasePanel.GetComponent<TweenToggleDemux>().Show();
		storeBgPanel.GetComponent<TweenToggleDemux>().Show();
	}

	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel();
		HUDUIManager.Instance.HideLabels();
		storeBasePanel.GetComponent<TweenToggleDemux>().Hide();
		storeBgPanel.GetComponent<TweenToggleDemux>().Hide();
		storeSubPanel.GetComponent<TweenToggleDemux>().Hide();
	}

	#region Store shortcut mode used by PetSpeechAI
	public void OpenToSubCategoryFoodWithLockAndCallBack(){
		ClickManager.Instance.Lock(UIModeTypes.Store, GetClickLockExceptions());
		OnShortcutModeEnd += ShortcutModeEnded;
		OpenToSubCategory("Food", true, StoreShortcutType.NeedFoodPetSpeech);
	}

	public void OpenToSubCategoryItemsWithLockAndCallBack(){
		ClickManager.Instance.Lock(UIModeTypes.Store, GetClickLockExceptions());
		OnShortcutModeEnd += ShortcutModeEnded;
		OpenToSubCategory("Items", true, StoreShortcutType.NeedEmergencyInhalerPetSpeech);
	}

	private void ShortcutModeEnded(object sender, EventArgs args){
		ClickManager.Instance.ReleaseLock();
		OnShortcutModeEnd -= ShortcutModeEnded;
	}
	#endregion
	
	/// <summary>
	/// Opens to sub category. Special function used to open the store UI straight
	/// up to a certain category
	/// </summary>
	/// <param name="category">Category.</param>
	/// <param name="isShortCut">If set to <c>true</c> is short cut.</param>
	public void OpenToSubCategory(string category, bool isShortCut = false, StoreShortcutType shortcutType = StoreShortcutType.None){		
		// this is a bit of a hack, but basically there are multiple ways to open the shop.  One way is a shortcut in that it
		// bypasses the normal means of opening a shop, so we need to do some special things in this case
		isShortcutMode = isShortCut;
		if(isShortcutMode){

			// Check to make sure we have a shortcut type and cache it
			if(shortcutType != StoreShortcutType.None){
				this.shortcutType = shortcutType;
			}
			else{
				Debug.LogError("No shortcut type supplied with store shortcut call");
			}

			// If we are shortcutting, we have to tween the bg in now
			storeBgPanel.GetComponent<TweenToggleDemux>().Show();
			HUDUIManager.Instance.ShowPanel();
			NavigationUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();
		}
		CreateSubCategoryItemsWithString(category, shortcutType); 
	}
	
	/// <summary>
	/// This function is called when buying an item. I creates an icon for the item
	/// and move it to Inventory
	/// </summary>
	/// <param name="itemData">Item data.</param>
	/// <param name="sprite">Sprite.</param>
	public void OnBuyAnimation(Item itemData, GameObject sprite){
		Vector3 origin = new Vector3(sprite.transform.position.x, sprite.transform.position.y, -0.1f);
		string itemID = sprite.transform.parent.transform.parent.name;
		Vector3 itemPosition = origin;

		//-0.22
		// depending on what type of item the user bought, the animation has the item going to different places
		ItemType eType = itemData.Type;
		switch(eType){
		case ItemType.Decorations:
			itemPosition = DecoInventoryUIManager.Instance.GetPositionOfDecoInvItem(itemID);
			break;
		default:
			itemPosition = InventoryUIManager.Instance.GetPositionOfInvItem(itemID);
			break;
		}
		
		//adjust moving speed here
		float speed = 1f;

		//Change the 3 V3 to where icon should move
		Vector3[] path = new Vector3[4];
		path[0] = origin;
		path[1] = origin + new Vector3(0f, 1.5f, -0.1f);
		path[2] = origin;
		path[3] = itemPosition;


		Hashtable optional = new Hashtable();
		GameObject animationSprite = NGUITools.AddChild(storeSubPanel, itemSpritePrefab);
		
		// hashtable for completion params for the callback (stash the icon we are animating)
		Hashtable completeParamHash = new Hashtable();
		completeParamHash.Add("Icon", animationSprite);		

		optional.Add("ease", LeanTweenType.easeOutQuad);
		optional.Add("onComplete", "DestroySprite");
		optional.Add("onCompleteTarget", gameObject);
		optional.Add("onCompleteParam", completeParamHash);
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(90, 90, 1);
		animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(animationSprite, path, speed, optional);
	}

	//---------------------------------------------------
	// DestroySprite()
	// Callback for buy animation -- will destroy the
	// sprite icon clone we create and animated.
	//---------------------------------------------------
	public void DestroySprite(Hashtable hash){
		// delete the icon we moved
		if(hash.ContainsKey("Icon")){
			GameObject goSprite = (GameObject)hash["Icon"];
			Destroy(goSprite);
		}	
	}

	/// <summary>
	/// Raises the buy button event.
	/// </summary>
	/// <param name="button">Button.</param>
	public void OnBuyButton(GameObject button){
		Transform buttonParent = button.transform.parent.parent;
		string itemID = buttonParent.name;
		Item itemData = ItemLogic.Instance.GetItem(itemID);
		switch(itemData.CurrencyType){
		case CurrencyTypes.WellaCoin:
			if(DataManager.Instance.GameData.Stats.Stars >= itemData.Cost){
				//Special case to handle here. Since only one wallpaper can be used at anytime
				//There is no point for the user to buy more than one of each diff wallpaper
				if(itemData.Type == ItemType.Decorations){
					DecorationItem decoItem = (DecorationItem)itemData;
					
					if(decoItem.DecorationType == DecorationTypes.Wallpaper){
						UIImageButton buyButton = button.GetComponent<UIImageButton>();
						
						//Disable the buy button so user can't buy the same wallpaper anymore 
						if(buyButton)
							buyButton.isEnabled = false;
					}
				}

				/////////// SECRET CODE UNLOCK /////////////
				if(itemData.ID == DataManager.Instance.GameData.Inventory.secretCode.Item1 && secretCodeEntry.Item1 != DataManager.Instance.GameData.Inventory.secretCode.Item1){
					secretCodeEntry.setItem1(itemData.ID);
					Debug.Log("correct item 1");
				}
				else if (secretCodeEntry.Item1 == DataManager.Instance.GameData.Inventory.secretCode.Item1 && itemData.ID == DataManager.Instance.GameData.Inventory.secretCode.Item2){
					ShowCodeEntry();
					Debug.Log("Showing Code Entry");
				}
				else{
					secretCodeEntry.setItem1("");
					Debug.Log("incorrect");
				}
				////////////////////////////////////////////

				InventoryLogic.Instance.AddItem(itemID, 1);
				StatsController.Instance.ChangeStats(deltaStars: itemData.Cost * -1);
				OnBuyAnimation(itemData, buttonParent.gameObject.FindInChildren("ItemTexture"));
				
				//Analytics
				Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_BOUGHT, itemData.Type, itemData.ID);
				
				// play a sound since an item was bought
				AudioManager.Instance.PlayClip(soundBuy);
			}
			else{
				HUDUIManager.Instance.PlayNeedMoneyAnimation();
				AudioManager.Instance.PlayClip("buttonDontClick");
			}
			break;
		}
	}

	public void ShowCodeEntry(){
		codeEntryUi.Show ();
	}

	public void CloseCode(){
		codeEntryUi.Hide();
	}

	public void OnSubmit(string input){
		if (input == "wella") {
			Debug.Log ("Unlocked Item");
			CloseCode();
			InventoryLogic.Instance.AddItem("specialBCH", 1);
			DataManager.Instance.GameData.Inventory.isSecretItemUnlocked.Add("specialBCH");
			AccessoryUIManager.Instance.Equip("specialBCH");
			codeRedeemedUi.Show();
		}
	}

	public void CloseRedeemUi(){
		codeRedeemedUi.Hide ();
	}

	/// <summary>
	/// This buy button is used only during tutorial. It doesn't check for wella coins
	/// but user will only be limited to one item in GameTutorialDecoration.cs
	/// </summary>
	/// <param name="button">Button.</param>
	public void OnBuyButtonTutorial(GameObject button){
		Transform buttonParent = button.transform.parent.parent;
		string itemID = buttonParent.name;
		Item itemData = ItemLogic.Instance.GetItem(itemID);

		switch(itemData.CurrencyType){
		case CurrencyTypes.WellaCoin:
			if(itemData.Type == ItemType.Decorations){
//				DecorationItem decoItem = (DecorationItem)itemData;

				//Use for tutorial to notify tutorial manager when deco item has been bought
				bool isDecorationTutorialDone = DataManager.Instance.GameData.
					Tutorial.IsTutorialFinished(TutorialManagerBedroom.TUT_DECOS);
				
				if(!isDecorationTutorialDone && OnDecorationItemBought != null)
					OnDecorationItemBought(this, EventArgs.Empty);
					
				InventoryLogic.Instance.AddItem(itemID, 1);

				OnBuyAnimation(itemData, buttonParent.gameObject.FindInChildren("ItemTexture"));
				
				//Analytics
				Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_BOUGHT, itemData.Type, itemData.ID);
				
				// play a sound since an item was bought
				AudioManager.Instance.PlayClip(soundBuy);
			}
			break;
		}
	}
	
	/// <summary>
	/// Creates the sub category items. Create tabs for sub category if sub category exists.
	/// Then call other methods to create the items
	/// </summary>
	/// <param name="page">Page.</param>
	public void CreateSubCategoryItems(GameObject page){
		CreateSubCategoryItemsWithString(page.name);
	}

	/// <summary>
	/// Creates the sub category items with string.
	/// </summary>
	/// <param name="page">Page.</param>
	private void CreateSubCategoryItemsWithString(string page, StoreShortcutType shortcutType = StoreShortcutType.None){
		if(page != "Items" && page != "Food" && page != "Decorations"){
			Debug.LogError("Illegal store sub category: " + page);
			return;
		}
		
		currentPage = page;

		//create the tabs for those sub category
		if(currentPage == "Food"){
			InventoryUIManager.Instance.ShowPanel();
			DecoInventoryUIManager.Instance.HideDecoInventory();

			foreach(Transform tabParent in tabArea.transform){
				HideUnuseTab(tabParent.FindChild("Tab"));
			}

			CreateSubCategoryItemsTab("foodDefaultTab", Color.white, shortcutType);
		}
		else if(currentPage == "Items"){
			InventoryUIManager.Instance.ShowPanel();
			DecoInventoryUIManager.Instance.HideDecoInventory();

			foreach(Transform tabParent in tabArea.transform){
				HideUnuseTab(tabParent.FindChild("Tab"));
			}

			CreateSubCategoryItemsTab("itemsDefaultTab", Color.white, shortcutType);
		}
		else if(currentPage == "Decorations"){
			InventoryUIManager.Instance.HidePanel();
			DecoInventoryUIManager.Instance.ShowDecoInventory();

			//Get a list of decoration types from Enum
			string[] decorationEnums = Enum.GetNames(typeof(DecorationTypes));
			int counter = 0;

			// Set the default category
			string defaultTabName = "Carpet";	
			if(Application.loadedLevelName == SceneUtils.YARD){
				defaultTabName = "SmallPlant";
			}

			List<string> unlockedDecoList = PartitionManager.Instance.GetAllowedDecoTypeFromLatestPartition();

			//Rename the tab to reflect the sub category name
			foreach(Transform tabParent in tabArea.transform){		// TODO-s CHANGE THIS TO FIT TABS
				if(counter < decorationEnums.Length){
					tabParent.name = decorationEnums[counter];
					
					UISprite imageSprite = tabParent.FindChild("Tab/TabImage").gameObject.GetComponent<UISprite>();
					imageSprite.spriteName = "iconDeco" + tabParent.name + "2";

					// Call resizer
					SpriteResizer resizer = imageSprite.GetComponent<SpriteResizer>();
					resizer.enabled = true;	// Resize automatically

					// If the gate xml has the deco type allowed, enable button
					if(unlockedDecoList.Contains(tabParent.name)){
						ShowActiveTab(tabParent.FindChild("Tab"));
					}
					// Else disable button
					else{
						ShowInactiveTab(tabParent.FindChild("Tab"));
					}
				}
				else{
					tabParent.name = "";

					HideUnuseTab(tabParent.FindChild("Tab"));
				}
				counter++;
			}

			//After tabs have been set up create items for the first/default tab
			CreateSubCategoryItemsTab(defaultTabName, Color.white, shortcutType);
		}
		ShowStoreSubPanel();
	}

	
	//----------------------------------------------------
	// CreateSubCategoryItemsTab()
	// Create items for sub category.
	// public method to be called by button
	//----------------------------------------------------
	public void CreateSubCategoryItemsTab(GameObject tab){
		UISprite backgroundSprite = tab.transform.FindChild("TabBackground").gameObject.GetComponent<UISprite>();
		Color tabColor = backgroundSprite.color;
		CreateSubCategoryItemsTab(tab.GetParent().name, tabColor);
	}
	
	//----------------------------------------------------
	// CreateSubCategoryItemsTab()
	// Create items for sub category 
	//----------------------------------------------------
	public void CreateSubCategoryItemsTab(string tabName, Color tabColor, StoreShortcutType shortcutType = StoreShortcutType.None){
//		Debug.Log("OPENING STORE MODE " + shortcutType.ToString());
		
		//Destroy existing items first
		DestroyGrid();
		
		//Reset clip range so scrolling will start from beginning again
		ResetUIPanelClipRange();

		AudioManager.Instance.PlayClip(soundChangeTab);
		
		//set current tab
		prevTab = GameObject.Find(currentTab);
		currentTab = tabName;
		
		//set panel background color
		storeSubPanelBg.GetComponent<UISprite>().color = tabColor;
		
		//base on the tab name and the page name, create proper set of item in the store
		if(currentPage == "Food"){
			//No sub category so retrieve a list of all food
			List<Item> foodList = ItemLogic.Instance.FoodList;
			
			foreach(Item itemData in foodList){
				if(!itemData.IsSecretItem){
					StoreItemEntryUIController.CreateEntry(grid, itemStorePrefabStats, itemData);
				}
			}
		}
		else if(currentPage == "Items"){
			//No sub category so retrieve a list of all item
			List<Item> usableList = ItemLogic.Instance.UsableList;
			
			foreach(Item itemData in usableList){
				if(!itemData.IsSecretItem){
					// Need emergency inhaler shortcut, only show emergency inhaler
					if(shortcutType == StoreShortcutType.SickNotification || shortcutType == StoreShortcutType.NeedEmergencyInhalerPetSpeech){
						if(itemData.ID == "Usable0"){
							StoreItemEntryUIController.CreateEntry(grid, itemStorePrefabStats, itemData);
							break;
						}
						continue;
					}
					// Default case, show everything
					else{	
						StoreItemEntryUIController.CreateEntry(grid, itemStorePrefabStats, itemData);
					}
				}
			}
		}
		else if(currentPage == "Decorations"){
			// our currently selected tab
			GameObject selectedTab = GameObject.Find(tabName);
			//Retrieve decoration items base on the tab name (sub category)
			Dictionary<DecorationTypes, List<DecorationItem>> decoDict = ItemLogic.Instance.DecorationSubCatList;	
			DecorationTypes decoType = (DecorationTypes)Enum.Parse(typeof(DecorationTypes), tabName);
			if(decoDict.ContainsKey(decoType)){
				List<DecorationItem> decoList = decoDict[decoType];

				// If we havent changed tabs, dont set reset anything
				if(prevTab != null){
					UIImageButton imageButtonPrev = prevTab.transform.Find("Tab").GetComponent<UIImageButton>();
					imageButtonPrev.normalSprite = "buttonCategory";
					imageButtonPrev.hoverSprite = "buttonCategory";
					imageButtonPrev.pressedSprite = "buttonCategory";
					imageButtonPrev.enabled = false;
					imageButtonPrev.enabled = true;
				}
				// Change the sprite of the current tab to active
				UIImageButton imageButtonSeletected = selectedTab.transform.Find("Tab").GetComponent<UIImageButton>();
				imageButtonSeletected.normalSprite = "buttonCategoryActive";
				imageButtonSeletected.hoverSprite = "buttonCategoryActive";
				imageButtonSeletected.pressedSprite = "buttonCategoryActive";
				imageButtonSeletected.enabled = false;
				imageButtonSeletected.enabled = true;

				foreach(DecorationItem decoItemData in decoList){
					if(!decoItemData.IsSecretItem){
						StoreItemEntryUIController.CreateEntry(grid, itemStorePrefab, (Item)decoItemData);
					}
				}
			}
		}
		grid.GetComponent<UIGrid>().Reposition();
	}

	//------------------------------------------
	/// <summary>
	/// Shows the store sub panel. By not calling the Open() of SingletonUI
	/// we bypass the clickmanager lock so it will remain lock
	/// </summary>
	private void ShowStoreSubPanel(){
		storeSubPanel.GetComponent<TweenToggleDemux>().Show();
		storeBasePanel.GetComponent<TweenToggleDemux>().Hide();
		HUDUIManager.Instance.ShowLabels();
	}

	//------------------------------------------
	// HideStoreSubPanel()
	// Return to the StoreBasePanel
	//------------------------------------------
	public void HideStoreSubPanel(){
		DestroyGrid();
		storeSubPanel.GetComponent<TweenToggleDemux>().Hide();
		if(isShortcutMode){
			switch(shortcutType){
			// Exit back to decoration UI
			case StoreShortcutType.DecorationUIStoreButton:
			case StoreShortcutType.DecorationUIStoreButtonTutorial:
				storeBgPanel.GetComponent<TweenToggleDemux>().Hide();
				DecoInventoryUIManager.Instance.ShowDecoInventory();
				InventoryUIManager.Instance.HidePanel();
				HUDUIManager.Instance.HidePanel();
				break;

			case StoreShortcutType.MinipetUIStoreButton:
				storeBgPanel.GetComponent<TweenToggleDemux>().Hide();
				NavigationUIManager.Instance.HidePanel();
				break;

			// Exit back to fireblowing room UI
//			case StoreShortcutType.FlameCrystalNeededNotification:
//				storeBgPanel.GetComponent<TweenToggleDemux>().Hide();
//				InventoryUIManager.Instance.ShowPanel();
//				RoomArrowsUIManager.Instance.ShowPanel();
//				DecoInventoryUIManager.Instance.HideDecoInventory();
//				break;

			// Exit back to default UI
			case StoreShortcutType.NeedFoodTutorial:
			case StoreShortcutType.NeedFoodPetSpeech:
			case StoreShortcutType.NeedEmergencyInhalerPetSpeech:
			case StoreShortcutType.SickNotification:
				_CloseUI();
				break;

			// Default cases which should never happen5
			case StoreShortcutType.None:
			default:
				Debug.LogWarning("Invalid shortcut type detected");
				_CloseUI();
				break;
			}


//			if(ClickManager.Instance.CheckStack(UIModeTypes.EditDecos)){	// If we are shortcuting from edit deco
//				storeBgPanel.GetComponent<TweenToggleDemux>().Hide();		// Only hide certain things
//				DecoInventoryUIManager.Instance.ShowDecoInventory();
//				InventoryUIManager.Instance.HidePanel();
//				HUDUIManager.Instance.HidePanel();
//			}
//			else if(ClickManager.Instance.CheckStack(UIModeTypes.GatingSystem)){	// If we are shortcuting from flame crystal notif
//				storeBgPanel.GetComponent<TweenToggleDemux>().Hide();		// Only hide certain things
//				InventoryUIManager.Instance.ShowPanel();
//				RoomArrowsUIManager.Instance.ShowPanel();
//				DecoInventoryUIManager.Instance.HideDecoInventory();
//			}
//			else if(ClickManager.Instance.CheckStack(UIModeTypes.MiniPet)){
//				storeBgPanel.GetComponent<TweenToggleDemux>().Hide();
//				InventoryUIManager.Instance.ShowPanel();
//				DecoInventoryUIManager.Instance.HideDecoInventory();
//			}
//			else{
//				_CloseUI();	
//			}

			if(OnShortcutModeEnd != null){
				// This will unlock click manager when shortcutted
				OnShortcutModeEnd(this, EventArgs.Empty);
			}

			isShortcutMode = false;
			shortcutType = StoreShortcutType.None;
		}
		else{
			storeBasePanel.GetComponent<TweenToggleDemux>().Show();
			DecoInventoryUIManager.Instance.HideDecoInventory();
			InventoryUIManager.Instance.HidePanel();
		}
	}

	//-----------------------------------------
	// HideUnuseTab()
	// Destroys the entries in the grid
	//------------------------------------------
	private void DestroyGrid(){
		foreach(Transform child in grid.transform){
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}
	}

	//-----------------------------------------
	// HideUnuseTab()
	// If the tab is not used. turn the UISprite 
	// script and the collider off
	//------------------------------------------
	private void HideUnuseTab(Transform tab){
		tab.FindChild("TabBackground").gameObject.GetComponent<UISprite>().enabled = false;
		tab.FindChild("TabImage").gameObject.GetComponent<UISprite>().enabled = false;
		tab.collider.enabled = false;
	}

	/// <summary>
	/// If tab is valid and unlocked
	/// </summary>
	/// <param name="tab">Tab transform</param>
	private void ShowActiveTab(Transform tab){
		tab.GetComponent<UIImageButton>().isEnabled = true;
		tab.FindChild("TabBackground").gameObject.GetComponent<UISprite>().enabled = true;
		tab.FindChild("TabImage").gameObject.GetComponent<UISprite>().enabled = true;
		tab.collider.enabled = true;
	}

	/// <summary>
	/// If tab is valid but locked
	/// </summary>
	/// <param name="tab">Tab transform</param>
	private void ShowInactiveTab(Transform tab){
		tab.GetComponent<UIImageButton>().isEnabled = false;
		tab.FindChild("TabBackground").gameObject.GetComponent<UISprite>().enabled = true;
		tab.FindChild("TabImage").gameObject.GetComponent<UISprite>().enabled = true;
		tab.collider.enabled = false;
	}

	//------------------------------------------
	// ResetUIPanelClipRange()
	// reset the clip range for the item area so that scrolling starts from the beginning
	//------------------------------------------
	private void ResetUIPanelClipRange(){
		Vector4 clipRange = itemArea.GetComponent<UIPanel>().clipRange;
		
		// Stop the springing action when switching
		SpringPanel spring = itemArea.GetComponent<SpringPanel>();
		if(spring != null){
			spring.enabled = false;	
		}
		
		// Reset the localposition and clipping position
		itemArea.transform.localPosition = new Vector3(52f, itemArea.transform.localPosition.y, itemArea.transform.localPosition.z);
		itemArea.GetComponent<UIPanel>().clipRange = new Vector4(-52f, clipRange.y, clipRange.z, clipRange.w);
	}
}

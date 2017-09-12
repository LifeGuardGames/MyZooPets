using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

// Shortcut mode types into store sub panel
public enum StoreShortcutType{
	None,
	DecorationUIStoreButton,
	MinipetUIStoreButton,
	NeedFoodPetSpeech,
	NeedFoodTutorial,
	NeedEmergencyInhalerPetSpeech,
	SickNotification,
}

public class StoreUIManager : SingletonUI<StoreUIManager>{
	public static EventHandler<EventArgs> OnShortcutModeEnd;
	public static EventHandler<EventArgs> OnDecorationItemBought;
	public GridLayoutGroup grid;
	public ScrollRect scrollRect;
	public GameObject itemStorePrefab;		//basic ui setup for an individual item
	public GameObject itemStorePrefabStats;	// a stats item entry
	public GameObject boughtItemTweenPrefab; 	// Used for tween animation
	public GameObject storeBasePanel; 		//Where you choose item category
	public GameObject storeSubPanel; 		//Where you choose item sub category
	public GameObject itemArea; 			//Where the items will be display
	public GameObject tabArea; 				//Where all the tabs for sub category are
	public GameObject storeBgPanel;			// the bg of the store (sub panel and base panel)
	private float gridStartPositionX;

	private bool isShortcutMode; 			//True: open store directly to specific item category
											//False: open the store base panel first	
	private StoreShortcutType shortcutType = StoreShortcutType.None;	// Store where shortcut was from

	private bool changePage;
	private string currentPage;	//The current category. i.e food, usable, decorations

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Store;
	}
	
	protected override void Start(){
		base.Start();
		// Reposition all the things nicely to stretch to the end of the screen
		
		/*// Position the UIPanel clipping range
		UIPanel itemAreaPanel = itemArea.GetComponent<UIPanel>();
		Vector4 oldRange = itemAreaPanel.clipRange;
		
		// The 52 comes from some wierd scaling issue.. not sure what it is but compensate now
		itemAreaPanel.transform.localPosition = new Vector3(52f, itemAreaPanel.transform.localPosition.y, 0f);
		itemAreaPanel.clipRange = new Vector4(52f, oldRange.y, (float)(CameraManager.Instance.NativeWidth), oldRange.w);
		
		// Position the grid origin to the left of the screen
		Vector3 gridPosition = grid.transform.localPosition;
		grid.transform.localPosition = new Vector3(
			(-1f * (CameraManager.Instance.NativeWidth / 2)) - itemArea.transform.localPosition.x,
			gridPosition.y, gridPosition.z);*/

		gridStartPositionX = grid.GetComponent<RectTransform>().position.x;
	}

	/// <summary>
	/// Enables the tab area. Use in tutorial
	/// </summary>
	public void EnableTabArea(){
		foreach(Transform tabTransform in tabArea.transform){
			tabTransform.Find("Tab").gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Disables the tab area. Use in tutorial
	/// </summary>
	public void DisableTabArea(){
		foreach(Transform tabTransform in tabArea.transform){
			tabTransform.Find("Tab").gameObject.SetActive(false);
		}
	}

	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		RoomArrowsUIManager.Instance.HidePanel();
		HUDUIManager.Instance.ToggleLabels(true);
		storeBasePanel.GetComponent<TweenToggleDemux>().Show();
		storeBgPanel.GetComponent<TweenToggleDemux>().Show();
	}

	public void OnCloseButton() {
		CloseUI();
	}

	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();
		RoomArrowsUIManager.Instance.ShowPanel();
		InventoryUIManager.Instance.ShowPanel(false);
		HUDUIManager.Instance.ToggleLabels(false);
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
	
	// Called when item bought, creates a sprite for the item and move it to correct inventory
	public void OnBuyAnimation(StoreItemController storeItemScript){
		Vector3 origin = storeItemScript.GetSpritePosition();
		Vector3 endPosition = InventoryUIManager.Instance.itemFlyToTransform.position; // TODO change this

		GameObject animationSprite = GameObjectUtils.AddChild(storeSubPanel, boughtItemTweenPrefab);
		animationSprite.GetComponentInChildren<Image>().sprite = SpriteCacheManager.GetItemSprite(storeItemScript.ItemData.ID);
		animationSprite.transform.position = origin;
		animationSprite.name = storeItemScript.ItemData.ID;

		LeanTween.move(animationSprite, endPosition, 0.666f)
			.setEase(LeanTweenType.easeOutQuad)
			.setOnComplete(OnBuyAnimationDone)
			.setOnCompleteParam(animationSprite);
	}

	// Show the updated inventory bar with new item and destroy the tweening sprite
	private void OnBuyAnimationDone(object obj) {
		string itemID = ((GameObject)obj).name;
		ItemType type = DataLoaderItems.GetItem(itemID).Type;
        if(type == ItemType.Foods || type == ItemType.Usables) {
			InventoryUIManager.Instance.PulseInventory();
			InventoryUIManager.Instance.RefreshPage();
		}
		else if(type == ItemType.Decorations) {
			DecoInventoryUIManager.Instance.PulseInventory();
			DecoInventoryUIManager.Instance.RefreshPage();
		}
		Destroy((GameObject)obj);
	}

	/// <summary>
	/// Called from buy StoreItemController OnBuyButton
	/// </summary>
	/// <param name="button"></param>
	public void BuyButtonLogic(StoreItemController storeItemScript){
		Item itemData = storeItemScript.ItemData;
		switch(itemData.CurrencyType){
		case CurrencyTypes.WellaCoin:
			if(DataManager.Instance.GameData.Stats.Stars >= itemData.Cost){
				InventoryManager.Instance.AddItemToInventory(itemData.ID);
				StatsManager.Instance.ChangeStats(coinsDelta: itemData.Cost * -1);
				OnBuyAnimation(storeItemScript);
				
				// Check for any special cases we need to account for (ie. wallpaper buying)
				storeItemScript.BuyButtonStateCheck();
				
				// Analytics
				Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_BOUGHT, itemData.Type, itemData.ID);
				
				// Play a sound since an item was bought
				AudioManager.Instance.PlayClip("shopBuy");
            }
			else{
				HUDUIManager.Instance.PlayNeedCoinAnimation();
				AudioManager.Instance.PlayClip("buttonDontClick");
			}
			break;
		}
	}
	
	/// <summary>
	/// Creates the sub category items. Create tabs for sub category if sub category exists.
	/// Then call other methods to create the items
	/// </summary>
	/// <param name="page">Page.</param>
	public void CreateSubCategoryItems(string page){
		CreateSubCategoryItemsWithString(page);
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

		// Reset the grid and scrolling
		scrollRect.StopMovement();
		Vector2 auxPosition = grid.GetComponent<RectTransform>().anchoredPosition;
		grid.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, auxPosition.y);

		//create the tabs for those sub category
		if(currentPage == "Food"){
			InventoryUIManager.Instance.ShowPanel(true);
			DecoInventoryUIManager.Instance.HidePanel();

			foreach(Transform tab in tabArea.transform){
				ToggleTab(tab, false);
			}

			CreateSubCategoryItemsTab("foodDefaultTab", shortcutType);
		}
		else if(currentPage == "Items"){
			InventoryUIManager.Instance.ShowPanel(true);
			DecoInventoryUIManager.Instance.HidePanel();

			foreach(Transform tab in tabArea.transform){
				ToggleTab(tab, false);
			}

			CreateSubCategoryItemsTab("itemsDefaultTab", shortcutType);
		}
		else if(currentPage == "Decorations"){
			InventoryUIManager.Instance.HidePanel();
			DecoInventoryUIManager.Instance.ShowPanel();

			//Get a list of decoration types from Enum
			string[] decorationEnums = Enum.GetNames(typeof(DecorationTypes));
			int counter = 0;

			// Set the default category
			string defaultTabName = "Carpet";	
			if(SceneUtils.CurrentScene == SceneUtils.YARD){
				defaultTabName = "SmallPlant";
			}

			List<string> unlockedDecoList = PartitionManager.Instance.GetAllowedDecoTypeFromLatestPartition();

			//Rename the tab to reflect the sub category name
			foreach(Transform tab in tabArea.transform){		// TODO-s CHANGE THIS TO FIT TABS
				if(counter < decorationEnums.Length){
					tab.name = decorationEnums[counter];
					if(tab.name == defaultTabName) {
						tab.GetComponent<Image>().sprite = SpriteCacheManager.GetSprite("buttonCategoryActive");
					}
					Image imageSprite = tab.Find("TabImage").gameObject.GetComponent<Image>();
					imageSprite.sprite = SpriteCacheManager.GetDecoIconSprite((DecorationTypes)Enum.Parse(typeof(DecorationTypes), tab.name));

					//Debug.Log(tabParent.name);
					// If the gate xml has the deco type allowed, enable button
					if(unlockedDecoList.Contains(tab.name)){
						ToggleTab(tab, true);
					}
					// Else disable button
					else{
						ToggleTab(tab, false);
					}
				}
				else{
					tab.name = "";
					ToggleTab(tab, false);
				}
				counter++;
			}

			//After tabs have been set up create items for the first/default tab
			CreateSubCategoryItemsTab(defaultTabName, shortcutType);
		}
		ShowStoreSubPanel();
	}

	// Create items for sub category, public method to be called by button
	public void CreateSubCategoryItemsTab(GameObject tab){
		CreateSubCategoryItemsTab(tab.name);
	}
	
	//----------------------------------------------------
	// CreateSubCategoryItemsTab()
	// Create items for sub category 
	//----------------------------------------------------
	public void CreateSubCategoryItemsTab(string tabName, StoreShortcutType shortcutType = StoreShortcutType.None){
		//		Debug.Log("OPENING STORE MODE " + shortcutType.ToString());

		//Destroy existing items first
		DestroyGrid();
		
		//Reset clip range so scrolling will start from beginning again
		ResetUIPanelClipRange();

		AudioManager.Instance.PlayClip("shopChangeTab");

		int itemCount = 0;

		//base on the tab name and the page name, create proper set of item in the store
		if(currentPage == "Food"){
			//No sub category so retrieve a list of all food
			List<Item> foodList = ItemManager.Instance.FoodList;
			foreach(Item itemData in foodList){
				if(!itemData.IsSecretItem){
					CreateStoreItem(grid.gameObject, itemStorePrefabStats, itemData);
					itemCount++;
				}
			}
		}

		else if(currentPage == "Items"){
			//No sub category so retrieve a list of all item
			List<Item> usableList = ItemManager.Instance.UsableList;
			
			foreach(Item itemData in usableList){
				if(!itemData.IsSecretItem){
					// Need emergency inhaler shortcut, only show emergency inhaler
					if(shortcutType == StoreShortcutType.SickNotification || shortcutType == StoreShortcutType.NeedEmergencyInhalerPetSpeech){
						if(itemData.ID == "Usable0"){
							itemCount++;
							CreateStoreItem(grid.gameObject, itemStorePrefabStats, itemData);
							break;
						}
						continue;
					}
					// Default case, show everything
					else{	
						CreateStoreItem(grid.gameObject, itemStorePrefabStats, itemData);
						itemCount++;
					}
				}
			}
		}
		else if(currentPage == "Decorations"){
			//Retrieve decoration items base on the tab name (sub category)
			Dictionary<DecorationTypes, List<DecorationItem>> decoDict = ItemManager.Instance.DecorationSubCatList;	
			DecorationTypes decoType = (DecorationTypes)Enum.Parse(typeof(DecorationTypes), tabName);
			if(decoDict.ContainsKey(decoType)){
				List<DecorationItem> decoList = decoDict[decoType];
				foreach(DecorationItem decoItemData in decoList){
					if(!decoItemData.IsSecretItem){
						CreateStoreItem(grid.gameObject, itemStorePrefab, (Item)decoItemData);
						itemCount++;
					}
				}
			}
		}

		// Adjust the grid width based on the width of the cell and spacing
		itemCount = (itemCount % 2 == 1) ? itemCount + 1 : itemCount;	// Dividing by 2 later, so make sure even
		float gridWidth = itemCount * 0.5f * (grid.cellSize.x + grid.spacing.x);
		grid.GetComponent<RectTransform>().sizeDelta = new Vector2(gridWidth, grid.GetComponent<RectTransform>().sizeDelta.y);
	}

	//------------------------------------------
	/// <summary>
	/// Shows the store sub panel. By not calling the Open() of SingletonUI
	/// we bypass the clickmanager lock so it will remain lock
	/// </summary>
	private void ShowStoreSubPanel(){
		storeSubPanel.GetComponent<TweenToggleDemux>().Show();
		storeBasePanel.GetComponent<TweenToggleDemux>().Hide();
		HUDUIManager.Instance.ToggleLabels(true);
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
			case StoreShortcutType.MinipetUIStoreButton:
				storeBgPanel.GetComponent<TweenToggleDemux>().Hide();
				NavigationUIManager.Instance.HidePanel();
				break;

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
			DecoInventoryUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
		}
	}

	//-----------------------------------------
	// HideUnuseTab()
	// Destroys the entries in the grid
	//------------------------------------------
	private void DestroyGrid(){
		foreach(Transform child in grid.transform){
			//child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}
	}

	private void ToggleTab(Transform tab, bool isOn) {
		tab.gameObject.SetActive(isOn);
    }

	//------------------------------------------
	// ResetUIPanelClipRange()
	// reset the clip range for the item area so that scrolling starts from the beginning
	//------------------------------------------
	private void ResetUIPanelClipRange(){
		grid.transform.position = new Vector3 (gridStartPositionX, grid.transform.position.y, grid.transform.position.z);
	}

	private void CreateStoreItem(GameObject goGrid, GameObject goPrefab, Item item) {
		GameObject itemUIObject = GameObjectUtils.AddChildGUI(goGrid, goPrefab);
		itemUIObject.GetComponent<StoreItemController>().Init(item);
	}
}

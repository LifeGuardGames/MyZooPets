using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreUIManager : SingletonUI<StoreUIManager>{
	public static EventHandler<EventArgs> OnShortcutModeEnd;
	public GameObject grid;
	public GameObject itemStorePrefab; //basic ui setup for an individual item
	public GameObject itemStorePrefabStats;	// a stats item entry
	public GameObject itemSpritePrefab; // item sprite for inventory
	public GameObject storeBasePanel; //Where you choose item category
	public GameObject storeSubPanel; //Where you choose item sub category
	public GameObject storeSubPanelBg;
	public GameObject itemArea; //Where the items will be display
	public GameObject tabArea; //Where all the tabs for sub category are
	public GameObject storeBgPanel;	// the bg of the store (sub panel and base panel)
	private GameObject goExitButton;	// exit button on sub panel
	
	// store related sounds
	public string soundChangeTab;
	public string soundBuy;
	private bool isShortcutMode; //True: open store directly to specific item category
	//False: open the store base panel first	
	private bool changePage;
	private string currentPage; //The current category. i.e food, usable, decorations
	private string currentTab; //The current sub category. only decorations have sub cat right now
	
//	public List<Color> colors; //colors for the tab;

	void Awake(){
		eModeType = UIModeTypes.Store;

		goExitButton = storeSubPanel.FindInChildren("ExitButton");
		if(goExitButton == null)
			Debug.LogError("Exit button is null...please set");
	}
	
	void Start(){
		// Reposition all the things nicely to stretch to the end of the screen
		
		// Position the UIPanel clipping range
		UIPanel itemAreaPanel = itemArea.GetComponent<UIPanel>();
		Vector4 oldRange = itemAreaPanel.clipRange;
		
		// The 52 comes from some wierd scaling issue.. not sure what it is but compensate now
		itemAreaPanel.transform.localPosition = new Vector3(52f, itemAreaPanel.transform.localPosition.y, 0f);
		itemAreaPanel.clipRange = new Vector4(52f, oldRange.y, (float)(CameraManager.Instance.GetNativeWidth()), oldRange.w);
		
		// Position the grid origin to the left of the screen
		Vector3 gridPosition = grid.transform.localPosition;
		grid.transform.localPosition = new Vector3(
			(-1f * (CameraManager.Instance.GetNativeWidth() / 2)) - itemArea.transform.localPosition.x,
			gridPosition.y, gridPosition.z);
	}
	
	//---------------------------------------------------
	// _OpenUI()
	//---------------------------------------------------	
	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		storeBasePanel.GetComponent<TweenToggleDemux>().Show();
		storeBgPanel.GetComponent<TweenToggleDemux>().Show();
	}
	
	//---------------------------------------------------
	// _CloseUI()
	//---------------------------------------------------	
	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();		
		storeBasePanel.GetComponent<TweenToggleDemux>().Hide();
		storeBgPanel.GetComponent<TweenToggleDemux>().Hide();
		storeSubPanel.GetComponent<TweenToggleDemux>().Hide();
	}

	//----------------Hacky code to fix store shortcut problems. need a better solution
	// The reason the click manager is locked from here is because these shorcuts circumvent the normal open/closing of this UI.
	public void OpenToSubCategoryFoodWithLockAndCallBack(){
		NavigationUIManager.Instance.HidePanel();
		ClickManager.Instance.Lock(UIModeTypes.Store, GetClickLockExceptions());
		OnShortcutModeEnd += ShortcutModeEnded;	

		OpenToSubCategory("Food", true);
	}

	public void OpenToSubCategoryItemsWithLockAndCallBack(){
		//send analytics
		Analytics.Instance.StoreItemShortCutClicked();

		NavigationUIManager.Instance.HidePanel();
		ClickManager.Instance.Lock(UIModeTypes.Store, GetClickLockExceptions());
		OnShortcutModeEnd += ShortcutModeEnded;	

		OpenToSubCategory("Items", true);
	}

	private void ShortcutModeEnded(object sender, EventArgs args){
		NavigationUIManager.Instance.ShowPanel();
		ClickManager.Instance.ReleaseLock();
		OnShortcutModeEnd -= ShortcutModeEnded;
	}
	//---------------------------------------------------

	//---------------------------------------------------
	// OpenToSubCategory()
	// Special function used to open the store UI 
	// straight up to a certain category.
	//---------------------------------------------------	
	public void OpenToSubCategory(string strCat, bool bShortcut = false){		
		// this is a bit of a hack, but basically there are multiple ways to open the shop.  One way is a shortcut in that it
		// bypasses the normal means of opening a shop, so we need to do some special things in this case
		isShortcutMode = bShortcut;
		if(isShortcutMode){
			// if we are shortcutting, we have to tween the bg in now	
			storeBgPanel.GetComponent<TweenToggleDemux>().Show();
		}	
		
		CreateSubCategoryItemsWithString(strCat); 
	}

	//---------------------------------------------------
	// OnBuyAnimation()
	// This function is called when buying an item
	// It creates a icon for the item and move it to Inventory
	// TODO Scales are a little mess up
	//---------------------------------------------------
	public void OnBuyAnimation(Item itemData, GameObject sprite){
		Vector3 origin = new Vector3(sprite.transform.position.x, sprite.transform.position.y, -0.1f);
		string itemID = sprite.transform.parent.transform.parent.name;
		Vector3 itemPosition = origin;

		//-0.22
		// depending on what type of item the user bought, the animation has the item going to different places
		ItemType eType = itemData.Type;
		switch(eType){
		case ItemType.Decorations:
			itemPosition = EditDecosUIManager.Instance.GetEditButtonPosition();
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

	//---------------------------------------------------
	// OnBuyButton()
	// Called when "Buy" is clicked
	//---------------------------------------------------
	public void OnBuyButton(GameObject button){
		string itemID = button.transform.parent.name;
		Item itemData = ItemLogic.Instance.GetItem(itemID);


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

			InventoryLogic.Instance.AddItem(itemID, 1);
			StatsController.Instance.ChangeStats(deltaStars: itemData.Cost * -1);
			OnBuyAnimation(itemData, button.transform.parent.gameObject.FindInChildren("ItemTexture"));

			//Analytics
			Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_BOUGHT, itemData.Type, itemData.ID);
			
			// play a sound since an item was bought
			AudioManager.Instance.PlayClip(soundBuy);
		}
	}

	//----------------------------------------------------
	// CreateSubCategoryItems()
	// Create tabs for sub category if sub category exists. 
	// Then call other methods to create the items
	//----------------------------------------------------
	public void CreateSubCategoryItems(GameObject page){
		CreateSubCategoryItemsWithString(page.name);
	}
	
	public void CreateSubCategoryItemsWithString(string page){
		if(page != "Items" && page != "Food" && page != "Decorations"){
			Debug.LogError("Illegal sore sub category: " + page);
			return;
		}
		
		// we also need to hide the exit button's active state based on whether or not we are shortcutting
		// NOTE: Just can't hide the damn button, so I am changing the function target...not a great solution...but...sigh...
		string strFunction = isShortcutMode ? "HideStoreSubPanel" : "CloseUI";
		goExitButton.GetComponent<LgButtonMessage>().functionName = strFunction;		
		
		currentPage = page;

		//create the tabs for those sub category
		if(currentPage == "Food"){
			foreach(Transform tabParent in tabArea.transform){
				HideUnuseTab(tabParent.FindChild("Tab"));
			}

			CreateSubCategoryItemsTab("foodDefaultTab", Color.white);
			//CreateSubCategoryItemsTab("foodDefaultTab", colors[3]);	// Disabling custom colors

		}
		else if(currentPage == "Items"){
			foreach(Transform tabParent in tabArea.transform){
				HideUnuseTab(tabParent.FindChild("Tab"));
			}

			CreateSubCategoryItemsTab("itemsDefaultTab", Color.white);
			//CreateSubCategoryItemsTab("itemsDefaultTab", colors[2]);	// Disabling custom colors

		}
		else if(currentPage == "Decorations"){
			//Get a list of decoration types from Enum
			string[] decorationEnums = Enum.GetNames(typeof(DecorationTypes));
			int counter = 0;
			string defaultTabName = "";

			//Rename the tab to reflect the sub category name
			foreach(Transform tabParent in tabArea.transform){		// TODO-s CHANGE THIS TO FIT TABS
				if(counter < decorationEnums.Length){
					tabParent.name = decorationEnums[counter];

					// Disabling custom colors
//					UISprite backgroundSprite = tab.FindChild("TabBackground").gameObject.GetComponent<UISprite>();
//					backgroundSprite.color = colors[counter];
					
					UISprite imageSprite = tabParent.FindChild("Tab/TabImage").gameObject.GetComponent<UISprite>();
					imageSprite.spriteName = "iconDeco" + tabParent.name + "2";

					ShowUseTab(tabParent.FindChild("Tab"));
					if(counter == 0){
						defaultTabName = tabParent.name;
//						defaultColor = colors[counter];
					}
				}
				else{
					tabParent.name = "";

					HideUnuseTab(tabParent.FindChild("Tab"));
				}
				counter++;
			}

			//After tabs have been set up create items for the first/default tab
			CreateSubCategoryItemsTab(defaultTabName, Color.white);
		}

		ShowStoreSubPanel();
	}

	//------------------------------------------
	// ShowStoreSubPanel()
	// By not calling the Open() of SingletonUI we 
	// by pass the clickmanager lock so it will
	// remain lock 
	//------------------------------------------
	private void ShowStoreSubPanel(){
		storeSubPanel.GetComponent<TweenToggleDemux>().Show();
		storeBasePanel.GetComponent<TweenToggleDemux>().Hide();
	}

	//------------------------------------------
	// HideStoreSubPanel()
	// Return to the StoreBasePanel
	//------------------------------------------
	public void HideStoreSubPanel(){
		storeSubPanel.GetComponent<TweenToggleDemux>().Hide();
		
		// this is a little hacky, but our demux system is kind of difficult to get around, so...
		// before doing anything else, check to see if the deco system has a saved node...
		// if it does, it actually means the store was opened from the deco system, so the normal path of showing the base
		// store doesn't apply...otherwise just show the store base panel like normal
		if(isShortcutMode){
			// close the store bg
			storeBgPanel.GetComponent<TweenToggleDemux>().Hide();
		
			if(OnShortcutModeEnd != null)
				OnShortcutModeEnd(this, EventArgs.Empty);

			isShortcutMode = false;
		}
		else
			storeBasePanel.GetComponent<TweenToggleDemux>().Show();
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
	public void CreateSubCategoryItemsTab(string tabName, Color tabColor){
		if(currentTab != tabName){
			//Destroy existing items first
			foreach(Transform child in grid.transform){
				child.gameObject.SetActive(false);
				Destroy(child.gameObject);
			}

			//Reset clip range so scrolling will start from beginning again
			ResetUIPanelClipRange();

			//if the current page is not null, we are switching tabs, so play a sound
			if(currentTab != null)
				AudioManager.Instance.PlayClip(soundChangeTab);

			//set current tab
			currentTab = tabName;

			//set panel background color
			storeSubPanelBg.GetComponent<UISprite>().color = tabColor;

			//base on the tab name and the page name, create proper set of item in the store
			if(currentPage == "Food"){
				//No sub category so retrieve a list of all food
				List<Item> foodList = ItemLogic.Instance.FoodList;

				foreach(Item itemData in foodList){
					if(!itemData.ItemBoxOnly)
						StoreItemEntryUIController.CreateEntry(grid, itemStorePrefabStats, itemData);
				}

			}
			else if(currentPage == "Items"){
				//No sub category so retrieve a list of all item
				List<Item> usableList = ItemLogic.Instance.UsableList;

				foreach(Item itemData in usableList){
					if(!itemData.ItemBoxOnly)
						StoreItemEntryUIController.CreateEntry(grid, itemStorePrefabStats, itemData);
				}

			}
			else if(currentPage == "Decorations"){
				//Retrieve decoration items base on the tab name (sub category)
				Dictionary<DecorationTypes, List<DecorationItem>> decoDict = ItemLogic.Instance.DecorationSubCatList;	
				DecorationTypes decoType = (DecorationTypes)Enum.Parse(typeof(DecorationTypes), tabName);

				if(decoDict.ContainsKey(decoType)){
					List<DecorationItem> decoList = decoDict[decoType];
					foreach(DecorationItem decoItemData in decoList){
						if(!decoItemData.ItemBoxOnly)
							StoreItemEntryUIController.CreateEntry(grid, itemStorePrefab, (Item)decoItemData);
					}
				}
			}

			grid.GetComponent<UIGrid>().Reposition();
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

	//-----------------------------------------
	// UseTab()
	//	If tab is used. Show 
	//------------------------------------------
	private void ShowUseTab(Transform tab){
		tab.FindChild("TabBackground").gameObject.GetComponent<UISprite>().enabled = true;
		tab.FindChild("TabImage").gameObject.GetComponent<UISprite>().enabled = true;
		tab.collider.enabled = true;
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

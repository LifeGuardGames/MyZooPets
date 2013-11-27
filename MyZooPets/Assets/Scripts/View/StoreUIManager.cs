using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreUIManager : SingletonUI<StoreUIManager> {
	public GameObject itemStorePrefab; //basic ui setup for an individual item
	public GameObject itemSpritePrefab; // item sprite for inventory
	public GameObject storeBasePanel; //Where you choose item category
	public GameObject storeSubPanel; //Where you choose item sub category
	public GameObject storeSubPanelBg; 
	public GameObject itemArea; //Where the items will be display
	public GameObject tabArea; //Where all the tabs for sub category are
	
	// store related sounds
	public string strSoundChangeTab;
	public string strSoundBuy;
	
	private bool changePage;
	private string currentPage; //The current category. i.e food, usable, decorations
	private string currentTab; //The current sub category. only decorations have sub cat right now
	private GameObject grid;

	private List<Color> colors; //colors for the tab;

	void Awake(){
		grid = itemArea.transform.Find("Grid").gameObject;

		Color pink = new Color(0.78f, 0f, 0.49f, 0.78f);
		Color purple = new Color(0.49f, 0.03f, 0.66f, 0.78f);
		Color blue = new Color(0.05f, 0.36f, 0.65f, 0.78f);
		Color teel = new Color(0, 0.58f, 0.6f, 0.78f);
		Color green = new Color(0, 0.71f, 0.31f, 0.78f);
		Color orange = new Color(1, 0.6f, 0, 0.78f);
		Color limeGreen = new Color(0.53f, 0.92f, 0, 0.78f);
		Color purpleish = new Color(0.44f, 0.04f, 0.67f, 0.78f);
		Color yellow = new Color(1, 0.91f, 0f, 0.78f);

		colors = new List<Color>();
		colors.Add(pink);
		colors.Add(purple);
		colors.Add(blue);
		colors.Add(teel);
		colors.Add(green);
		colors.Add(orange);
		colors.Add(limeGreen);
		colors.Add(purpleish);
		colors.Add(yellow);
	}

	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		BGController.Instance.Show("black");
		storeBasePanel.GetComponent<TweenToggleDemux>().Show();
		// storePanel.GetComponent<TweenToggle>().Show();
		// EditDecosUIManager.Instance.HideNavButton();
	}

	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();		
		BGController.Instance.Hide();
		storeBasePanel.GetComponent<TweenToggleDemux>().Hide();
		// storePanel.GetComponent<TweenToggle>().Hide();
		// EditDecosUIManager.Instance.ShowNavButton();
	}

	//---------------------------------------------------
	// OnBuyAnimation()
	// This function is called when buying an item
	// It creates a icon for the item and move it to Inventory
	// TODO Scales are a little mess up
	//---------------------------------------------------
	public void OnBuyAnimation(Item itemData, GameObject sprite){
		Vector3 origin = new Vector3(sprite.transform.position.x, sprite.transform.position.y, -0.1f);
		string itemID = sprite.transform.parent.name;
		Vector3 itemPosition = origin;

		//-0.22
		// depending on what type of item the user bought, the animation has the item going to different places
		ItemType eType = itemData.Type;
		switch ( eType ) {
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
		optional.Add ("onComplete", "DestroySprite");
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
	public void DestroySprite( Hashtable hash ){
		// delete the icon we moved
		if ( hash.ContainsKey("Icon") ) {
			GameObject goSprite = (GameObject) hash["Icon"];
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
			InventoryLogic.Instance.AddItem(itemID, 1);
			StatsController.Instance.ChangeStats(0, Vector3.zero, itemData.Cost * -1, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);	// Convert to negative
			OnBuyAnimation(itemData, button.transform.parent.FindChild("ItemTexture").gameObject);
			
			// play a sound since an item was bought
			AudioManager.Instance.PlayClip( strSoundBuy );
		}
	}

	//----------------------------------------------------
	// CreateSubCategoryItems()
	// Create tabs for sub category if sub category exists. 
	// Then call other methods to create the items
	//----------------------------------------------------
	public void CreateSubCategoryItems(GameObject page){
		currentPage = page.name;

		//create the tabs for those sub category
		if(currentPage == "Food"){
			foreach(Transform tab in tabArea.transform){
				HideUnuseTab(tab);
			}

			CreateSubCategoryItemsTab("foodDefaultTab", colors[3]);

		}else if(currentPage == "Items"){
			foreach(Transform tab in tabArea.transform){
				HideUnuseTab(tab);
			}

			CreateSubCategoryItemsTab("itemsDefaultTab", colors[2]);

		}else if(currentPage == "Decorations"){
			//Get a list of decoration types from Enum
			string[] decorationEnums = Enum.GetNames(typeof(DecorationTypes));
			int counter = 0;
			string defaultTabName = "";
			Color defaultColor = new Color(0, 0, 0, 0);

			//Rename the tab to reflect the sub category name
			foreach(Transform tab in tabArea.transform){		// TODO-s CHANGE THIS TO FIT TABS
				if(counter < decorationEnums.Length){
					tab.name = decorationEnums[counter];
					
					UISprite backgroundSprite = tab.FindChild("TabBackground").gameObject.GetComponent<UISprite>();
					backgroundSprite.color = colors[counter];
					
					UISprite imageSprite = tab.FindChild("TabImage").gameObject.GetComponent<UISprite>();
					imageSprite.spriteName = "iconDeco" + tab.name;

					ShowUseTab(tab);
					if(counter == 0){
						defaultTabName = tab.name;
						defaultColor = colors[counter];
					}
				}else{
					tab.name = "";

					HideUnuseTab(tab);
				}
				counter++;
			}

			//After tabs have been set up create items for the first/default tab
			CreateSubCategoryItemsTab(defaultTabName, defaultColor);
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
		CreateSubCategoryItemsTab(tab.name, tabColor);
	}

	//----------------------------------------------------
	// CreateSubCategoryItemsTab()
	// Create items for sub category 
	//----------------------------------------------------
	private void CreateSubCategoryItemsTab(string tabName, Color tabColor){
		if(currentTab != tabName){
			//Destroy existing items first
			foreach(Transform child in grid.transform)
				Destroy(child.gameObject);

			//Reset clip range so scrolling will start from beginning again
			// ResetUIPanelClipRange();

			//if the current page is not null, we are switching tabs, so play a sound
			if(currentTab != null)
				AudioManager.Instance.PlayClip(strSoundChangeTab);

			//set current tab
			currentTab = tabName;

			//set panel background color
			storeSubPanelBg.GetComponent<UISprite>().color = tabColor; 

			//base on the tab name and the page name, create proper set of item in the store
			if(currentPage == "Food"){
				//No sub category so retrieve a list of all food
				List<Item> foodList = ItemLogic.Instance.FoodList;

				foreach(Item itemData in foodList)
					SetUpItemObject(itemData);

			}else if(currentPage == "Items"){
				//No sub category so retrieve a list of all item
				List<Item> usableList = ItemLogic.Instance.UsableList;

				foreach(Item itemData in usableList)
					SetUpItemObject(itemData);

			}else if(currentPage == "Decorations"){
				//Retrieve decoration items base on the tab name (sub category)
				Dictionary<DecorationTypes, List<DecorationItem>> decoDict = ItemLogic.Instance.DecorationSubCatList;	
				DecorationTypes decoType = (DecorationTypes) Enum.Parse(typeof(DecorationTypes), tabName);

				if(decoDict.ContainsKey(decoType)){
					List<DecorationItem> decoList = decoDict[decoType];
					foreach(DecorationItem decoItemData in decoList)
						SetUpItemObject((Item)decoItemData);
				}
			}

			grid.GetComponent<UIGrid>().Reposition();
			Invoke("Reposition",0.00000001f);
		}
	}
	
	//---------------------------------------------------
	// SetUpItemObject()
	// Creates an individual store UI entry for the
	// incoming item data.
	//---------------------------------------------------	
	private void SetUpItemObject(Item itemData) {
		// create and add our UI entry to NGUI
		GameObject itemUIObject = NGUITools.AddChild(grid, itemStorePrefab);
		
		// set the proper values on the entry
		itemUIObject.name = itemData.ID;
		itemUIObject.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemData.Description;
		itemUIObject.transform.FindChild("BuyButton/L_Cost").GetComponent<UILabel>().text = itemData.Cost.ToString();
		itemUIObject.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemData.Name;
		itemUIObject.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemData.TextureName;
		itemUIObject.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
		itemUIObject.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
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
        itemArea.transform.localPosition = new Vector3(itemArea.transform.localPosition.x, -56f, itemArea.transform.localPosition.z);
        itemArea.GetComponent<UIPanel>().clipRange = new Vector4(clipRange.x, 30.5f, clipRange.z, clipRange.w);
	}

	//Delay calling reposition due to async problem Destroying/Repositionoing.
	//TODO Maybe change later when we have moreItems 
	private void Reposition(){
		grid.GetComponent<UIGrid>().Reposition();

	}
}

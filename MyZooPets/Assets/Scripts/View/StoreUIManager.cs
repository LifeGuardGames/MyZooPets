using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreUIManager : SingletonUI<StoreUIManager> {
	public GameObject itemStorePrefab; //basic ui setup for an individual item
	public GameObject itemSpritePrefab; // item sprite for inventory
	public GameObject storePanel;
	public GameObject storeBackground;


	public GameObject storeBasePanel; //Where you choose item category
	public GameObject storeSubPanel; //Where you choose item sub category
	public GameObject itemArea; //Where the items will be display
	public GameObject tabArea; //Where all the tabs for sub category are
	// public GameObject FirstPageTag;
	
	// store related sounds
	public string strSoundChangeTab;
	public string strSoundBuy;
	
	private bool changePage;
	private string currentPage; //The current category. i.e food, usable, decorations
	private string currentTab; //The current sub category. only decorations have sub cat right now

	private GameObject grid;

	void Awake(){
		grid = itemArea.transform.Find("Grid").gameObject;
	}

	void Start (){
		// CreateItems(FirstPageTag);
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

	//This function is called when buying an item
	//It creates a icon for the item and move it to Inventory
	//TODO Scales are a little mess up
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
		GameObject animationSprite = NGUITools.AddChild(storePanel, itemSpritePrefab);
		
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

	//Called when "Buy" is clicked
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

	//--------------------------------------------------
	// CreateCategoryItems() 
	// Draw according to ItemLogic.Instance 
	//--------------------------------------------------
	private void CreateCategoryItems(GameObject page){
		// if(currentPage != page.name){
		// 	// Destory first
		// 	foreach(Transform child in grid.transform){
		// 		Destroy(child.gameObject);
		// 	}
			
		// 	// if the current page is not null, we are switching pages, so play a sound
		// 	if ( currentPage != null )
		// 		AudioManager.Instance.PlayClip( strSoundChangeTab );	
			
		// 	// cache our new page name
		// 	currentPage = page.name;
			
		// 	// based on the page, create the proper set of item in the store
		// 	if(page == null || page.name == "Food")
		// 		CreateCategoryItemsTab( new Color(0.5529f, 0.6863f, 1f, .784f), ItemLogic.Instance.FoodList);
		// 	else if(page.name == "Item")
		// 		CreateCategoryItemsTab( new Color(1f, 0.6196f, 0.6196f, .784f), ItemLogic.Instance.UsableList);
		// 	else if(page.name == "Decoration")
		// 		CreateSubCategoryItems();
		// 		// CreateItemsTab( new Color(0.639f, 1, 0.7529f, .784f), ItemLogic.Instance.DecorationList);
		// 	else
		// 		Debug.Log("Illegal store UI page: " + page.name);
			
		// 	grid.GetComponent<UIGrid>().Reposition();
		// 	Invoke("Reposition",0.00000001f);
		// }
	}
	
	//---------------------------------------------------
	// CreateItemsTab()
	// Populates the store UI with the incoming list
	// of items and bg atlas.
	//---------------------------------------------------	
	private void CreateCategoryItemsTab( Color colorBG, List<Item> listItems ) {
		// reset the clip range for the item area so that the scrolling get rest 
		// Vector4 clipRange = itemArea.GetComponent<UIPanel>().clipRange;
		// itemArea.transform.localPosition = new Vector3(itemArea.transform.localPosition.x, -56f, itemArea.transform.localPosition.z);
		// itemArea.GetComponent<UIPanel>().clipRange = new Vector4(clipRange.x, 30.5f, clipRange.z, clipRange.w);
		
		// // set the proper bg
		// storeBackground.GetComponent<UISprite>().color = colorBG;
		
		// // go through our list of items and create an entry for each one
		// foreach(Item itemData in listItems)
		// 	SetUpItemObject(itemData);
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
			//No tabs so turn them all off
			foreach(Transform tab in tabArea.transform){
				tab.gameObject.SetActive(false);
			}

			CreateSubCategoryItemsTab("");
		}
		else if(currentPage == "Items"){
			foreach(Transform tab in tabArea.transform){
				tab.gameObject.SetActive(false);
			}

			CreateSubCategoryItemsTab("");
		}
		else if(currentPage == "Decorations"){
			string[] decorationEnums = Enum.GetNames(typeof(DecorationTypes));
			int counter = 0;
			string defaultTabName = "";

			//Rename the tab to reflect the sub category name
			foreach(Transform tab in tabArea.transform){
				if(counter < decorationEnums.Length){
					tab.name = decorationEnums[counter];

					if(counter == 0) defaultTabName = tab.name;
				}else{
					tab.name = "";
					// tab.gameObject.SetActive(false);
				}
				counter++;
			}

			//After tabs have been set up create items for the first/default tab
			CreateSubCategoryItemsTab(defaultTabName);
		}

		ShowStoreSubPanel();
	}

	//By not calling the Open() of SingletonUI we by pass the clickmanager lock so it will
	//remain lock 
	private void ShowStoreSubPanel(){
		storeSubPanel.GetComponent<TweenToggleDemux>().Show();
		storeBasePanel.GetComponent<TweenToggleDemux>().Hide();
	}

	//Return to the StoreBasePanel
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
		CreateSubCategoryItemsTab(tab.name);
	}

	//----------------------------------------------------
	// CreateSubCategoryItemsTab()
	// Create items for sub category 
	//----------------------------------------------------
	private void CreateSubCategoryItemsTab(string tabName){
		if(currentTab != tabName){
			//Destroy existing items first
			foreach(Transform child in grid.transform)
				Destroy(child.gameObject);

			//if the current page is not null, we are switching tabs, so play a sound
			if(currentTab != null)
				AudioManager.Instance.PlayClip(strSoundChangeTab);

			currentTab = tabName;

			//base on the tab name and the page name, create proper set of item in the store
			if(currentPage == "Food"){
				//No sub category so retrieve a list of all food
				List<Item> foodList = ItemLogic.Instance.FoodList;

				foreach(Item itemData in foodList)
					SetUpItemObject(itemData);

			}
			else if(currentPage == "Items"){
				//No sub category so retrieve a list of all item
				List<Item> usableList = ItemLogic.Instance.UsableList;

				foreach(Item itemData in usableList)
					SetUpItemObject(itemData);

			}
			else if(currentPage == "Decorations"){
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
		itemUIObject.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemData.GetDesc();
		itemUIObject.transform.FindChild("BuyButton/L_Cost").GetComponent<UILabel>().text = itemData.Cost.ToString();
		itemUIObject.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemData.Name;
		itemUIObject.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemData.TextureName;
		itemUIObject.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
		itemUIObject.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
	}

	//Delay calling reposition due to async problem Destroying/Repositionoing.
	//TODO Maybe change later when we have moreItems 
	private void Reposition(){
		grid.GetComponent<UIGrid>().Reposition();

	}
}

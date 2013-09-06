using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreUIManager : SingletonUI<StoreUIManager> {
	public GameObject ItemPrefab;
	public GameObject ItemSpritePrefab;
	public GameObject storePanel;
	public GameObject storeBackground;

	private bool changePage;
	private int page;
	private UISprite uisprite;
	private GameObject grid;
	private List<GameObject> animSpriteRemoveList = new List<GameObject>(); //sprites used for buy animation

	void Awake(){
		uisprite = GameObject.Find("BuyingAreaBackground").GetComponent<UISprite>();
		grid = GameObject.Find("Grid");
	}

	void Start (){
		CreateItems(null);
	}

	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		BGController.Instance.Show("black");
		storePanel.GetComponent<TweenToggle>().Show();
	}

	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();		
		BGController.Instance.Hide();
		storePanel.GetComponent<TweenToggle>().Hide();
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
		GameObject animationSprite = NGUITools.AddChild(storePanel, ItemSpritePrefab);

		optional.Add("ease", LeanTweenType.easeOutQuad);
		optional.Add ("onComplete", "DestroyAllSprites");
		optional.Add("onCompleteTarget", gameObject);
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(90, 90, 1);
		animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(animationSprite, path, speed, optional);
		animSpriteRemoveList.Add(animationSprite);
	}

	//helper for Buying Animation
	public void DestroyAllSprites(){
		foreach (GameObject go in animSpriteRemoveList){
			Destroy(go);
		}
		animSpriteRemoveList.Clear();
	}

	//Called when "Buy" is clicked
	public void OnBuyButton(GameObject button){
		string itemID = button.transform.parent.name;
		Item itemData = ItemLogic.Instance.GetItem(itemID);
		if(DataManager.Instance.Stats.Stars >= itemData.Cost){
			InventoryLogic.Instance.AddItem(itemID, 1);
			StatsController.Instance.ChangeStats(0, Vector3.zero, itemData.Cost * -1, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);	// Convert to negative
			OnBuyAnimation(itemData, button.transform.parent.FindChild("ItemTexture").gameObject);
		}
	}

	// Drawing function
	// Draw according to ItemLogic.Instance 
	private void CreateItems(GameObject page){

		// Destory first
		foreach(Transform child in grid.transform){
			Destroy(child.gameObject);
		}

		if(page == null || page.name == "Food")
			CreateItemsTab( new Color(0.5529f, 0.6863f, 1f, .784f), ItemLogic.Instance.FoodList);
		else if(page.name == "Usable")
			CreateItemsTab( new Color(1f, 0.6196f, 0.6196f, .784f), ItemLogic.Instance.UsableList);
		else if(page.name == "Decoration")
			CreateItemsTab( new Color(0.639f, 1, 0.7529f, .784f), ItemLogic.Instance.DecorationList);
		else
			Debug.Log("Illegal store UI page: " + page.name);
		
		grid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);
	}
	
	//---------------------------------------------------
	// CreateItemsTab()
	// Populates the store UI with the incoming list
	// of items and bg atlas.
	//---------------------------------------------------	
	private void CreateItemsTab( Color colorBG, List<Item> listItems ) {
		// set the proper bg
		storeBackground.GetComponent<UISprite>().color = colorBG;
		
		// go through our list of items and create an entry for each one
		foreach(Item itemData in listItems)
			SetUpItemObject(itemData);
	}
	
	//---------------------------------------------------
	// SetUpItemObject()
	// Creates an individual store UI entry for the
	// incoming item data.
	//---------------------------------------------------	
	private void SetUpItemObject(Item itemData) {
		// create and add our UI entry to NGUI
		GameObject itemUIObject = NGUITools.AddChild(grid, ItemPrefab);
		
		// set the proper values on the entry
		itemUIObject.name = itemData.ID;
		itemUIObject.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemData.GetDesc();
		itemUIObject.transform.FindChild("BuyButton/L_Cost").GetComponent<UILabel>().text = itemData.Cost.ToString();
		itemUIObject.transform.FindChild("ItemName").GetComponent<UILabel>().text = Localization.Localize(itemData.Name);
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

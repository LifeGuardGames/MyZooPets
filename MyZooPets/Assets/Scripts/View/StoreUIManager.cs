using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreUIManager : SingletonUI<StoreUIManager> {
	public bool isDebug;
	public GameObject ItemPrefab;
	public GameObject ItemSpritePrefab;
	public GameObject storePanel;
	public UIAtlas BackGroundRed;
	public UIAtlas BackGroundGreen;
	public UIAtlas BackGroundOrange;
	public UIAtlas BackGroundPurple;

	private ItemLogic itemlogic;
	private Inventory inventory;
	private bool changePage;
	private int page;
	private UISprite uisprite;
	private GameObject grid;
	private GameObject inventoryGrid;
	private List<GameObject> toDestroy = new List<GameObject>();

	void Awake(){
		if(isDebug)	itemlogic = GameObject.Find("Grid").GetComponent<ItemLogic>();
		else {
			itemlogic = GameObject.Find("GameManager/ItemLogic").GetComponent<ItemLogic>();
			inventory = GameObject.Find("GameManager/InventoryLogic").GetComponent<Inventory>();
		}
		uisprite = GameObject.Find("BuyingAreaBackground").GetComponent<UISprite>();
		grid = GameObject.Find("Grid");
		inventoryGrid = GameObject.Find("UI Grid");
	}

	// Use this for initialization
	void Start () {
		CreateItems(null);
	}

	protected override void _OpenUI(){
		//Hide other UI objects
		NavigationUIManager.Instance.HidePanel();
		
		storePanel.GetComponent<MoveTweenToggle>().Show();
	}

	protected override void _CloseUI(){
		//Show other UI object
		NavigationUIManager.Instance.ShowPanel();		
		
		storePanel.GetComponent<MoveTweenToggle>().Hide();
	}

	//This function is called when buying an item
	//It creates a icon for the item and move it to Inventory
	//TODO Scales are a little mess up
	public void OnBuyAnimation(GameObject sprite){
		Vector3 origin = new Vector3(sprite.transform.position.x, sprite.transform.position.y, -0.1f);
		string id = sprite.transform.parent.name;
		Vector3 itemPosition = origin;

		//Find the existing object
		foreach(Transform item in inventoryGrid.transform){
			if(item.FindChild(id)){
				if(item.FindChild("label").gameObject.GetComponent<UILabel>().text == "1"){
					itemPosition = item.FindChild(id).position + new Vector3(-0.22f,0,0);
				}
				else{
					itemPosition = item.FindChild(id).position;
				}
			}
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
		optional.Add("ease", LeanTweenType.easeOutQuad);
		optional.Add ("onComplete", "DestroyAllSprites");
		GameObject animationSprite = NGUITools.AddChild(storePanel, ItemSpritePrefab);
		optional.Add("onCompleteTarget", gameObject);
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(130, 130, 1);
		animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(animationSprite, path, speed, optional);
		toDestroy.Add(animationSprite);
	}

	//helper for Buying Animation
	public void DestroyAllSprites(){
		foreach (GameObject go in toDestroy){
			Destroy(go);
		}
		toDestroy.Clear();
	}

	//Called when "Buy" is clicked
	public void OnBuyButton(GameObject button){
		int cost = int.Parse(button.transform.parent.FindChild("ItemCost").GetComponent<UILabel>().text);
		int itemId = int.Parse(button.transform.parent.name);
		if(DataManager.Instance.Stats.Stars >= cost){
			inventory.AddItem(itemId, 1);
			StatsController.Instance.ChangeStats(0, Vector3.zero, cost * -1, Vector3.zero, 0, Vector3.zero, 0, Vector3.zero);	// Convert to negative
			OnBuyAnimation(button.transform.parent.FindChild("ItemTexture").gameObject);
		}
	}

	//Drawing function.
	//draw according to itemlogic
	private void CreateItems(GameObject page){

		//Destory first
		foreach(Transform child in grid.transform){
			Destroy(child.gameObject);
		}

		if(page == null || page.name == "Food"){
			uisprite.atlas = BackGroundRed;
			for(int i = 0;i<itemlogic.FoodList.Count;i++){
				GameObject item = NGUITools.AddChild(grid, ItemPrefab);
				item.name = itemlogic.FoodList[i].ToString();
				//item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelBlue";
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.FoodList[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.FoodList[i]].cost.ToString();
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.FoodList[i]].name;
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.Items[itemlogic.FoodList[i]].textureName;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		else if(page.name == "Item"){
			uisprite.atlas = BackGroundGreen;
			for(int i = 0;i<itemlogic.ItemList.Count;i++){
				GameObject item = NGUITools.AddChild(grid, ItemPrefab);
				item.name = itemlogic.ItemList[i].ToString();
				//item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelRed";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.ItemList[i]].name;
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.ItemList[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.ItemList[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.Items[itemlogic.ItemList[i]].textureName;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		else if(page.name == "Decoration"){
			uisprite.atlas = BackGroundPurple;
			for(int i = 0;i<itemlogic.DecoList.Count;i++){
				GameObject item = NGUITools.AddChild(grid, ItemPrefab);
				item.name = itemlogic.DecoList[i].ToString();
				//item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelPurple";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.DecoList[i]].name;
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.DecoList[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.DecoList[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.Items[itemlogic.DecoList[i]].textureName;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		else if(page.name == "Inhaler"){
			uisprite.atlas = BackGroundOrange;
			for(int i = 0;i<itemlogic.InhalerList.Count;i++){
				GameObject item = NGUITools.AddChild(grid, ItemPrefab);
				item.name = itemlogic.InhalerList[i].ToString();
				//item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelYellow";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.InhalerList[i]].name;
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.InhalerList[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.Items[itemlogic.InhalerList[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.Items[itemlogic.InhalerList[i]].textureName;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		grid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);
	}
	//Delay calling reposition due to async problem Destroying/Repositionoing.
	//TODO Maybe change later when we have moreItems 
	private void Reposition(){
		grid.GetComponent<UIGrid>().Reposition();

	}
}

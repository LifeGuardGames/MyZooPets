using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StoreUIManager : MonoBehaviour {
	
	public bool isDebug;
	public GameObject ItemPrefab;
	public GameObject ItemSpritePrefab;
	public UIAtlas BackGroundRed;
	public UIAtlas BackGroundGreen;
	public UIAtlas BackGroundOrange;
	public UIAtlas BackGroundPurple;

	//=========================Events====================
	public delegate void CallBack(object sender, EventArgs e);
	public static event CallBack OnStoreClosed; //call when store is closed
	//===================================================
	
	private ItemLogic itemlogic;
	private Inventory inventory;
	private bool changePage;
	private int page;
	private UISprite uisprite;
	private GameObject grid;
	private GameObject inventoryGrid;
	private GameObject toDestroy = null;
	
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

	public void StoreClicked(){
		GetComponent<MoveTweenToggle>().Show();
	}

	public void StoreClosed(){
		GetComponent<MoveTweenToggle>().Hide();
		if(OnStoreClosed != null){
			OnStoreClosed(this, EventArgs.Empty);
		}else{
			Debug.LogError("OnStoreClosed listener is null");
		}
	}
	
	//This function is called when buying an item
	//It creates a icon for the item and move it to Inventory
	//TODO Scales are a little mess up
	public void OnBuyAnimation(GameObject sprite){
		Vector3 origin = sprite.transform.position;
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
		path[0] = origin ;
		path[1] = origin + new Vector3(0,1.5f,0);
		path[2] = origin;
		path[3] = itemPosition;
		
		Hashtable optional = new Hashtable();
		optional.Add("ease",LeanTweenType.easeOutQuad);
		optional.Add ("onComplete","DestroySprite");
		GameObject animationSprite = NGUITools.AddChild(GameObject.Find("Anchor-Center/Store")/*sprite.transform.parent.gameObject*/,ItemSpritePrefab);
		optional.Add("onCompleteTarget",gameObject);
		animationSprite.transform.position = origin;
		animationSprite.transform.localScale = new Vector3(100,100,0);
		animationSprite.GetComponent<UISprite>().spriteName = sprite.GetComponent<UISprite>().spriteName;
		LeanTween.move(animationSprite,path,speed,optional);
		toDestroy =animationSprite;
	}
	
	//helper for Buying Animation
	public void DestroySprite(){
		Destroy(toDestroy);
		toDestroy = null;
	}
	
	//Called when "Buy" is clicked
	public void OnBuyButton(GameObject button){
		int cost = int.Parse(button.transform.parent.FindChild("ItemCost").GetComponent<UILabel>().text);
		int itemId = int.Parse(button.transform.parent.name);
		if(DataManager.Stars >= cost){
			inventory.AddItem(itemId, 1);
			DataManager.SubtractStars(cost);
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
			for(int i = 0;i<itemlogic.foodlist.Count;i++){
				GameObject item = NGUITools.AddChild(grid,ItemPrefab);
				item.name = itemlogic.foodlist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelBlue";
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.items[itemlogic.foodlist[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.items[itemlogic.foodlist[i]].cost.ToString();
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.foodlist[i]].name;
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.foodlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		else if(page.name == "Item"){
			uisprite.atlas = BackGroundGreen;
			for(int i = 0;i<itemlogic.itemlist.Count;i++){
				GameObject item = NGUITools.AddChild(grid,ItemPrefab);
				item.name = itemlogic.itemlist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelRed";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.itemlist[i]].name;
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.items[itemlogic.itemlist[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.items[itemlogic.itemlist[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.itemlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		else if(page.name == "Decoration"){
			uisprite.atlas = BackGroundPurple;
			for(int i = 0;i<itemlogic.decolist.Count;i++){
				GameObject item = NGUITools.AddChild(grid,ItemPrefab);
				item.name = itemlogic.decolist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelPurple";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.decolist[i]].name;
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.items[itemlogic.decolist[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.items[itemlogic.decolist[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.decolist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		else if(page.name == "Inhaler"){
			uisprite.atlas = BackGroundOrange;
			for(int i = 0;i<itemlogic.inhalerlist.Count;i++){
				GameObject item = NGUITools.AddChild(grid,ItemPrefab);
				item.name = itemlogic.inhalerlist[i].ToString();
				item.transform.FindChild("ItemBackground").GetComponent<UISprite>().spriteName = "panelYellow";
				item.transform.FindChild("ItemName").GetComponent<UILabel>().text = itemlogic.items[itemlogic.inhalerlist[i]].name;
				item.transform.FindChild("ItemDescription").GetComponent<UILabel>().text = itemlogic.items[itemlogic.inhalerlist[i]].description;
				item.transform.FindChild("ItemCost").GetComponent<UILabel>().text = itemlogic.items[itemlogic.inhalerlist[i]].cost.ToString();
				item.transform.FindChild("ItemTexture").GetComponent<UISprite>().spriteName = itemlogic.items[itemlogic.inhalerlist[i]].name;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().target = gameObject;
				item.transform.FindChild("BuyButton").GetComponent<UIButtonMessage>().functionName = "OnBuyButton";
			}
		}
		grid.GetComponent<UIGrid>().Reposition();
		Invoke("Reposition",0.00000001f);
	}
	//Delay calling reposition due to async problem Destroying/Repositionoing.
	//TODO Maybe change later when we have more items 
	private void Reposition(){
		grid.GetComponent<UIGrid>().Reposition();
		
	}
}
